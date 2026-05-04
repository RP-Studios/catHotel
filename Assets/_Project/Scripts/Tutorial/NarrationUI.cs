using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CatHotel.Core;

namespace CatHotel.Tutorial
{
    /// <summary>
    /// Canvas overlay showing a speaker portrait (left) + speech bubble (right) + text.
    /// Handles typewriter reveal and tap-to-advance.
    /// All UI elements (including Skip link + Confirm popup) are serialized refs on the prefab.
    /// </summary>
    public class NarrationUI : MonoBehaviour
    {
        [Header("References (wired by TutorialManager or ProtoSceneSetup)")]
        [SerializeField] private Image _portraitImage;
        [SerializeField] private TMP_Text _speakerLabel;
        [SerializeField] private Image _bubbleImage;
        [SerializeField] private TMP_Text _dialogueText;
        [SerializeField] private RectTransform _containerRect;

        [Header("Typewriter")]
        [SerializeField] private float _charsPerSecond = 60f;

        [Header("Transitions")]
        [SerializeField] private float _slideInDuration = 0.35f;
        [SerializeField] private float _slideOutDuration = 0.25f;

        [Header("Skip Tutorial Link (shown only on step 0)")]
        [Tooltip("The whole 'Skip tutorial' clickable label. Toggled active/inactive.")]
        [SerializeField] private GameObject _skipLinkGo;
        [Tooltip("Text component of the skip link — auto-localized to 'tuto.skip.link'.")]
        [SerializeField] private TMP_Text _skipLinkText;
        [Tooltip("Button that opens the confirmation popup when tapped.")]
        [SerializeField] private Button _skipLinkButton;

        [Header("Skip Confirm Popup")]
        [Tooltip("The whole popup overlay. Toggled active/inactive.")]
        [SerializeField] private GameObject _confirmPopupGo;
        [Tooltip("Text component of the 'Skip tutorial?' question — auto-localized to 'tuto.skip.ask'.")]
        [SerializeField] private TMP_Text _confirmAskText;
        [Tooltip("'Ready' button (confirms the skip). Uses ready.png frames via UIFrameAnimator.")]
        [SerializeField] private Button _confirmYesButton;
        [Tooltip("'Cancel' button (cancels the skip). Uses cancel.png frames via UIFrameAnimator.")]
        [SerializeField] private Button _confirmNoButton;

        private CanvasGroup _canvasGroup;
        private bool _isVisible;
        private bool _typewriterDone;
        private string _fullText;
        private Coroutine _typewriterCo;
        private Tween _slideTween;

        // Cached original RectTransform state (left-aligned default) so we can mirror to right and back.
        private struct AnchorSnapshot
        {
            public RectTransform rt;
            public Vector2 min, max, pivot, anchoredPos, sizeDelta;
            public Vector3 scale;
        }
        private System.Collections.Generic.List<AnchorSnapshot> _originalAnchors;
        private bool _rightAlignedActive;

        /// <summary>Fired when the player taps the bubble AFTER the text is fully revealed.</summary>
        public event Action OnTapAdvance;

        /// <summary>Fired when the player confirms skipping the tutorial.</summary>
        public event Action OnSkipConfirmed;

        /// <summary>True while the overlay is displayed on screen.</summary>
        public bool IsVisible => _isVisible;

        private void Awake()
        {
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
            if (_canvasGroup == null && _containerRect != null)
                _canvasGroup = _containerRect.gameObject.AddComponent<CanvasGroup>();

            // Force this Canvas to draw on top of every other UI so tap-to-advance
            // never gets eaten by overlapping panels (CatInfoPanel, shop, etc.).
            var canvas = GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = 9999;
            }

            // Self-wire Button.onClick on the bubble image → OnBubbleTapped
            WireButton(_bubbleImage);

            // Wire skip-related buttons
            if (_skipLinkButton != null)
            {
                _skipLinkButton.onClick.RemoveListener(ShowConfirmPopup);
                _skipLinkButton.onClick.AddListener(ShowConfirmPopup);
            }
            if (_confirmYesButton != null)
            {
                _confirmYesButton.onClick.RemoveListener(OnConfirmYes);
                _confirmYesButton.onClick.AddListener(OnConfirmYes);
            }
            if (_confirmNoButton != null)
            {
                _confirmNoButton.onClick.RemoveListener(OnConfirmNo);
                _confirmNoButton.onClick.AddListener(OnConfirmNo);
            }

            // Start hidden
            if (_skipLinkGo != null) _skipLinkGo.SetActive(false);
            if (_confirmPopupGo != null) _confirmPopupGo.SetActive(false);

            LocalizedStrings.OnLanguageChanged += ApplySkipLocalization;
            ApplySkipLocalization();

            Hide(immediate: true);
        }

        private void OnDestroy()
        {
            LocalizedStrings.OnLanguageChanged -= ApplySkipLocalization;
        }

        private void WireButton(Image bubbleImg)
        {
            if (bubbleImg == null) return;
            var btn = bubbleImg.GetComponent<Button>();
            if (btn == null) btn = bubbleImg.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(OnBubbleTapped);
        }

        private void ApplySkipLocalization()
        {
            if (_skipLinkText != null)   _skipLinkText.text   = LocalizedStrings.Get("tuto.skip.link");
            if (_confirmAskText != null) _confirmAskText.text = LocalizedStrings.Get("tuto.skip.ask");
        }

        private void CacheOriginalAnchors()
        {
            _originalAnchors = new System.Collections.Generic.List<AnchorSnapshot>();
            void Snap(RectTransform rt)
            {
                if (rt == null) return;
                _originalAnchors.Add(new AnchorSnapshot
                {
                    rt = rt,
                    min = rt.anchorMin, max = rt.anchorMax, pivot = rt.pivot,
                    anchoredPos = rt.anchoredPosition, sizeDelta = rt.sizeDelta,
                    scale = rt.localScale
                });
            }
            Snap(_portraitImage != null ? _portraitImage.rectTransform : null);
            Snap(_speakerLabel != null ? _speakerLabel.rectTransform : null);
            Snap(_bubbleImage != null ? _bubbleImage.rectTransform : null);
            Snap(_dialogueText != null ? _dialogueText.rectTransform : null);
        }

        private void ApplyAlignment(bool rightAligned)
        {
            if (_originalAnchors == null) CacheOriginalAnchors();
            if (_originalAnchors == null) return;
            if (rightAligned == _rightAlignedActive && _originalAnchors.Count > 0) return;

            foreach (var snap in _originalAnchors)
            {
                if (snap.rt == null) continue;
                if (rightAligned)
                {
                    // Full horizontal mirror: anchors, pivot AND anchoredPosition.x
                    // (a bubble offset 100px to the right of its anchor must end up
                    //  100px to the left of the mirrored anchor).
                    snap.rt.anchorMin       = new Vector2(1f - snap.max.x, snap.min.y);
                    snap.rt.anchorMax       = new Vector2(1f - snap.min.x, snap.max.y);
                    snap.rt.pivot           = new Vector2(1f - snap.pivot.x, snap.pivot.y);
                    snap.rt.anchoredPosition= new Vector2(-snap.anchoredPos.x, snap.anchoredPos.y);
                    snap.rt.sizeDelta       = snap.sizeDelta;
                }
                else
                {
                    snap.rt.anchorMin        = snap.min;
                    snap.rt.anchorMax        = snap.max;
                    snap.rt.pivot            = snap.pivot;
                    snap.rt.anchoredPosition = snap.anchoredPos;
                    snap.rt.sizeDelta        = snap.sizeDelta;
                }
            }

            // Mirror only the bubble visual so the tail/arrow points in the right direction.
            if (_bubbleImage != null)
            {
                var s = _bubbleImage.transform.localScale;
                s.x = rightAligned ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
                _bubbleImage.transform.localScale = s;
            }

            _rightAlignedActive = rightAligned;
        }

        /// <summary>
        /// Show the narration overlay with a specific speaker, expression, bubble style and text.
        /// </summary>
        public void Show(string speakerLabelKey, Sprite portrait, string textKey)
        {
            Show(speakerLabelKey, portrait, textKey, showSkipLink: false, rightAligned: false);
        }

        public void Show(string speakerLabelKey, Sprite portrait, string textKey, bool showSkipLink)
        {
            Show(speakerLabelKey, portrait, textKey, showSkipLink, rightAligned: false);
        }

        /// <summary>
        /// Show the narration overlay. rightAligned=true mirrors the layout (portrait on the right,
        /// bubble extending to the left) so the highlighted HUD element on the left isn't obscured.
        /// </summary>
        public void Show(string speakerLabelKey, Sprite portrait, string textKey, bool showSkipLink, bool rightAligned)
        {
            ApplyAlignment(rightAligned);
            gameObject.SetActive(true);

            if (_skipLinkGo != null) _skipLinkGo.SetActive(showSkipLink);
            if (_confirmPopupGo != null) _confirmPopupGo.SetActive(false);

            // Portrait + label
            if (_portraitImage != null)
            {
                _portraitImage.sprite = portrait;
                _portraitImage.enabled = portrait != null;
                _portraitImage.preserveAspect = true;
            }
            if (_speakerLabel != null)
            {
                string label = string.IsNullOrEmpty(speakerLabelKey) ? "" : LocalizedStrings.Get(speakerLabelKey);
                _speakerLabel.text = label;
                _speakerLabel.gameObject.SetActive(!string.IsNullOrEmpty(label));
            }

            if (_bubbleImage != null)
                _bubbleImage.gameObject.SetActive(true);

            // Text
            _fullText = LocalizedStrings.Get(textKey);
            if (_dialogueText != null)
            {
                _dialogueText.text = "";
                _dialogueText.gameObject.SetActive(true);
            }

            // Slide in
            _isVisible = true;
            _typewriterDone = false;

            if (_containerRect != null)
            {
                _slideTween?.Kill();
                float offY = -_containerRect.rect.height;
                _containerRect.anchoredPosition = new Vector2(0f, offY);
                _slideTween = _containerRect.DOAnchorPosY(0f, _slideInDuration)
                    .SetEase(Ease.OutCubic)
                    .SetUpdate(true);
            }
            if (_canvasGroup != null) _canvasGroup.alpha = 1f;

            // Start typewriter after slide
            if (_typewriterCo != null) StopCoroutine(_typewriterCo);
            _typewriterCo = StartCoroutine(TypewriterCoroutine());
        }

        /// <summary>Hide the narration overlay.</summary>
        public void Hide(bool immediate = false)
        {
            if (_typewriterCo != null) { StopCoroutine(_typewriterCo); _typewriterCo = null; }
            _slideTween?.Kill();

            if (_skipLinkGo != null) _skipLinkGo.SetActive(false);
            if (_confirmPopupGo != null) _confirmPopupGo.SetActive(false);

            if (immediate || _containerRect == null)
            {
                _isVisible = false;
                if (_canvasGroup != null) _canvasGroup.alpha = 0f;
                gameObject.SetActive(false);
                return;
            }

            float offY = -_containerRect.rect.height;
            _slideTween = _containerRect.DOAnchorPosY(offY, _slideOutDuration)
                .SetEase(Ease.InCubic)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _isVisible = false;
                    if (_canvasGroup != null) _canvasGroup.alpha = 0f;
                    gameObject.SetActive(false);
                });
        }

        /// <summary>Call this from a tap handler (Button.onClick or similar).</summary>
        public void OnBubbleTapped()
        {
            if (!_isVisible) return;

            if (!_typewriterDone)
            {
                // Skip typewriter → reveal all
                SkipTypewriter();
                return;
            }

            // Text fully revealed → signal advance
            OnTapAdvance?.Invoke();
        }

        private void SkipTypewriter()
        {
            if (_typewriterCo != null) { StopCoroutine(_typewriterCo); _typewriterCo = null; }
            if (_dialogueText != null) _dialogueText.text = _fullText;
            _typewriterDone = true;
        }

        private IEnumerator TypewriterCoroutine()
        {
            // Wait for slide-in
            yield return new WaitForSecondsRealtime(_slideInDuration * 0.5f);

            if (_dialogueText == null || string.IsNullOrEmpty(_fullText))
            {
                _typewriterDone = true;
                yield break;
            }

            float interval = 1f / _charsPerSecond;
            for (int i = 1; i <= _fullText.Length; i++)
            {
                // Skip rich text tags (e.g. <b>, </b>)
                while (i < _fullText.Length && _fullText[i - 1] == '<')
                {
                    int closeIdx = _fullText.IndexOf('>', i);
                    if (closeIdx < 0) break;
                    i = closeIdx + 2; // past the '>' and to the next char
                }
                _dialogueText.text = _fullText[..Mathf.Min(i, _fullText.Length)];
                yield return new WaitForSecondsRealtime(interval);
            }

            _dialogueText.text = _fullText;
            _typewriterDone = true;
            _typewriterCo = null;
        }

        // ==================== SKIP TUTORIAL ====================

        private void ShowConfirmPopup()
        {
            if (_confirmPopupGo != null) _confirmPopupGo.SetActive(true);
        }

        private void HideConfirmPopup()
        {
            if (_confirmPopupGo != null) _confirmPopupGo.SetActive(false);
        }

        private void OnConfirmYes()
        {
            HideConfirmPopup();
            OnSkipConfirmed?.Invoke();
        }

        private void OnConfirmNo()
        {
            HideConfirmPopup();
        }
    }
}
