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

        [Header("Wool ball")]
        [Tooltip("Placeholder wool sprite until the unrolling animation is ready.")]
        [SerializeField] private Sprite _woolSprite;

        // How many loading.tip.N keys exist in LocalizedStrings (keep in sync).
        private const int TipCount = 8;

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private Image _background;
        private TMP_Text _titleText;
        private TMP_Text _tipText;
        private Image _woolImage;
        private RectTransform _woolRect;
        private bool _isTransitioning;

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
            // Prefer the designer-editable prefab if present; fall back to the old code path
            // so the loading screen still works if Resources/UI/LoadingScreen.prefab is missing.
            if (TryBuildFromPrefab()) return;
            BuildFromCode();
        }

        private bool TryBuildFromPrefab()
        {
            var prefab = Resources.Load<GameObject>("UI/LoadingScreen");
            if (prefab == null) return false;

            var instance = Instantiate(prefab, transform);
            instance.name = "LoadingScreenUI";

            _canvas = instance.GetComponent<Canvas>();
            _canvasGroup = instance.GetComponent<CanvasGroup>();
            if (_canvas == null || _canvasGroup == null)
            {
                Debug.LogWarning("[LoadingScreen] Prefab is missing Canvas/CanvasGroup. Falling back to code build.");
                Destroy(instance);
                return false;
            }

            // Make sure overlays draw on top
            _canvas.sortingOrder = 999;

            // Resolve named children (names must match the prefab builder)
            var root = instance.transform;
            _background = FindByName<Image>(root, "Background");
            _titleText  = FindByName<TMP_Text>(root, "TitleLabel");
            _tipText    = FindByName<TMP_Text>(root, "TipLabel");

            var woolTransform = FindChildDeep(root, "WoolBall");
            if (woolTransform != null)
            {
                _woolImage = woolTransform.GetComponent<Image>();
                _woolRect  = woolTransform as RectTransform;
                if (_woolImage != null && _woolImage.sprite == null)
                {
                    if (_woolSprite == null)
                        _woolSprite = Resources.Load<Sprite>("UI/LoadingWoolBall");
                    if (_woolSprite != null) _woolImage.sprite = _woolSprite;
                }
            }

            return true;
        }

        private static T FindByName<T>(Transform root, string name) where T : Component
        {
            var t = FindChildDeep(root, name);
            return t != null ? t.GetComponent<T>() : null;
        }

        private static Transform FindChildDeep(Transform root, string name)
        {
            if (root.name == name) return root;
            for (int i = 0; i < root.childCount; i++)
            {
                var found = FindChildDeep(root.GetChild(i), name);
                if (found != null) return found;
            }
            return null;
        }

        private void BuildFromCode()
        {
            // Canvas: overlay, highest sort order
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 999;

            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            gameObject.AddComponent<GraphicRaycaster>();

            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = false;

            // Background (full-screen flat colour — swap for a sprite later if needed)
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform, false);
            _background = bgObj.AddComponent<Image>();
            _background.color = _backgroundColor;
            var bgRt = bgObj.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;

            // --- Title "Chargement" ---
            var titleObj = new GameObject("TitleLabel");
            titleObj.transform.SetParent(transform, false);
            _titleText = titleObj.AddComponent<TextMeshProUGUI>();
            _titleText.text = Core.LocalizedStrings.Get("ui.loading");
            _titleText.fontSize = 96;
            _titleText.fontStyle = FontStyles.Bold;
            _titleText.color = new Color(1f, 1f, 1f, 0.95f);
            _titleText.alignment = TextAlignmentOptions.Center;

            var titleRt = titleObj.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0f, 0.78f);
            titleRt.anchorMax = new Vector2(1f, 0.92f);
            titleRt.offsetMin = Vector2.zero;
            titleRt.offsetMax = Vector2.zero;

            // --- Wool ball (centered, placeholder sprite until the unroll animation is ready) ---
            var woolObj = new GameObject("WoolBall");
            woolObj.transform.SetParent(transform, false);
            _woolImage = woolObj.AddComponent<Image>();
            _woolImage.preserveAspect = true;
            if (_woolSprite == null)
                _woolSprite = Resources.Load<Sprite>("UI/LoadingWoolBall");
            if (_woolSprite != null)
                _woolImage.sprite = _woolSprite;
            else
                _woolImage.color = new Color(1f, 1f, 1f, 0.0f); // invisible if no placeholder

            _woolRect = woolObj.GetComponent<RectTransform>();
            _woolRect.anchorMin = new Vector2(0.5f, 0.5f);
            _woolRect.anchorMax = new Vector2(0.5f, 0.5f);
            _woolRect.anchoredPosition = Vector2.zero;
            _woolRect.sizeDelta = new Vector2(320f, 320f);

            // --- Tip text (random) ---
            var tipObj = new GameObject("TipLabel");
            tipObj.transform.SetParent(transform, false);
            _tipText = tipObj.AddComponent<TextMeshProUGUI>();
            _tipText.fontSize = 40;
            _tipText.fontStyle = FontStyles.Italic;
            _tipText.color = new Color(1f, 1f, 1f, 0.7f);
            _tipText.alignment = TextAlignmentOptions.Center;
            _tipText.enableWordWrapping = true;
            _tipText.text = PickRandomTip();

            var tipRt = tipObj.GetComponent<RectTransform>();
            tipRt.anchorMin = new Vector2(0.1f, 0.12f);
            tipRt.anchorMax = new Vector2(0.9f, 0.22f);
            tipRt.offsetMin = Vector2.zero;
            tipRt.offsetMax = Vector2.zero;
        }

        private static string PickRandomTip()
        {
            int index = UnityEngine.Random.Range(0, TipCount);
            return Core.LocalizedStrings.Get($"loading.tip.{index}");
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

            // Refresh content for this transition
            if (_titleText != null)
                _titleText.text = Core.LocalizedStrings.Get("ui.loading");
            if (_tipText != null)
                _tipText.text = PickRandomTip();

            // Fade in
            _canvas.enabled = true;
            _canvasGroup.alpha = 0f;

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

            _isTransitioning = false;
        }
    }
}
