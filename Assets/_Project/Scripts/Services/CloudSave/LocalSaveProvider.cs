using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace CatHotel.Services
{
    /// <summary>
    /// JSON file-based local save. Acts as cache for cloud data
    /// and as offline fallback when cloud is unavailable.
    /// </summary>
    public static class LocalSaveProvider
    {
        private const string SettingsFile = "save_settings.json";
        private const string ProgressionFile = "save_progression.json";
        private const string PendingSyncFile = "save_pending_sync.json";

        private static string SettingsPath =>
            Path.Combine(Application.persistentDataPath, SettingsFile);

        private static string ProgressionPath =>
            Path.Combine(Application.persistentDataPath, ProgressionFile);

        private static string PendingSyncPath =>
            Path.Combine(Application.persistentDataPath, PendingSyncFile);

        // --- Settings ---

        public static void SaveSettings(SettingsSaveData data)
        {
            WriteJson(SettingsPath, data);
        }

        public static Task SaveSettingsAsync(SettingsSaveData data)
        {
            return WriteJsonAsync(SettingsPath, data);
        }

        public static SettingsSaveData LoadSettings()
        {
            return ReadJson<SettingsSaveData>(SettingsPath);
        }

        // --- Progression ---

        public static void SaveProgression(ProgressionSaveData data)
        {
            WriteJson(ProgressionPath, data);
        }

        public static Task SaveProgressionAsync(ProgressionSaveData data)
        {
            return WriteJsonAsync(ProgressionPath, data);
        }

        public static ProgressionSaveData LoadProgression()
        {
            return ReadJson<ProgressionSaveData>(ProgressionPath);
        }

        public static void DeleteProgression()
        {
            if (File.Exists(ProgressionPath)) File.Delete(ProgressionPath);
        }

        // --- Pending Sync tracking ---

        public static void SetPendingSync(bool settingsDirty, bool progressionDirty)
        {
            var state = new PendingSyncState
            {
                settingsDirty = settingsDirty,
                progressionDirty = progressionDirty
            };
            WriteJson(PendingSyncPath, state);
        }

        public static PendingSyncState LoadPendingSync()
        {
            return ReadJson<PendingSyncState>(PendingSyncPath) ?? new PendingSyncState();
        }

        public static void ClearPendingSync()
        {
            if (File.Exists(PendingSyncPath))
                File.Delete(PendingSyncPath);
        }

        // --- Helpers ---

        /// <summary>Synchronous write (used for Load path and critical saves).</summary>
        private static void WriteJson<T>(string path, T data)
        {
            try
            {
                // JsonUtility must run on main thread
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[LocalSave] Write failed ({path}): {e.Message}");
            }
        }

        /// <summary>Async write — serializes on main thread, I/O on background thread.</summary>
        public static async Task WriteJsonAsync<T>(string path, T data)
        {
            try
            {
                // JsonUtility must run on main thread — only offload I/O
                string json = JsonUtility.ToJson(data, false);
                await Task.Run(() => File.WriteAllText(path, json));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[LocalSave] Async write failed ({path}): {e.Message}");
            }
        }

        private static T ReadJson<T>(string path) where T : class
        {
            if (!File.Exists(path)) return null;
            try
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[LocalSave] Read failed ({path}): {e.Message}");
                return null;
            }
        }
    }

    [Serializable]
    public class PendingSyncState
    {
        public bool settingsDirty;
        public bool progressionDirty;
    }
}
