using System;
using UnityEngine;
using Unity.Services.LevelPlay;

namespace CatHotel.Services
{
    public class AdManager : MonoBehaviour
    {
        public static AdManager Instance { get; private set; }

        [SerializeField] private AdConfig _config;

        private LevelPlayRewardedAd _rewardedAd;
        private bool _sdkReady;

        // Daily tracking
        private int _adsWatchedToday;
        private string _lastResetDate;
        private const string PrefKey = "AdDailyCount";
        private const string PrefDateKey = "AdDailyDate";

        public bool IsAdReady => _sdkReady && _rewardedAd != null && _rewardedAd.IsAdReady();
        public bool HasReachedDailyCap => _adsWatchedToday >= _config.dailyCap;
        public int AdsWatchedToday => _adsWatchedToday;
        public int DailyCap => _config != null ? _config.dailyCap : 10;

        public event Action OnAdCompleted;
        public event Action OnAdFailed;
        public event Action OnAdAvailabilityChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadDailyCount();
        }

        public void InitializeAds()
        {
            if (_config == null)
            {
                Debug.LogError("[Ads] AdConfig is missing!");
                return;
            }

            if (_config.testMode)
            {
                LevelPlay.SetMetaData("is_test_suite", "enable");
                Debug.Log("[Ads] Test mode enabled");
            }

            LevelPlay.OnInitSuccess += OnSdkInitSuccess;
            LevelPlay.OnInitFailed += OnSdkInitFailed;

            Debug.Log($"[Ads] Initializing LevelPlay with appKey: {_config.appKey}");
            LevelPlay.Init(_config.appKey);
        }

        private void OnSdkInitSuccess(LevelPlayConfiguration config)
        {
            Debug.Log("[Ads] LevelPlay SDK initialized");
            _sdkReady = true;
            CreateRewardedAd();
        }

        private void OnSdkInitFailed(LevelPlayInitError error)
        {
            Debug.LogWarning($"[Ads] LevelPlay init failed: {error.ErrorMessage}");
        }

        private void CreateRewardedAd()
        {
            _rewardedAd = new LevelPlayRewardedAd(_config.rewardedAdUnitId);

            _rewardedAd.OnAdLoaded += info =>
            {
                Debug.Log("[Ads] Rewarded ad loaded");
                OnAdAvailabilityChanged?.Invoke();
            };

            _rewardedAd.OnAdLoadFailed += error =>
            {
                Debug.LogWarning($"[Ads] Rewarded ad load failed: {error.ErrorMessage}");
                OnAdAvailabilityChanged?.Invoke();
            };

            _rewardedAd.OnAdRewarded += (info, reward) =>
            {
                Debug.Log($"[Ads] Reward granted: {reward.Name} x{reward.Amount}");
                _adsWatchedToday++;
                SaveDailyCount();
                OnAdCompleted?.Invoke();
            };

            _rewardedAd.OnAdDisplayFailed += (info, error) =>
            {
                Debug.LogWarning($"[Ads] Rewarded ad display failed: {error.ErrorMessage}");
                OnAdFailed?.Invoke();
            };

            _rewardedAd.OnAdClosed += info =>
            {
                Debug.Log("[Ads] Rewarded ad closed — preloading next");
                LoadRewardedAd();
            };

            LoadRewardedAd();
        }

        private void LoadRewardedAd()
        {
            if (!_sdkReady || _rewardedAd == null) return;
            if (HasReachedDailyCap) return;

            _rewardedAd.LoadAd();
        }

        public bool ShowRewardedAd()
        {
            if (!IsAdReady || HasReachedDailyCap)
            {
                Debug.LogWarning("[Ads] Cannot show ad — not ready or daily cap reached");
                OnAdFailed?.Invoke();
                return false;
            }

            _rewardedAd.ShowAd("BoostX2");
            return true;
        }

        private void LoadDailyCount()
        {
            _lastResetDate = PlayerPrefs.GetString(PrefDateKey, "");
            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");

            if (_lastResetDate != today)
            {
                _adsWatchedToday = 0;
                _lastResetDate = today;
                SaveDailyCount();
            }
            else
            {
                _adsWatchedToday = PlayerPrefs.GetInt(PrefKey, 0);
            }
        }

        private void SaveDailyCount()
        {
            PlayerPrefs.SetInt(PrefKey, _adsWatchedToday);
            PlayerPrefs.SetString(PrefDateKey, _lastResetDate);
            PlayerPrefs.Save();
            OnAdAvailabilityChanged?.Invoke();
        }

        private void OnDestroy()
        {
            _rewardedAd?.DestroyAd();
            LevelPlay.OnInitSuccess -= OnSdkInitSuccess;
            LevelPlay.OnInitFailed -= OnSdkInitFailed;
        }
    }
}
