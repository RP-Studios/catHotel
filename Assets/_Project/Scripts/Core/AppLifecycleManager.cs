using UnityEngine;
using DG.Tweening;

namespace CatHotel.Core
{
    /// <summary>
    /// Controls target frame rate, accelerometer, and pause/resume behavior.
    /// Attach to a persistent GameObject (e.g. via ProtoSceneSetup or Boot scene).
    /// </summary>
    public class AppLifecycleManager : MonoBehaviour
    {
        [Header("Frame Rate")]
        [SerializeField] private int _activeFrameRate = 30;
        [SerializeField] private int _idleFrameRate = 20;
        [SerializeField] private float _idleTimeout = 5f;

        private float _lastInputTime;

        private void Awake()
        {
            // Target 30 FPS — halves GPU/CPU work vs 60 FPS, massive battery savings
            Application.targetFrameRate = _activeFrameRate;

            // Disable screen dimming
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void Update()
        {
            // Battery saver overrides our fps targets when active
            bool battery = BatterySaverController.Instance != null
                        && BatterySaverController.Instance.IsActive;
            int activeFps = battery ? BatterySaverController.Instance.BatteryFps : _activeFrameRate;
            int idleFps = battery ? BatterySaverController.Instance.BatteryIdleFps : _idleFrameRate;

            // Detect any input to switch back to active framerate
            if (UnityEngine.InputSystem.Pointer.current != null &&
                UnityEngine.InputSystem.Pointer.current.press.isPressed)
            {
                _lastInputTime = Time.unscaledTime;

                if (Application.targetFrameRate != activeFps)
                    Application.targetFrameRate = activeFps;
            }
            else if (Application.targetFrameRate != idleFps &&
                     Time.unscaledTime - _lastInputTime > _idleTimeout)
            {
                // No input for a while — drop to idle framerate to save battery
                Application.targetFrameRate = idleFps;
            }
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                // Pause all DOTween animations
                DOTween.PauseAll();
            }
            else
            {
                // Resume DOTween
                DOTween.PlayAll();

                // Reset idle timer on resume
                _lastInputTime = Time.unscaledTime;
                Application.targetFrameRate = _activeFrameRate;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                _lastInputTime = Time.unscaledTime;
                Application.targetFrameRate = _activeFrameRate;
            }
        }
    }
}
