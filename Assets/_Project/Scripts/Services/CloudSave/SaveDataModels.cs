using System;
using System.Collections.Generic;

namespace CatHotel.Services
{
    /// <summary>
    /// Serializable data models for cloud save.
    /// Each model maps to a separate cloud save key.
    /// </summary>

    [Serializable]
    public class SettingsSaveData
    {
        public string language;          // "fr" or "en"
        public float musicVolume;        // 0-1
        public float effectsVolume;      // 0-1
        public bool pushNotifications;
        public bool batterySaving;
        public int saveVersion = 1;
    }

    [Serializable]
    public class ProgressionSaveData
    {
        public int reputationLevel;
        public int reputationXp;
        public List<PlacedObjectSaveData> placedObjects = new();
        public List<CatCloudSaveData> cats = new();
        public string lastSaveTime;      // ISO 8601
        public int saveVersion = 1;
    }

    [Serializable]
    public class PlacedObjectSaveData
    {
        public string objectAssetName;   // ScriptableObject.name (e.g. "Obj_Bed")
        public int gridX;
        public int gridY;
    }

    [Serializable]
    public class CatCloudSaveData
    {
        public string breedName;
        public string catName;
        public string mode;              // "Pension" or "Refuge"
        public bool isSpecial;

        // Needs (5 floats: Hunger, Thirst, Sleep, Play, Clean)
        public float[] needs;
        public float happiness;

        // Pension tracking
        public float pensionDuration;
        public float pensionTimeRemaining;

        // Happiness tracking
        public float happinessSum;
        public int happinessSamples;
        public float happyDuration;
    }
}
