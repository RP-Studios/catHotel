using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace CatHotel.UI
{
    /// <summary>
    /// Persistent loading screen singleton. Creates its own overlay Canvas.
    /// Call LoadingScreen.TransitionTo("SceneName") from anywhere.
    /// Flow: fade in → load scene async → fade out.
    /// </summary>
    public class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen Instance { get; private set; }

        [Header("Timing")]
        [SerializeField] private float _fadeInDuration = 0.4f;
        [SerializeField] private float _fadeOutDuration = 0.4f;
        [SerializeField] private float _minDisplayTime = 0.8f;

        [Header("Colors")]
        [SerializeField] private Color _backgroundColor = new(0.12f, 0.10f, 0.08f, 1f);

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private Image _background;
        private TMP_Text _loadingText;
        private RectTransform _spinnerRect;
        private bool _isTransitioning;

        private Coroutine _dotsCoroutine;

        public bool IsTransitioning => _isTransitioning;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildUI();
            _canvasGroup.alpha = 0f;
            _canvas.enabled = false;
        }

        private void BuildUI()
        {
            // Canvas: overlay, highest sort order
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 999;

            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            gameObject.AddComponent<GraphicRaycaster>();

            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = false;

            // Background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform, false);
            _background = bgObj.AddComponent<Image>();
            _background.color = _backgroundColor;
            var bgRt = bgObj.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;

            // Spinner (rotating dot)
            var spinnerObj = new GameObject("Spinner");
            spinnerObj.AddComponent<RectTransform>();
            spinnerObj.transform.SetParent(transform, false);
            _spinnerRect = spinnerObj.GetComponent<RectTransform>();
            _spinnerRect.anchorMin = new Vector2(0.5f, 0.4f);
            _spinnerRect.anchorMax = new Vector2(0.5f, 0.4f);
            _spinnerRect.sizeDelta = new Vector2(60f, 60f);

            // Create 3 dots in a circle pattern
            for (int i = 0; i < 3; i++)
            {
                var dotObj = new GameObject($"Dot_{i}");
                dotObj.transform.SetParent(spinnerObj.transform, false);
                var dotImg = dotObj.AddComponent<Image>();
                dotImg.color = new Color(1f, 1f, 1f, 0.6f - i * 0.15f);

                var dotRt = dotObj.GetComponent<RectTransform>();
                float angle = i * 120f * Mathf.Deg2Rad;
                dotRt.anchoredPosition = new Vector2(Mathf.Cos(angle) * 20f, Mathf.Sin(angle) * 20f);
                dotRt.sizeDelta = new Vector2(12f - i * 2f, 12f - i * 2f);

                // Round the dots
                dotImg.type = Image.Type.Simple;
            }

            // Loading text
            var textObj = new GameObject("LoadingText");
            textObj.transform.SetParent(transform, false);
            _loadingText = textObj.AddComponent<TextMeshProUGUI>();
            _loadingText.text = "Chargement";
            _loadingText.fontSize = 36;
            _loadingText.color = new Color(1f, 1f, 1f, 0.7f);
            _loadingText.alignment = TextAlignmentOptions.Center;

            var textRt = textObj.GetComponent<RectTransform>();
            textRt.anchorMin = new Vector2(0.5f, 0.3f);
            textRt.anchorMax = new Vector2(0.5f, 0.3f);
            textRt.sizeDelta = new Vector2(400f, 60f);
        }

        /// <summary>
        /// Transition to a scene with fade in/out loading screen.
        /// </summary>
        public static void TransitionTo(string sceneName, Action onBeforeLoad = null)
        {
            if (Instance == null)
            {
                // Fallback: no loading screen, just load
                SceneManager.LoadScene(sceneName);
                return;
            }

            if (Instance._isTransitioning) return;
            Instance.StartCoroutine(Instance.TransitionCoroutine(sceneName, onBeforeLoad));
        }

        private IEnumerator TransitionCoroutine(string sceneName, Action onBeforeLoad)
        {
            _isTransitioning = true;

            // Fade in
            _canvas.enabled = true;
            _canvasGroup.alpha = 0f;

            // Start spinner rotation + dots animation
            StartSpinner();
            _dotsCoroutine = StartCoroutine(AnimateDots());

            yield return _canvasGroup.DOFade(1f, _fadeInDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true)
                .WaitForCompletion();

            // Callback before load (e.g. reset timeScale)
            onBeforeLoad?.Invoke();

            float minTimer = _minDisplayTime;

            // Ensure loading spreads across frames (set before LoadSceneAsync)
            var prevPriority = Application.backgroundLoadingPriority;
            Application.backgroundLoadingPriority = ThreadPriority.Low;

            // Load scene async
            var op = SceneManager.LoadSceneAsync(sceneName);
            if (op != null)
            {
                op.allowSceneActivation = false;

                // Wait until scene is 90% loaded (Unity holds at 0.9 until allowSceneActivation)
                while (op.progress < 0.9f || minTimer > 0f)
                {
                    minTimer -= Time.unscaledDeltaTime;
                    yield return null;
                }

                op.allowSceneActivation = true;

                // Wait for scene to actually activate
                while (!op.isDone)
                    yield return null;
            }

            // Give new scene a few frames to stabilize (Awake/Start + first render)
            yield return null;
            yield return null;

            Application.backgroundLoadingPriority = prevPriority;

            // Fade out
            yield return _canvasGroup.DOFade(0f, _fadeOutDuration)
                .SetEase(Ease.InQuad)
                .SetUpdate(true)
                .WaitForCompletion();

            _canvas.enabled = false;
            StopSpinner();
            if (_dotsCoroutine != null)
            {
                StopCoroutine(_dotsCoroutine);
                _dotsCoroutine = null;
            }

            _isTransitioning = false;
        }

        private Tween _spinnerTween;

        private void StartSpinner()
        {
            if (_spinnerRect == null) return;
            _spinnerRect.localRotation = Quaternion.identity;
            _spinnerTween = _spinnerRect.DORotate(new Vector3(0, 0, -360f), 1.5f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1)
                .SetUpdate(true);
        }

        private void StopSpinner()
        {
            _spinnerTween?.Kill();
            if (_spinnerRect != null)
                _spinnerRect.localRotation = Quaternion.identity;
        }

        private IEnumerator AnimateDots()
        {
            string baseText = Core.LocalizedStrings.Loading;
            int dots = 0;
            while (true)
            {
                dots = (dots + 1) % 4;
                if (_loadingText != null)
                    _loadingText.text = baseText + new string('.', dots);
                float elapsed = 0f;
                while (elapsed < 0.4f)
                {
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
        }
    }
}
