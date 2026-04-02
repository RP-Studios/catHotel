using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CatHotel.Core
{
    /// <summary>
    /// Adapts rendering quality at startup based on device capabilities.
    /// Runs once in Boot scene. Detects low-end devices and reduces settings.
    /// </summary>
    public class QualityAdapter : MonoBehaviour
    {
        [Header("Thresholds")]
        [Tooltip("System memory (MB) below which device is considered low-end")]
        [SerializeField] private int _lowEndMemoryMB = 3000;
        [Tooltip("GPU memory estimate threshold (low-end if no dedicated GPU info)")]
        [SerializeField] private int _lowEndGpuMemoryMB = 1024;

        [Header("Low-End Settings")]
        [SerializeField] private float _lowEndRenderScale = 0.75f;
        [SerializeField] private int _lowEndTargetFPS = 30;
        [SerializeField] private int _normalTargetFPS = 60;

        public static bool IsLowEnd { get; private set; }

        private void Awake()
        {
            DetectAndApply();
        }

        private void DetectAndApply()
        {
            int systemMemory = SystemInfo.systemMemorySize;
            int gpuMemory = SystemInfo.graphicsMemorySize;
            string gpu = SystemInfo.graphicsDeviceName;

            IsLowEnd = systemMemory < _lowEndMemoryMB
                     || gpuMemory < _lowEndGpuMemoryMB;

            Debug.Log($"[QualityAdapter] RAM={systemMemory}MB, VRAM={gpuMemory}MB, GPU={gpu}, LowEnd={IsLowEnd}");

            if (IsLowEnd)
                ApplyLowEnd();
            else
                ApplyNormal();
        }

        private void ApplyLowEnd()
        {
            Application.targetFrameRate = _lowEndTargetFPS;

            // Reduce render scale on URP
            var pipeline = UniversalRenderPipeline.asset;
            if (pipeline != null)
                pipeline.renderScale = _lowEndRenderScale;

            // Lower quality level
            QualitySettings.SetQualityLevel(0, true);

            // Disable VSync (let targetFrameRate control)
            QualitySettings.vSyncCount = 0;

            Debug.Log($"[QualityAdapter] Low-end mode: {_lowEndTargetFPS}fps, renderScale={_lowEndRenderScale}");
        }

        private void ApplyNormal()
        {
            Application.targetFrameRate = _normalTargetFPS;
            QualitySettings.vSyncCount = 0;

            Debug.Log($"[QualityAdapter] Normal mode: {_normalTargetFPS}fps");
        }
    }
}
