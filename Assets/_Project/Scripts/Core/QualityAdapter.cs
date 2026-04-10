using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CatHotel.Core
{
    /// <summary>
    /// Adapts rendering quality at startup based on device capabilities.
    /// Runs once in Boot scene. Detects device tier and applies matching quality level.
    ///
    /// Tiers:
    ///   Low-end  (RAM < 3 GB or VRAM < 1 GB)  → Very Low (index 0)
    ///   Mid-range (RAM < 6 GB)                 → Low      (index 1)
    ///   High-end  (RAM >= 6 GB)                → Medium   (index 2)
    ///
    /// Default Android/iPhone quality is set to Very Low (index 0) in QualitySettings
    /// so that even BEFORE this script runs, the first scene loads with minimal settings.
    /// This script then upgrades quality if the device can handle it.
    /// </summary>
    public class QualityAdapter : MonoBehaviour
    {
        [Header("Thresholds")]
        [Tooltip("System memory (MB) below which device is considered low-end")]
        [SerializeField] private int _lowEndMemoryMB = 3000;
        [Tooltip("System memory (MB) above which device is considered high-end")]
        [SerializeField] private int _highEndMemoryMB = 6000;
        [Tooltip("GPU memory estimate threshold (low-end if below)")]
        [SerializeField] private int _lowEndGpuMemoryMB = 1024;

        [Header("Low-End Settings")]
        [SerializeField] private int _lowEndTargetFPS = 30;

        [Header("Mid-Range Settings")]
        [SerializeField] private int _midTargetFPS = 30;

        [Header("High-End Settings")]
        [SerializeField] private int _highEndTargetFPS = 60;

        public enum DeviceTier { LowEnd, MidRange, HighEnd }
        public static DeviceTier Tier { get; private set; }
        public static bool IsLowEnd => Tier == DeviceTier.LowEnd;

        private void Awake()
        {
            DetectAndApply();
        }

        private void DetectAndApply()
        {
            int systemMemory = SystemInfo.systemMemorySize;
            int gpuMemory = SystemInfo.graphicsMemorySize;
            string gpu = SystemInfo.graphicsDeviceName;
            int cpuCount = SystemInfo.processorCount;

            // Classify device
            if (systemMemory < _lowEndMemoryMB || gpuMemory < _lowEndGpuMemoryMB)
                Tier = DeviceTier.LowEnd;
            else if (systemMemory >= _highEndMemoryMB)
                Tier = DeviceTier.HighEnd;
            else
                Tier = DeviceTier.MidRange;

            Debug.Log($"[QualityAdapter] RAM={systemMemory}MB, VRAM={gpuMemory}MB, GPU={gpu}, " +
                      $"Cores={cpuCount}, Tier={Tier}");

            switch (Tier)
            {
                case DeviceTier.LowEnd:
                    ApplyQuality(0, _lowEndTargetFPS);
                    break;
                case DeviceTier.MidRange:
                    ApplyQuality(1, _midTargetFPS);
                    break;
                case DeviceTier.HighEnd:
                    ApplyQuality(2, _highEndTargetFPS);
                    break;
            }
        }

        private void ApplyQuality(int qualityIndex, int targetFPS)
        {
            qualityIndex = Mathf.Clamp(qualityIndex, 0, QualitySettings.count - 1);

            QualitySettings.SetQualityLevel(qualityIndex, applyExpensiveChanges: true);
            Application.targetFrameRate = targetFPS;
            QualitySettings.vSyncCount = 0;

            // Always render at native resolution — 2D sprites look blurry at lower scales
            var pipeline = UniversalRenderPipeline.asset;
            if (pipeline != null)
                pipeline.renderScale = 1f;

            Debug.Log($"[QualityAdapter] Applied: quality={QualitySettings.names[qualityIndex]}, fps={targetFPS}");
        }
    }
}
