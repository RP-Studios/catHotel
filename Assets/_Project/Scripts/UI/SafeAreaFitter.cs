using UnityEngine;

namespace CatHotel.UI
{
    /// <summary>
    /// Anchors this RectTransform to Screen.safeArea so its children stay
    /// inside the device's usable area. Defaults to horizontal-only padding —
    /// matches the common tablet case where the long edges have cameras /
    /// gesture zones but the top and bottom are fine.
    /// Re-evaluates on resolution / orientation changes.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class SafeAreaFitter : MonoBehaviour
    {
        [SerializeField] private bool _applyHorizontal = true;
        [SerializeField] private bool _applyVertical = false;

        private RectTransform _rt;
        private Rect _lastArea;
        private Vector2Int _lastResolution;

        private void OnEnable()
        {
            _rt = (RectTransform)transform;
            _lastArea = default;
            Apply();
        }

        private void Update()
        {
            if (NeedsUpdate()) Apply();
        }

        private bool NeedsUpdate()
        {
            return Screen.safeArea != _lastArea
                || Screen.width != _lastResolution.x
                || Screen.height != _lastResolution.y;
        }

        private void Apply()
        {
            if (_rt == null) _rt = (RectTransform)transform;
            if (Screen.width <= 0 || Screen.height <= 0) return;

            var area = Screen.safeArea;
            _lastArea = area;
            _lastResolution = new Vector2Int(Screen.width, Screen.height);

            Vector2 min = area.position;
            Vector2 max = area.position + area.size;
            min.x /= Screen.width;
            max.x /= Screen.width;
            min.y /= Screen.height;
            max.y /= Screen.height;

            if (!_applyHorizontal) { min.x = 0f; max.x = 1f; }
            if (!_applyVertical)   { min.y = 0f; max.y = 1f; }

            _rt.anchorMin = min;
            _rt.anchorMax = max;
            _rt.offsetMin = Vector2.zero;
            _rt.offsetMax = Vector2.zero;
        }
    }
}
