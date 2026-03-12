using System;
using UnityEngine;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Manages reputation level (0-10). Controls breed unlocks and cat limits.
    /// GDD section 8.
    /// </summary>
    public class ReputationManager : MonoBehaviour
    {
        private static readonly ReputationLevel[] Levels = new[]
        {
            new ReputationLevel(0,  "Débutant",          0,  0,    0,    5),
            new ReputationLevel(1,  "Amateur",           3,  60,   100,  10),
            new ReputationLevel(2,  "Compétent",         5,  65,   200,  15),
            new ReputationLevel(3,  "Professionnel",     7,  70,   350,  20),
            new ReputationLevel(4,  "Expert",            10, 72,   500,  25),
            new ReputationLevel(5,  "Renommé",           12, 75,   750,  30),
            new ReputationLevel(6,  "Célèbre",           14, 77,   1000, 35),
            new ReputationLevel(7,  "Prestigieux",       16, 80,   1500, 40),
            new ReputationLevel(8,  "Élite",             18, 82,   2000, 45),
            new ReputationLevel(9,  "Légendaire",        20, 85,   3000, 50),
            new ReputationLevel(10, "Maître des Chats",  25, 88,   5000, 55),
        };

        private int _level;
        private int _reputationPoints; // accumulated from adoptions and good pension service

        public int Level => _level;
        public string LevelName => Levels[_level].Name;
        public int MaxCats => Levels[_level].MaxCats;
        public ReputationLevel CurrentLevel => Levels[_level];

        public event Action<int> OnLevelChanged;

        public void Init(int level, int points)
        {
            _level = Mathf.Clamp(level, 0, 10);
            _reputationPoints = points;
        }

        /// <summary>Check if a breed with the given min reputation is unlocked.</summary>
        public bool IsBreedUnlocked(int minReputation)
        {
            return _level >= minReputation;
        }

        /// <summary>Get reputation deficit for a breed (used for need decay penalty).</summary>
        public int GetDeficit(int minReputation)
        {
            return Mathf.Max(0, minReputation - _level);
        }

        /// <summary>Add reputation points (from adoptions, good pension service).</summary>
        public void AddPoints(int points)
        {
            _reputationPoints += points;
        }

        /// <summary>Try to upgrade to next level. Returns true if successful.</summary>
        public bool TryUpgrade(int currentCatCount, float avgHappiness, System.Func<int, bool> spendCoins)
        {
            if (_level >= 10) return false;

            var next = Levels[_level + 1];
            if (currentCatCount < next.CatsRequired) return false;
            if (avgHappiness < next.MinHappiness) return false;
            if (!spendCoins(next.UpgradeCost)) return false;

            _level++;
            OnLevelChanged?.Invoke(_level);
            return true;
        }

        public int SaveLevel() => _level;
        public int SavePoints() => _reputationPoints;
    }

    public readonly struct ReputationLevel
    {
        public readonly int Index;
        public readonly string Name;
        public readonly int CatsRequired;
        public readonly float MinHappiness;
        public readonly int UpgradeCost;
        public readonly int MaxCats;

        public ReputationLevel(int index, string name, int catsRequired, float minHappiness, int upgradeCost, int maxCats)
        {
            Index = index;
            Name = name;
            CatsRequired = catsRequired;
            MinHappiness = minHappiness;
            UpgradeCost = upgradeCost;
            MaxCats = maxCats;
        }
    }
}
