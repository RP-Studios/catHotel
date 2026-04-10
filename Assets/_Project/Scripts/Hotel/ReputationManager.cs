using System;
using UnityEngine;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Manages reputation level (0-10). Controls breed unlocks and cat limits.
    /// Level-up requires BOTH: cumulative XP threshold AND enough happy cats.
    /// XP is earned from completed pensions and adoptions.
    /// GDD section 8.
    /// </summary>
    public class ReputationManager : MonoBehaviour
    {
        private static readonly ReputationLevel[] Levels = new[]
        {
            //                                              cats  happiness  cost   maxCats  xpThreshold
            new ReputationLevel(0,  "rep.0",   0,  0,    0,    5,    0),
            new ReputationLevel(1,  "rep.1",   3,  60,   100,  10,   50),
            new ReputationLevel(2,  "rep.2",   5,  65,   200,  15,   150),
            new ReputationLevel(3,  "rep.3",   7,  70,   350,  20,   350),
            new ReputationLevel(4,  "rep.4",   10, 72,   500,  25,   700),
            new ReputationLevel(5,  "rep.5",   12, 75,   750,  30,   1200),
            new ReputationLevel(6,  "rep.6",   14, 77,   1000, 35,   1950),
            new ReputationLevel(7,  "rep.7",   16, 80,   1500, 40,   3000),
            new ReputationLevel(8,  "rep.8",   18, 82,   2000, 45,   4500),
            new ReputationLevel(9,  "rep.9",   20, 85,   3000, 50,   6500),
            new ReputationLevel(10, "rep.10",  25, 88,   5000, 55,   10000),
        };

        // XP rewards
        public const int XpPension = 10;
        public const int XpAdoption = 25;
        public const float XpHappyBonus = 1.5f;   // x1.5 if happiness > 80%
        public const float XpSpecialMultiplier = 2f; // x2 for special cats
        public const float HappyBonusThreshold = 80f;

        private int _level;
        private int _xp;

        public int Level => _level;
        public int Xp => _xp;
        public string LevelName => Levels[_level].Name;
        public int MaxCats => Levels[_level].MaxCats;
        public ReputationLevel CurrentLevel => Levels[_level];

        /// <summary>Get the next level data, or null if already max.</summary>
        public ReputationLevel? NextLevel => _level < 10 ? Levels[_level + 1] : null;

        /// <summary>XP progress within current level (0-1).</summary>
        public float XpProgress
        {
            get
            {
                if (_level >= 10) return 1f;
                var next = Levels[_level + 1];
                int currentThreshold = Levels[_level].XpThreshold;
                int nextThreshold = next.XpThreshold;
                int range = nextThreshold - currentThreshold;
                if (range <= 0) return 1f;
                return Mathf.Clamp01((float)(_xp - currentThreshold) / range);
            }
        }

        public event Action<int> OnLevelChanged;
        public event Action<int> OnXpGained;

        public void Init(int level, int xp)
        {
            _level = Mathf.Clamp(level, 0, 10);
            _xp = xp;
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

        /// <summary>
        /// Award XP for a completed pension. Checks for auto level-up.
        /// </summary>
        public void AwardPensionXp(float catHappiness, bool isSpecial,
            int currentCatCount, float avgHappiness, Func<int, bool> spendCoins)
        {
            int xp = XpPension;
            if (catHappiness > HappyBonusThreshold) xp = Mathf.RoundToInt(xp * XpHappyBonus);
            if (isSpecial) xp = Mathf.RoundToInt(xp * XpSpecialMultiplier);

            AddXp(xp, currentCatCount, avgHappiness, spendCoins);
        }

        /// <summary>
        /// Award XP for a completed adoption. Checks for auto level-up.
        /// </summary>
        public void AwardAdoptionXp(float catHappiness, bool isSpecial,
            int currentCatCount, float avgHappiness, Func<int, bool> spendCoins)
        {
            int xp = XpAdoption;
            if (catHappiness > HappyBonusThreshold) xp = Mathf.RoundToInt(xp * XpHappyBonus);
            if (isSpecial) xp = Mathf.RoundToInt(xp * XpSpecialMultiplier);

            AddXp(xp, currentCatCount, avgHappiness, spendCoins);
        }

        private void AddXp(int amount, int currentCatCount, float avgHappiness, Func<int, bool> spendCoins)
        {
            _xp += amount;
            OnXpGained?.Invoke(amount);
            TryAutoLevelUp(currentCatCount, avgHappiness, spendCoins);
        }

        /// <summary>Try to auto level-up if both XP and cat conditions are met.</summary>
        private void TryAutoLevelUp(int currentCatCount, float avgHappiness, Func<int, bool> spendCoins)
        {
            while (_level < 10)
            {
                var next = Levels[_level + 1];
                if (_xp < next.XpThreshold) break;
                if (currentCatCount < next.CatsRequired) break;
                if (avgHappiness < next.MinHappiness) break;
                if (!spendCoins(next.UpgradeCost)) break;

                _level++;
                OnLevelChanged?.Invoke(_level);
            }
        }

        public int SaveLevel() => _level;
        public int SaveXp() => _xp;
    }

    public readonly struct ReputationLevel
    {
        public readonly int Index;
        public readonly string Name;
        public readonly int CatsRequired;
        public readonly float MinHappiness;
        public readonly int UpgradeCost;
        public readonly int MaxCats;
        public readonly int XpThreshold;

        public ReputationLevel(int index, string name, int catsRequired, float minHappiness,
            int upgradeCost, int maxCats, int xpThreshold)
        {
            Index = index;
            Name = name;
            CatsRequired = catsRequired;
            MinHappiness = minHappiness;
            UpgradeCost = upgradeCost;
            MaxCats = maxCats;
            XpThreshold = xpThreshold;
        }
    }
}
