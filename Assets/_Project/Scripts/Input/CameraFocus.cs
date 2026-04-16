using UnityEngine;

namespace CatHotel.Input
{
    /// <summary>
    /// Smooth camera focus on a moving target (e.g. a selected cat).
    /// - Focus(t): locks user pan, smoothly follows target each frame.
    /// - Focus(t) again with a different t: retargets and smoothly drifts to the new target.
    /// - Release(): smoothly returns to the position the camera was at before the first Focus(),
    ///   then restores user pan.
    /// No zoom changes — only X/Y translation.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFocus : MonoBehaviour
    {
        [Header("Smoothing")]
        [Tooltip("Higher = snappier follow. Lower = floatier. Frame-rate independent.")]
        [SerializeField] private float _followDamping = 8f;

        [Tooltip("Distance at which the release slide is considered complete.")]
        [SerializeField] private float _releaseThreshold = 0.03f;

        private enum State { Idle, Following, Releasing }

        private Camera _cam;
        private CameraController _controller;
        private Transform _followTarget;
        private GameObject _staticTarget; // auto-created for FocusOnPosition
        private Vector3 _returnPosition;
        private bool _hasReturnPosition;
        private State _state = State.Idle;

        public bool IsFocusing => _state != State.Idle;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _controller = GetComponent<CameraController>();
        }

        /// <summary>Focus on a fixed world position (no follow — camera just pans there).</summary>
        public void FocusOnPosition(Vector3 worldPos)
        {
            if (_staticTarget == null)
            {
                _staticTarget = new GameObject("[CameraFocusTarget]");
                _staticTarget.hideFlags = HideFlags.HideAndDontSave;
            }
            _staticTarget.transform.position = worldPos;
            Focus(_staticTarget.transform);
        }

        /// <summary>Start or retarget a smooth follow on the given transform.</summary>
        public void Focus(Transform target)
        {
            if (target == null) return;

            // Remember the "free-cam" position on first focus so Release() can return to it.
            if (_state == State.Idle)
            {
                _returnPosition = transform.position;
                _hasReturnPosition = true;
                if (_controller != null) _controller.PanLocked = true;
            }

            _followTarget = target;
            _state = State.Following;
        }

        /// <summary>Smoothly return the camera to its pre-focus position, then unlock user pan.</summary>
        public void Release()
        {
            if (_state == State.Idle) return;

            if (!_hasReturnPosition)
            {
                EnterIdle();
                return;
            }

            _state = State.Releasing;
        }

        private void LateUpdate()
        {
            if (_state == State.Idle) return;

            Vector3 targetPos;

            if (_state == State.Following)
            {
                if (_followTarget == null)
                {
                    // Target was destroyed — slide back cleanly.
                    Release();
                    return;
                }
                targetPos = new Vector3(
                    _followTarget.position.x,
                    _followTarget.position.y,
                    transform.position.z);
            }
            else // Releasing
            {
                targetPos = new Vector3(
                    _returnPosition.x,
                    _returnPosition.y,
                    transform.position.z);
            }

            // Frame-rate independent exponential smoothing.
            float t = 1f - Mathf.Exp(-_followDamping * Time.unscaledDeltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPos, t);

            if (_state == State.Releasing &&
                (transform.position - targetPos).sqrMagnitude < _releaseThreshold * _releaseThreshold)
            {
                transform.position = targetPos;
                EnterIdle();
            }
        }

        private void EnterIdle()
        {
            _state = State.Idle;
            _followTarget = null;
            _hasReturnPosition = false;
            if (_controller != null) _controller.PanLocked = false;
        }
    }
}
