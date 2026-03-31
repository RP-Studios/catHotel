using System.Collections;
using UnityEngine;
using CatHotel.Services;
using CatHotel.UI;

namespace CatHotel.Boot
{
    /// <summary>
    /// Entry point of the game. Initializes services (Auth, Ads) then hands off
    /// to the main menu. Services use DontDestroyOnLoad so they persist across scenes.
    /// </summary>
    public class BootManager : MonoBehaviour
    {
        [SerializeField] private float _authTimeout = 5f;

        public bool IsReady { get; private set; }

        private IEnumerator Start()
        {
            // Reduce async loading impact on main thread → smoother animations during load
            Application.backgroundLoadingPriority = ThreadPriority.Low;

            // Create persistent loading screen (once, survives scene changes)
            if (LoadingScreen.Instance == null)
            {
                var lsGo = new GameObject("[LoadingScreen]");
                lsGo.AddComponent<LoadingScreen>();
            }

            // Warmup shaders to avoid first-frame compilation stutter
            yield return WarmupShaders();

            // Initialize Auth (non-blocking, with timeout)
            if (AuthManager.Instance != null)
            {
                var authTask = AuthManager.Instance.InitializeAsync();
                float timeout = _authTimeout;
                while (!authTask.IsCompleted && timeout > 0f)
                {
                    timeout -= Time.unscaledDeltaTime;
                    yield return null;
                }
                if (!authTask.IsCompleted)
                    Debug.LogWarning("[Boot] Auth timeout — continuing offline");
            }

            // Initialize Ads (non-blocking)
            var adManager = AdManager.Instance ?? FindAnyObjectByType<AdManager>();
            if (adManager != null)
                adManager.InitializeAds();

            IsReady = true;
            Debug.Log("[Boot] Services initialized. Main menu ready.");
        }

        private IEnumerator WarmupShaders()
        {
            // Force Unity to precompile all shader variants used in the project
            Shader.WarmupAllShaders();
            yield return null; // let the frame flush
        }
    }
}
