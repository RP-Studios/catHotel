using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

namespace CatHotel.UI
{
    /// <summary>
    /// Manages the "ParametersPanel" overlay.
    /// Opens from OptionsPanel > ParamsOption, slides in from the right.
    /// </summary>
    public class ParametersPanel : MonoBehaviour
    {
        private RectTransform _panel;
        private RectTransform _closeRect;
        private float _panelWidth;
        private Tween _slideTween;
        private bool _isOpen;

        public bool IsOpen => _isOpen;

        public System.Action OnClosed;

        private void Start()
        {
            var panelObj = FindInactiveByName("ParametersPanel");
            if (panelObj == null) return;

            panelObj.SetActive(true);
            _panel = panelObj.GetComponent<RectTransform>();

            // Ensure raycastable background
            var panelImg = panelObj.GetComponent<Image>();
            if (panelImg == null)
            {
                panelImg = panelObj.AddComponent<Image>();
                panelImg.color = Color.clear;
            }
            panelImg.raycastTarget = true;

            // Get panel width
            Canvas.ForceUpdateCanvases();
            _panelWidth = _panel.rect.width;
            if (_panelWidth <= 0f) _panelWidth = 800f;

            // Start hidden off-screen right
            var pos = _panel.anchoredPosition;
            pos.x = _panelWidth;
            _panel.anchoredPosition = pos;

            // CloseImage button
            var closeTransform = FindInChildren(panelObj.transform, "CloseImage");
            if (closeTransform != null)
            {
                _closeRect = closeTransform.GetComponent<RectTransform>();
                if (closeTransform.GetComponent<ButtonJuice>() == null)
                    closeTransform.gameObject.AddComponent<ButtonJuice>();
            }
        }

        private void Update()
        {
            if (!_isOpen) return;

            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;

            Vector2 screenPos = pointer.position.ReadValue();

            if (_closeRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_closeRect, screenPos, null))
            {
                Close();
            }
        }

        public void Open()
        {
            if (_panel == null) return;
            _isOpen = true;
            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosX(0f, 0.35f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }

        public void Close()
        {
            if (_panel == null) return;
            _isOpen = false;
            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosX(_panelWidth, 0.25f)
                .SetEase(Ease.InCubic)
                .SetUpdate(true);
            OnClosed?.Invoke();
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
