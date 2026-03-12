using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using CatHotel.Cats;
using CatHotel.Hotel;

namespace CatHotel.Core
{
    /// <summary>
    /// Local JSON save/load. Stores economy, reputation, and cat states.
    /// Will be replaced by Cloud Save later.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        private const string FileName = "cathotel_save.json";

        private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

        public static void Save(HotelManager hotel)
        {
            var data = new SaveData
            {
                coins = hotel.Economy.Coins,
                gems = hotel.Economy.Gems,
                reputationLevel = hotel.Reputation.Level,
                lastSaveTime = DateTime.UtcNow.ToString("o"),
                cats = new List<CatSaveData>()
            };

            foreach (var cat in hotel.Cats)
            {
                data.cats.Add(new CatSaveData
                {
                    breedName = cat.Breed.breedName,
                    catName = cat.CatName,
                    mode = cat.Mode.ToString(),
                    isSpecial = cat.IsSpecial,
                    needs = cat.Needs.ToArray(),
                    happiness = cat.Happiness.Value,
                    pensionDuration = cat.PensionDuration,
                    pensionTimeRemaining = cat.PensionTimeRemaining
                });
            }

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
            Debug.Log($"[Save] Saved to {FilePath}");
        }

        public static SaveData Load()
        {
            if (!File.Exists(FilePath))
            {
                Debug.Log("[Save] No save file found, starting fresh.");
                return null;
            }

            string json = File.ReadAllText(FilePath);
            var data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"[Save] Loaded: {data.coins}$, {data.cats.Count} cats, rep {data.reputationLevel}");
            return data;
        }

        public static void Delete()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
                Debug.Log("[Save] Save file deleted.");
            }
        }
    }

    [Serializable]
    public class SaveData
    {
        public int coins;
        public int gems;
        public int reputationLevel;
        public string lastSaveTime;
        public List<CatSaveData> cats;
    }

    [Serializable]
    public class CatSaveData
    {
        public string breedName;
        public string catName;
        public string mode;
        public bool isSpecial;
        public float[] needs;
        public float happiness;
        public float pensionDuration;
        public float pensionTimeRemaining;
    }
}
