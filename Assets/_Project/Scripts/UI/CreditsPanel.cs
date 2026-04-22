using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CatHotel.Core;

namespace CatHotel.UI
{
    /// <summary>
    /// Manages the "CreditsPanel" overlay.
    /// Slides in from the right on open, closes via BackAction.
    /// Populates localized labels: CreditsLabel, GameFromLabel,
    /// DirectionLabel, DisclaimerLabel.
    /// </summary>
    public class CreditsPanel : MonoBehaviour
    {
        private RectTransform _panel;
        private GameObject _panelObj;
        private RectTransform _backRect;
        private float _panelWidth;
        private Tween _slideTween;
        private bool _isOpen;

        // Localized labels
        private TMP_Text _creditsLabel;
        private TMP_Text _gameFromLabel;
        private TMP_Text _directionLabel;
        private TMP_Text _disclaimerLabel;

        public bool IsOpen => _isOpen;
        public System.Action OnClosed;

        private void Start()
        {
            _panelObj = FindInactiveByName("CreditsPanel");
            if (_panelObj == null) return;

            _panel = _panelObj.GetComponent<RectTransform>();
            _panelObj.SetActive(true);

            var panelImg = _panelObj.GetComponent<Image>();
            if (panelImg == null)
            {
                panelImg = _panelObj.AddComponent<Image>();
                panelImg.color = Color.clear;
            }
            panelImg.raycastTarget = true;

            Canvas.ForceUpdateCanvases();
            _panelWidth = _panel.rect.width;
            if (_panelWidth <= 0f) _panelWidth = 800f;

            // Start off-screen on the right
            var pos = _panel.anchoredPosition;
            pos.x = _panelWidth;
            _panel.anchoredPosition = pos;
            _panelObj.SetActive(false);

            // Back button
            var backT = FindInChildren(_panelObj.transform, "BackAction");
            if (backT != null)
            {
                _backRect = backT.GetComponent<RectTransform>();
                if (backT.GetComponent<ButtonJuice>() == null)
                    backT.gameObject.AddComponent<ButtonJuice>();
            }

            // Labels
            _creditsLabel   = FindTMP(_panelObj.transform, "CreditsLabel");
            _gameFromLabel  = FindTMP(_panelObj.transform, "GameFromLabel");
            _directionLabel = FindTMP(_panelObj.transform, "DirectionLabel");
            _disclaimerLabel = FindTMP(_panelObj.transform, "DisclaimerLabel");

            ApplyLocalization();
            LocalizedStrings.OnLanguageChanged += ApplyLocalization;
        }

        private void OnDestroy()
        {
            LocalizedStrings.OnLanguageChanged -= ApplyLocalization;
            _slideTween?.Kill();
        }

        private void Update()
        {
            if (!_isOpen) return;

            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;

            Vector2 screenPos = pointer.position.ReadValue();

            // Back button closes the panel
            if (_backRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_backRect, screenPos, null))
            {
                Close();
            }
        }

        private void ApplyLocalization()
        {
            if (_creditsLabel != null)   _creditsLabel.text   = LocalizedStrings.Get("credits.title");
            if (_gameFromLabel != null)  _gameFromLabel.text  = LocalizedStrings.Get("credits.gameby");
            if (_directionLabel != null) _directionLabel.text = LocalizedStrings.Get("credits.direction");
            if (_disclaimerLabel != null) _disclaimerLabel.text = LocalizedStrings.Get("credits.disclaimer");
        }

        public void Open()
        {
            if (_panel == null) return;
            _isOpen = true;
            _panelObj.SetActive(true);
            var p = _panel.anchoredPosition;
            p.x = _panelWidth;
            _panel.anchoredPosition = p;
            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosX(0f, 0.7f)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);
        }

        public void Close()
        {
            if (_panel == null) return;
            _isOpen = false;
            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosX(_panelWidth, 0.5f)
                .SetEase(Ease.InCubic)
                .SetUpdate(true)
                .OnComplete(() => _panelObj.SetActive(false));
            OnClosed?.Invoke();
        }

        // ---- Helpers (same pattern as ParametersPanel) ----

        private static TMP_Text FindTMP(Transform parent, string name)
        {
            var t = FindInChildren(parent, name);
            return t != null ? t.GetComponent<TMP_Text>() : null;
        }

        private static Transform FindInChildren(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name) return child;
                var found = FindInChildren(child, name);
                if (found != null) return found;
            }
            return null;
        }

        private static GameObject FindInactiveByName(string name)
        {
            var go = GameObject.Find(name);
            if (go != null) return go;

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                foreach (var root in scene.GetRootGameObjects())
                {
                    var found = FindInChildren(root.transform, name);
                    if (found != null) return found.gameObject;
                }
            }
            return null;
        }
    }
}
