using UnityEngine;
using CatHotel.UI;

namespace CatHotel.Core
{
    /// <summary>
    /// Persistent singleton that applies battery-saving effects when
    /// ParametersPanel.BatterySaving is ON.
    ///
    /// Level 1 — Frame rate: caps to BatteryFps (default 25) instead of the
    /// tier-based fps from QualityAdapter. Also reduces idle fps further.
    ///
    /// Level 2 — Passive animations: scales all Animator.speed across the
    /// project (cats, floating coins, etc.) by AnimSpeedScale (default 0.5).
    ///
    /// Plugs in via the static event ParametersPanel.OnBatterySavingChanged.
    /// </summary>
    public class BatterySaverController : MonoBehaviour
    {
        public static BatterySaverController Instance { get; private set; }

        [Header("Frame Rate")]
        [Tooltip("Target fps when battery saving is ON")]
        [SerializeField] private int _batteryFps = 25;
        [Tooltip("Target fps when battery saving is ON and the player is idle")]
        [SerializeField] private int _batteryIdleFps = 15;

        [Header("Animations")]
        [Tooltip("Multiplier applied to Animator.speed on all active animators")]
        [SerializeField, Range(0.1f, 1f)] private float _animSpeedScale = 0.5f;

        private bool _isActive;
        private int _normalFps;
        private int _normalIdleFps;

        public int BatteryFps => _batteryFps;
        public int BatteryIdleFps => _batteryIdleFps;
        public float AnimSpeedScale => _animSpeedScale;
        public bool IsActive => _isActive;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Remember tier-based values from QualityAdapter / AppLifecycleManager
            _normalFps = Application.targetFrameRate > 0 ? Application.targetFrameRate : 30;
            _normalIdleFps = 20;
        }

        private void OnEnable()
        {
            ParametersPanel.OnBatterySavingChanged += Apply;
            // Apply current state in case the event fired before we subscribed
            Apply(ParametersPanel.BatterySaving);
        }

        private void OnDisable()
        {
            ParametersPanel.OnBatterySavingChanged -= Apply;
        }

        private void Apply(bool on)
        {
            if (on == _isActive) return;
            _isActive = on;

            if (on)
            {
                // Cache current "normal" values right before overriding them
                if (Application.targetFrameRate > 0)
                    _normalFps = Application.targetFrameRate;

                Application.targetFrameRate = _batteryFps;
                QualitySettings.vSyncCount = 0;
                ScaleAllAnimators(_animSpeedScale);
                Debug.Log($"[BatterySaver] ON — fps={_batteryFps}, idleFps={_batteryIdleFps}, " +
                          $"animScale={_animSpeedScale}");
            }
            else
            {
                Application.targetFrameRate = _normalFps;
                ScaleAllAnimators(1f / _animSpeedScale);
                Debug.Log($"[BatterySaver] OFF — restored fps={_normalFps}");
            }
        }

        /// <summary>
        /// Scale every active Animator in the scene. Multiplies their .speed by the factor.
        /// Used to slow down idle cat animations and floating coin spins.
        /// </summary>
        private static void ScaleAllAnimators(float factor)
        {
            var animators = FindObjectsByType<Animator>(FindObjectsSortMode.None);
            for (int i = 0; i < animators.Length; i++)
            {
                animators[i].speed = Mathf.Max(0.05f, animators[i].speed * factor);
            }
        }

        /// <summary>
        /// Called by newly spawned animators (e.g. cats spawned after toggle ON)
        /// to inherit the current scale factor.
        /// </summary>
        public void ApplyToNewAnimator(Animator animator)
        {
            if (animator != null && _isActive)
                animator.speed *= _animSpeedScale;
        }
    }
}
