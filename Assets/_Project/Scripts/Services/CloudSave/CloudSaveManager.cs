using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CatHotel.Services
{
    /// <summary>
    /// Central orchestrator for save/load. Cloud-first with local fallback.
    /// Always writes to local cache, then attempts cloud sync.
    /// At boot, loads from cloud if available, otherwise from local cache.
    /// </summary>
    public class CloudSaveManager : MonoBehaviour
    {
        public static CloudSaveManager Instance { get; private set; }

        private const string SettingsKey = "settings";
        private const string ProgressionKey = "progression";
        private const float SyncRetryInterval = 60f;

        /// <summary>Current settings data (in-memory cache).</summary>
        public SettingsSaveData Settings { get; private set; }

        /// <summary>Current progression data (in-memory cache).</summary>
        public ProgressionSaveData Progression { get; set; }

        /// <summary>True if cloud save is reachable and auth is signed in.</summary>
        public bool IsCloudAvailable { get; private set; }

        /// <summary>True if local changes haven't been synced to cloud yet.</summary>
        public bool HasPendingSync { get; private set; }

        /// <summary>True once initial load is complete (cloud or local).</summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// True if a persisted save (local file or cloud key) was found during load.
        /// This is the authoritative signal for "is this a returning player".
        /// </summary>
        public bool HasPersistedSave { get; private set; }

        private bool _settingsDirty;
        private bool _progressionDirty;
        private float _syncRetryTimer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Settings = new SettingsSaveData();
            Progression = new ProgressionSaveData();
        }

        private void Update()
        {
            if (!HasPendingSync) return;

            _syncRetryTimer += Time.unscaledDeltaTime;
            if (_syncRetryTimer < SyncRetryInterval) return;
            _syncRetryTimer = 0f;

            _ = TrySyncPendingAsync();
        }

        // ========== LOAD ==========

        /// <summary>
        /// Load all data. Tries cloud first, falls back to local.
        /// Called by BootManager after auth.
        /// </summary>
        public async Task LoadAllAsync(bool isSignedIn)
        {
            if (isSignedIn)
            {
                try
                {
                    await LoadFromCloudAsync();
                    IsCloudAvailable = true;

                    // Sync any pending local changes from a previous offline session
                    var pending = LocalSaveProvider.LoadPendingSync();
                    if (pending.settingsDirty || pending.progressionDirty)
                    {
                        Debug.Log("[CloudSaveManager] Found pending sync from previous session");
                        await ResolvePendingSyncAsync(pending);
                    }

                    Debug.Log("[CloudSaveManager] Loaded from cloud");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[CloudSaveManager] Cloud load failed: {e.Message}");
                    IsCloudAvailable = false;
                    LoadFromLocal();
                }
            }
            else
            {
                IsCloudAvailable = false;
                LoadFromLocal();
            }

            IsLoaded = true;
        }

        /// <summary>Load all data from local cache only (offline mode).</summary>
        public void LoadFromLocal()
        {
            var localSettings = LocalSaveProvider.LoadSettings();
            var localProgression = LocalSaveProvider.LoadProgression();

            Settings = localSettings ?? new SettingsSaveData();
            Progression = localProgression ?? new ProgressionSaveData();

            // If either local save file existed, it's a returning player
            if (localProgression != null) HasPersistedSave = true;

            var pending = LocalSaveProvider.LoadPendingSync();
            HasPendingSync = pending.settingsDirty || pending.progressionDirty;
            _settingsDirty = pending.settingsDirty;
            _progressionDirty = pending.progressionDirty;

            Debug.Log($"[CloudSaveManager] Loaded from local cache (hasSave={HasPersistedSave})");
        }

        private async Task LoadFromCloudAsync()
        {
            var cloudSettings = await CloudSaveProvider.LoadAsync<SettingsSaveData>(SettingsKey);
            var cloudProgression = await CloudSaveProvider.LoadAsync<ProgressionSaveData>(ProgressionKey);

            Settings = cloudSettings ?? new SettingsSaveData();
            Progression = cloudProgression ?? new ProgressionSaveData();

            // If cloud returned a progression key, it's a returning player
            if (cloudProgression != null) HasPersistedSave = true;

            // Fallback check: if cloud has nothing but local does, still a returning player
            if (!HasPersistedSave && LocalSaveProvider.LoadProgression() != null)
                HasPersistedSave = true;

            // Update local cache with cloud data
            LocalSaveProvider.SaveSettings(Settings);
            LocalSaveProvider.SaveProgression(Progression);
            LocalSaveProvider.ClearPendingSync();
        }

        /// <summary>
        /// When cloud is back online after offline play, resolve conflicts.
        /// Strategy: local data wins (player's most recent changes).
        /// </summary>
        private async Task ResolvePendingSyncAsync(PendingSyncState pending)
        {
            // Load local data (most recent player changes)
            var localSettings = LocalSaveProvider.LoadSettings();
            var localProgression = LocalSaveProvider.LoadProgression();

            if (pending.settingsDirty && localSettings != null)
            {
                Settings = localSettings;
                await CloudSaveProvider.SaveAsync(SettingsKey, Settings);
            }

            if (pending.progressionDirty && localProgression != null)
            {
                Progression = localProgression;
                await CloudSaveProvider.SaveAsync(ProgressionKey, Progression);
            }

            LocalSaveProvider.ClearPendingSync();
            HasPendingSync = false;
            _settingsDirty = false;
            _progressionDirty = false;

            Debug.Log("[CloudSaveManager] Pending sync resolved");
        }

        // ========== SAVE ==========

        /// <summary>
        /// Save settings. Async local I/O + cloud attempt.
        /// Non-blocking for the main thread.
        /// </summary>
        public async void SaveSettings()
        {
            // Async local write (serialization on main thread, I/O on background)
            await LocalSaveProvider.SaveSettingsAsync(Settings);

            if (IsCloudAvailable)
            {
                try
                {
                    await CloudSaveProvider.SaveAsync(SettingsKey, Settings);
                    _settingsDirty = false;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[CloudSaveManager] Cloud save settings failed: {e.Message}");
                    MarkPendingSync(settingsDirty: true);
                }
            }
            else
            {
                MarkPendingSync(settingsDirty: true);
            }
        }

        /// <summary>
        /// Save progression. Async local I/O + cloud attempt.
        /// Non-blocking for the main thread.
        /// </summary>
        public async void SaveProgression()
        {
            Progression.lastSaveTime = DateTime.UtcNow.ToString("o");
            HasPersistedSave = true;

            // Async local write (serialization on main thread, I/O on background)
            await LocalSaveProvider.SaveProgressionAsync(Progression);

            if (IsCloudAvailable)
            {
                try
                {
                    await CloudSaveProvider.SaveAsync(ProgressionKey, Progression);
                    _progressionDirty = false;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[CloudSaveManager] Cloud save progression failed: {e.Message}");
                    MarkPendingSync(progressionDirty: true);
                }
            }
            else
            {
                MarkPendingSync(progressionDirty: true);
            }
        }

        /// <summary>
        /// Synchronous save for OnApplicationPause/Quit where we MUST finish
        /// before the OS kills the process.
        /// </summary>
        public void SaveProgressionImmediate()
        {
            Progression.lastSaveTime = DateTime.UtcNow.ToString("o");
            HasPersistedSave = true;
            LocalSaveProvider.SaveProgression(Progression);
        }

        /// <summary>
        /// Reset all progression data (new game). Keeps settings intact.
        /// Clears local + cloud.
        /// </summary>
        public async void ResetAllData()
        {
            Progression = new ProgressionSaveData();
            LocalSaveProvider.DeleteProgression();
            LocalSaveProvider.ClearPendingSync();
            HasPendingSync = false;
            _progressionDirty = false;
            // No persisted save anymore — HotelManager will initialize with GameConfig defaults.
            // First real SaveProgression (auto-save, event) will re-create the file.
            HasPersistedSave = false;

            if (IsCloudAvailable)
            {
                try
                {
                    await CloudSaveProvider.DeleteAsync(ProgressionKey);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[CloudSaveManager] Cloud reset failed: {e.Message}");
                }
            }

            Debug.Log("[CloudSaveManager] Progression reset (new game)");
        }

        private void MarkPendingSync(bool settingsDirty = false, bool progressionDirty = false)
        {
            if (settingsDirty) _settingsDirty = true;
            if (progressionDirty) _progressionDirty = true;
            HasPendingSync = _settingsDirty || _progressionDirty;
            LocalSaveProvider.SetPendingSync(_settingsDirty, _progressionDirty);
        }

        /// <summary>Retry syncing pending local changes to cloud.</summary>
        private async Task TrySyncPendingAsync()
        {
            if (!HasPendingSync) return;

            try
            {
                if (_settingsDirty)
                {
                    await CloudSaveProvider.SaveAsync(SettingsKey, Settings);
                    _settingsDirty = false;
                }

                if (_progressionDirty)
                {
                    await CloudSaveProvider.SaveAsync(ProgressionKey, Progression);
                    _progressionDirty = false;
                }

                HasPendingSync = false;
                IsCloudAvailable = true;
                LocalSaveProvider.ClearPendingSync();
                Debug.Log("[CloudSaveManager] Pending sync completed");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[CloudSaveManager] Sync retry failed: {e.Message}");
            }
        }
    }
}
