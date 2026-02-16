using UnityEngine;
using UnityEngine.InputSystem;

namespace CatHotel.Input
{
    public class CameraController : MonoBehaviour
    {
        [Header("Zoom")]
        [SerializeField] private float _minOrthoSize = 3f;
        [SerializeField] private float _maxOrthoSize = 8.5f;
        [SerializeField] private float _zoomSpeed    = 0.5f;

        [Header("Bounds")]
        [SerializeField] private Vector2 _gridMin = Vector2.zero;
        [SerializeField] private Vector2 _gridMax = new(24f, 16f);
        [SerializeField] private float   _padding = 0.5f;

        private Camera _cam;
        private bool _isPanning;
        private Vector3 _panOrigin;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _cam.orthographicSize = _maxOrthoSize;
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
            HandleMouseZoom();
            HandleMousePan();
            HandleTouchPinchZoom();
            ClampPosition();
        }

        private void HandleMouseZoom()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            float scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) < 0.01f) return;

            _cam.orthographicSize = Mathf.Clamp(
                _cam.orthographicSize - scroll * _zoomSpeed,
                _minOrthoSize, _maxOrthoSize);
        }

        private void HandleMousePan()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            Vector2 pos = mouse.position.ReadValue();
            if (float.IsInfinity(pos.x) || float.IsNaN(pos.x)) return;

            // Right-click or middle-click to pan
            bool panButton = mouse.rightButton.isPressed || mouse.middleButton.isPressed;
            bool panStart  = mouse.rightButton.wasPressedThisFrame || mouse.middleButton.wasPressedThisFrame;
            bool panEnd    = mouse.rightButton.wasReleasedThisFrame || mouse.middleButton.wasReleasedThisFrame;

            if (panStart)
            {
                _isPanning = true;
                _panOrigin = ScreenToWorld(pos);
            }

            if (_isPanning && panButton)
            {
                Vector3 current = ScreenToWorld(pos);
                transform.position += _panOrigin - current;
            }

            if (panEnd)
                _isPanning = false;
        }

        private void HandleTouchPinchZoom()
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

            float prevDist = Vector2.Distance(p0 - d0, p1 - d1);
            float curDist  = Vector2.Distance(p0, p1);

            if (Mathf.Abs(prevDist) < 0.01f) return;

            float factor = prevDist / curDist;
            _cam.orthographicSize = Mathf.Clamp(
                _cam.orthographicSize * factor,
                _minOrthoSize, _maxOrthoSize);
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
