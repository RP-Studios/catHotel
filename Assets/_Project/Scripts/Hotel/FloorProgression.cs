using System;
using UnityEngine;
using CatHotel.Economy;
using CatHotel.Services;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Runtime authority for floor unlocking.
    /// Reads FloorUnlockConfig, validates rep + coins, persists progress, exposes capacity.
    /// All UI flows through TryUnlock(int).
    /// </summary>
    public class FloorProgression : MonoBehaviour
    {
        [SerializeField] private FloorUnlockConfig _config;
        [SerializeField] private EconomyManager _economy;
        [SerializeField] private ReputationManager _reputation;

        public static FloorProgression Instance { get; private set; }

        private int _highestUnlockedFloor; // 0 = only RDC

        public int HighestUnlockedFloor => _highestUnlockedFloor;
        public FloorUnlockConfig Config => _config;

        /// <summary>Failure reasons for TryUnlock.</summary>
        public enum UnlockResult
        {
            Success,
            AlreadyUnlocked,
            UnknownFloor,
            NotNextFloor,
            ReputationTooLow,
            NotEnoughCoins,
            ConfigMissing
        }

        /// <summary>Fired when a floor is successfully unlocked. Argument = newly unlocked floor index.</summary>
        public event Action<int> OnFloorUnlocked;

        private void Awake()
        {
            Instance = this;
            if (_economy == null) _economy = FindAnyObjectByType<EconomyManager>();
            if (_reputation == null) _reputation = FindAnyObjectByType<ReputationManager>();
        }

        private void Start()
        {
            // Floor unlock is now driven by reputation level: 1 rep level = 1 floor unlocked.
            // We listen to OnLevelChanged and sync HighestUnlockedFloor accordingly.
            if (_reputation != null)
            {
                _highestUnlockedFloor = _reputation.Level;
                _reputation.OnLevelChanged += OnRepLevelChanged;
            }
        }

        private void OnDestroy()
        {
            if (_reputation != null) _reputation.OnLevelChanged -= OnRepLevelChanged;
        }

        private void OnRepLevelChanged(int newLevel)
        {
            if (newLevel <= _highestUnlockedFloor) return;
            _highestUnlockedFloor = newLevel;
            Persist();
            OnFloorUnlocked?.Invoke(newLevel);
            Debug.Log($"[FloorProgression] Floor {newLevel} unlocked via rep level-up.");
        }

        // ---- Queries ----

        public bool IsUnlocked(int floorIndex) => floorIndex <= _highestUnlockedFloor;

        public int GetCost(int floorIndex) => _config?.GetEntry(floorIndex)?.catCoinCost ?? 0;

        public int GetRequiredRep(int floorIndex) => _config?.GetEntry(floorIndex)?.requiredReputationLevel ?? 0;

        public int GetCapacityFor(int floorIndex) => _config?.GetEntry(floorIndex)?.totalCapacity ?? 5;

        /// <summary>Total max cats. Reputation-driven; falls back to config or 5.</summary>
        public int MaxCats => _reputation != null ? _reputation.MaxCats : GetCapacityFor(_highestUnlockedFloor);

        /// <summary>Next floor that is NOT yet unlocked. -1 if everything is unlocked.</summary>
        public int NextLockedFloor
        {
            get
            {
                if (_config == null) return -1;
                int next = _highestUnlockedFloor + 1;
                if (_config.GetEntry(next) == null) return -1;
                return next;
            }
        }

        /// <summary>Check whether the next floor can be unlocked right now.</summary>
        public UnlockResult CanUnlock(int floorIndex)
        {
            if (_config == null) return UnlockResult.ConfigMissing;
            if (IsUnlocked(floorIndex)) return UnlockResult.AlreadyUnlocked;
            var entry = _config.GetEntry(floorIndex);
            if (entry == null) return UnlockResult.UnknownFloor;
            if (floorIndex != _highestUnlockedFloor + 1) return UnlockResult.NotNextFloor;
            if (_reputation != null && _reputation.Level < entry.requiredReputationLevel)
                return UnlockResult.ReputationTooLow;
            if (_economy != null && _economy.Coins < entry.catCoinCost)
                return UnlockResult.NotEnoughCoins;
            return UnlockResult.Success;
        }

        /// <summary>Attempt to unlock a floor. Spends coins on success and persists.</summary>
        public UnlockResult TryUnlock(int floorIndex)
        {
            var check = CanUnlock(floorIndex);
            if (check != UnlockResult.Success) return check;

            var entry = _config.GetEntry(floorIndex);
            if (_economy != null && entry.catCoinCost > 0 && !_economy.TrySpend(entry.catCoinCost))
                return UnlockResult.NotEnoughCoins;

            _highestUnlockedFloor = floorIndex;
            Persist();
            OnFloorUnlocked?.Invoke(floorIndex);
            Debug.Log($"[FloorProgression] Unlocked floor {floorIndex} for {entry.catCoinCost} coins.");
            return UnlockResult.Success;
        }

        private void Persist()
        {
            if (CloudSaveManager.Instance == null) return;
            CloudSaveManager.Instance.Progression.highestUnlockedFloor = _highestUnlockedFloor;
            CloudSaveManager.Instance.SaveProgression();
        }

        // ---- Debug helpers ----

#if UNITY_EDITOR
        [ContextMenu("Debug: Unlock Next Floor (free)")]
        private void DebugUnlockNext()
        {
            int next = NextLockedFloor;
            if (next < 0) { Debug.Log("[FloorProgression] All floors unlocked."); return; }
            _highestUnlockedFloor = next;
            Persist();
            OnFloorUnlocked?.Invoke(next);
            Debug.Log($"[FloorProgression] DEBUG forced unlock of floor {next}");
        }
#endif
    }
}
