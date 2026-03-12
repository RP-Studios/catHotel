using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CatHotel.Input
{
    public class CameraController : MonoBehaviour
    {
        [Header("Zoom")]
        [SerializeField] private float _minOrthoSize = 1.5f;
        [SerializeField] private float _maxOrthoSize = 5f;
        [SerializeField] private float _zoomSpeed    = 0.5f;

        [Header("Inertia")]
        [SerializeField] private float _inertiaDamping = 8f;

        [Header("Bounds")]
        [SerializeField] private Vector2 _gridMin = Vector2.zero;
        [SerializeField] private Vector2 _gridMax = new(24f, 16f);
        [SerializeField] private float   _padding = 0.5f;

        private Camera _cam;
        private bool _isPanning;
        private Vector3 _panOrigin;
        private Vector3 _velocity;

        /// <summary>
        /// Set to true to block pan input (e.g. during build mode).
        /// </summary>
        public bool PanLocked { get; set; }

        public float MinOrthoSize => _minOrthoSize;
        public float CurrentOrthoSize => _cam != null ? _cam.orthographicSize : _maxOrthoSize;

        /// <summary>
        /// Max zoom-out capped so the camera never shows more than 95% of the grid,
        /// ensuring there is always room to pan in both landscape and portrait.
        /// </summary>
        public float EffectiveMaxOrthoSize
        {
            get
            {
                if (_cam == null) return _maxOrthoSize;
                float gridW = _gridMax.x - _gridMin.x;
                float gridH = _gridMax.y - _gridMin.y;
                float maxByHeight = gridH * 0.95f / 2f;
                float maxByWidth  = gridW * 0.95f / (2f * _cam.aspect);
                return Mathf.Min(_maxOrthoSize, Mathf.Min(maxByHeight, maxByWidth));
            }
        }

        public event System.Action OnZoomChanged;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _cam.orthographicSize = EffectiveMaxOrthoSize;
        }

        private void Start()
        {
            transform.position = new Vector3(
                (_gridMin.x + _gridMax.x) * 0.5f,
                (_gridMin.y + _gridMax.y) * 0.5f,
                transform.position.z);
        }

        private void LateUpdate()
        {
            HandlePan();
            HandleMouseZoom();
            HandlePinchZoom();
            ApplyInertia();
        }

        /// <summary>
        /// Set zoom from a normalized 0-1 value (0 = max zoom out, 1 = max zoom in).
        /// </summary>
        public void SetZoomNormalized(float t)
        {
            float effective = EffectiveMaxOrthoSize;
            _cam.orthographicSize = Mathf.Lerp(effective, _minOrthoSize, t);
            OnZoomChanged?.Invoke();
        }

        /// <summary>
        /// Get current zoom as a normalized 0-1 value (0 = max zoom out, 1 = max zoom in).
        /// </summary>
        public float GetZoomNormalized()
        {
            float effective = EffectiveMaxOrthoSize;
            if (Mathf.Approximately(effective, _minOrthoSize)) return 1f;
            return Mathf.InverseLerp(effective, _minOrthoSize, _cam.orthographicSize);
        }

        private void HandlePan()
        {
            if (PanLocked)
            {
                _isPanning = false;
                return;
            }

            // Skip 1-finger pan when 2 fingers are down (pinch mode)
            var touch = Touchscreen.current;
            if (touch != null &&
                touch.touches[0].press.isPressed &&
                touch.touches[1].press.isPressed)
            {
                _isPanning = false;
                return;
            }

            var pointer = Pointer.current;
            if (pointer == null) return;

            Vector2 screenPos = pointer.position.ReadValue();
            if (float.IsInfinity(screenPos.x) || float.IsNaN(screenPos.x)) return;

            if (pointer.press.wasPressedThisFrame)
            {
                // Don't start pan if clicking on UI
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                _isPanning = true;
                _panOrigin = ScreenToWorld(screenPos);
                _velocity = Vector3.zero;
            }

            if (_isPanning && pointer.press.isPressed)
            {
                Vector3 current = ScreenToWorld(screenPos);
                Vector3 delta = _panOrigin - current;
                transform.position += delta;

                // Smooth velocity tracking for inertia
                if (Time.unscaledDeltaTime > 0.0001f)
                    _velocity = Vector3.Lerp(_velocity, delta / Time.unscaledDeltaTime, 0.3f);
            }

            if (_isPanning && pointer.press.wasReleasedThisFrame)
                _isPanning = false;
        }

        private void HandleMouseZoom()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            float scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) < 0.01f) return;

            // Normalize scroll (Windows returns ±120 per notch)
            float normalizedScroll = scroll / 120f;

            _cam.orthographicSize = Mathf.Clamp(
                _cam.orthographicSize - normalizedScroll * _zoomSpeed,
                _minOrthoSize, EffectiveMaxOrthoSize);
            OnZoomChanged?.Invoke();
        }

        private void HandlePinchZoom()
        {
            var touch = Touchscreen.current;
            if (touch == null) return;

            var t0 = touch.touches[0];
            var t1 = touch.touches[1];

            if (!t0.press.isPressed || !t1.press.isPressed) return;

            Vector2 p0 = t0.position.ReadValue();
            Vector2 p1 = t1.position.ReadValue();
            Vector2 d0 = t0.delta.ReadValue();
            Vector2 d1 = t1.delta.ReadValue();

            // Pinch zoom
            float prevDist = Vector2.Distance(p0 - d0, p1 - d1);
            float curDist  = Vector2.Distance(p0, p1);

            if (Mathf.Abs(prevDist) > 0.01f)
            {
                float factor = prevDist / curDist;
                _cam.orthographicSize = Mathf.Clamp(
                    _cam.orthographicSize * factor,
                    _minOrthoSize, EffectiveMaxOrthoSize);
                OnZoomChanged?.Invoke();
            }

            // 2-finger pan via midpoint
            Vector2 midPrev = (p0 - d0 + p1 - d1) * 0.5f;
            Vector2 midCur  = (p0 + p1) * 0.5f;
            Vector3 worldPrev = ScreenToWorld(midPrev);
            Vector3 worldCur  = ScreenToWorld(midCur);
            transform.position += worldPrev - worldCur;
        }

        private void ApplyInertia()
        {
            if (_isPanning || _velocity.sqrMagnitude < 0.01f)
            {
                _velocity = Vector3.zero;
                return;
            }

            transform.position += _velocity * Time.unscaledDeltaTime;
            // Linear approximation of Exp decay — cheaper, visually identical
            float decay = Mathf.Max(0f, 1f - _inertiaDamping * Time.unscaledDeltaTime);
            _velocity *= decay;

            if (_velocity.sqrMagnitude < 0.01f)
                _velocity = Vector3.zero;
        }

        private void ClampPosition()
        {
            float halfH = _cam.orthographicSize;
            float halfW = halfH * _cam.aspect;

            float minX = _gridMin.x - _padding + halfW;
            float maxX = _gridMax.x + _padding - halfW;
            float minY = _gridMin.y - _padding + halfH;
            float maxY = _gridMax.y + _padding - halfH;

            if (minX > maxX) minX = maxX = (_gridMin.x + _gridMax.x) * 0.5f;
            if (minY > maxY) minY = maxY = (_gridMin.y + _gridMax.y) * 0.5f;

            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;
        }

        private Vector3 ScreenToWorld(Vector2 screenPos)
        {
            return _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        }
    }
}
