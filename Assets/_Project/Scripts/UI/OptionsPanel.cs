using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

namespace CatHotel.UI
{
    /// <summary>
    /// Manages the "OptionsPanel" overlay.
    /// Opens when tapping "System" button, slides in from the right.
    /// </summary>
    public class OptionsPanel : MonoBehaviour
    {
        private RectTransform _panel;
        private GameObject _panelObj;
        private float _panelWidth;
        private Tween _slideTween;
        private bool _isOpen;

        // Button hit rects
        private RectTransform _backToGameRect;
        private RectTransform _paramsRect;
        private RectTransform _mainMenuRect;

        private ParametersPanel _parametersPanel;

        public bool IsOpen => _isOpen;

        private void Start()
        {
            // Find the OptionsPanel (may be inactive)
            _panelObj = FindInactiveByName("OptionsPanel");
            if (_panelObj == null) return;

            _panel = _panelObj.GetComponent<RectTransform>();

            // Activate briefly to measure, then deactivate
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

            // Position off-screen and deactivate
            var pos = _panel.anchoredPosition;
            pos.x = _panelWidth;
            _panel.anchoredPosition = pos;
            _panelObj.SetActive(false);

            // Find button rects + add tap juice
            _backToGameRect = FindRect(_panelObj, "BackToGameOption");
            _paramsRect = FindRect(_panelObj, "ParamsOption");
            _mainMenuRect = FindRect(_panelObj, "MainMenuOption");
            AddJuice(_backToGameRect);
            AddJuice(_paramsRect);
            AddJuice(_mainMenuRect);

            // Get or add ParametersPanel on same object
            _parametersPanel = GetComponent<ParametersPanel>();
            if (_parametersPanel == null)
                _parametersPanel = gameObject.AddComponent<ParametersPanel>();

            // Wire "System" button to open this panel
            var systemObj = GameObject.Find("System");
            if (systemObj != null)
            {
                var btn = systemObj.GetComponent<Button>();
                if (btn == null) btn = systemObj.AddComponent<Button>();
                btn.onClick.AddListener(Open);

                // Ensure graphic for raycast
                var graphic = systemObj.GetComponent<Graphic>();
                if (graphic != null) graphic.raycastTarget = true;

                AddJuice(systemObj.GetComponent<RectTransform>());
            }
        }

        private void Update()
        {
            if (!_isOpen) return;

            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;

            Vector2 screenPos = pointer.position.ReadValue();

            // BackToGame => close
            if (_backToGameRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_backToGameRect, screenPos, null))
            {
                Close();
                return;
            }

            // Params => open ParametersPanel
            if (_paramsRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_paramsRect, screenPos, null))
            {
                OpenParameters();
                return;
            }

            // MainMenu => return to Boot scene
            if (_mainMenuRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_mainMenuRect, screenPos, null))
            {
                ReturnToMainMenu();
                return;
            }
        }

        public void Open()
        {
            if (_panel == null) return;
            _isOpen = true;
            _panelObj.SetActive(true);
            var p = _panel.anchoredPosition;
            p.x = _panelWidth;
            _panel.anchoredPosition = p;
            Time.timeScale = 0f;
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
                .OnComplete(() =>
                {
                    _panelObj.SetActive(false);
                    Time.timeScale = 1f;
                });
        }

        private void ReturnToMainMenu()
        {
            _isOpen = false;
            LoadingScreen.TransitionTo("Boot", () => Time.timeScale = 1f);
        }

        private void OpenParameters()
        {
            if (_parametersPanel == null) return;
            Close();
            _parametersPanel.Open();
            _parametersPanel.OnClosed = Open; // Return to OptionsPanel when ParametersPanel closes
        }

        private static void AddJuice(RectTransform rt)
        {
            if (rt == null) return;
            if (rt.GetComponent<ButtonJuice>() == null)
                rt.gameObject.AddComponent<ButtonJuice>();
        }

        private static RectTransform FindRect(GameObject root, string childName)
        {
            var t = FindInChildren(root.transform, childName);
            return t != null ? t.GetComponent<RectTransform>() : null;
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
