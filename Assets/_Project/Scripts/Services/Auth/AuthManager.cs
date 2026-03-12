using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;
#if UNITY_ANDROID && !UNITY_EDITOR
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

namespace CatHotel.Services
{
    public class AuthManager : MonoBehaviour
    {
        public static AuthManager Instance { get; private set; }

        [SerializeField] private AuthConfig _config;

        public bool IsSignedIn { get; private set; }
        public bool IsGooglePlayLinked { get; private set; }
        public string PlayerId { get; private set; }

        public event Action OnSignInComplete;
        public event Action<string> OnSignInFailed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async Task InitializeAsync()
        {
            try
            {
                var options = new InitializationOptions();
                options.SetEnvironmentName(_config.environmentName);
                await UnityServices.InitializeAsync(options);

                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                PlayerId = AuthenticationService.Instance.PlayerId;
                IsSignedIn = true;
                Debug.Log($"[Auth] UGS anonymous sign-in OK. PlayerId: {PlayerId}");

                await TryLinkGooglePlayAsync();

                OnSignInComplete?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Auth] Sign-in failed: {e.Message}");
                OnSignInFailed?.Invoke(e.Message);
            }
        }

        private async Task TryLinkGooglePlayAsync()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                PlayGamesPlatform.Activate();

                string authCode = await RequestServerAuthCodeAsync();
                if (string.IsNullOrEmpty(authCode))
                {
                    Debug.LogWarning("[Auth] GPGS: no auth code obtained");
                    return;
                }

                try
                {
                    await AuthenticationService.Instance
                        .LinkWithGooglePlayGamesAsync(authCode);
                    IsGooglePlayLinked = true;
                    Debug.Log("[Auth] Google Play linked successfully");
                }
                catch (AuthenticationException e) when (e.ErrorCode == 10002)
                {
                    await AuthenticationService.Instance
                        .SignInWithGooglePlayGamesAsync(authCode);
                    IsGooglePlayLinked = true;
                    PlayerId = AuthenticationService.Instance.PlayerId;
                    Debug.Log("[Auth] Google Play sign-in (already linked)");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Auth] GPGS link failed: {e.Message}");
            }
#else
            await Task.CompletedTask;
            Debug.Log("[Auth] GPGS skipped (not Android)");
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private Task<string> RequestServerAuthCodeAsync()
        {
            var tcs = new TaskCompletionSource<string>();
            PlayGamesPlatform.Instance.RequestServerSideAccess(
                forceRefreshToken: false,
                code => tcs.SetResult(code)
            );
            return tcs.Task;
        }
#endif
    }
}
