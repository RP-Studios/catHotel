using System;
using UnityEngine;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Manages reputation level (0-10). Level-up is now MANUAL via the LevelUnlock panel.
    /// Conditions: XP threshold + N cats with happiness ≥ 75% + coin cost.
    /// Each level above 0 unlocks a corresponding upper floor (rep N → floor N).
    /// </summary>
    public class ReputationManager : MonoBehaviour
    {
        public const float HappyCatThreshold = 75f;

        // (level, nameKey, xpRequired, happyCatsRequired, coinCost, totalCapacity)
        private static readonly ReputationLevel[] Levels = new[]
        {
            new ReputationLevel(0,  "rep.0",  0,      0,  0,       5),
            new ReputationLevel(1,  "rep.1",  50,     2,  750,    10),
            new ReputationLevel(2,  "rep.2",  150,    3,  1500,   12),
            new ReputationLevel(3,  "rep.3",  350,    4,  4000,   16),
            new ReputationLevel(4,  "rep.4",  700,    5,  8000,   18),
            new ReputationLevel(5,  "rep.5",  1200,   7,  18000,  24),
            new ReputationLevel(6,  "rep.6",  1950,   9,  25000,  26),
            new ReputationLevel(7,  "rep.7",  3000,   12, 60000,  32),
            new ReputationLevel(8,  "rep.8",  4500,   15, 80000,  34),
            new ReputationLevel(9,  "rep.9",  6500,   18, 120000, 40),
            new ReputationLevel(10, "rep.10", 10000,  22, 200000, 45),
        };

        // XP rewards
        public const int XpPension = 10;
        public const int XpAdoption = 25;
        public const float XpHappyBonus = 1.5f;
        public const float XpSpecialMultiplier = 2f;
        public const float HappyBonusThreshold = 80f;

        public const int MaxLevel = 10;

        private int _level;
        private int _xp;

        public int Level => _level;
        public int Xp => _xp;
        public string LevelNameKey => Levels[_level].NameKey;
        public int MaxCats => Levels[_level].TotalCapacity;
        public ReputationLevel CurrentLevel => Levels[_level];

        public ReputationLevel? NextLevel => _level < MaxLevel ? Levels[_level + 1] : null;
        public bool IsMaxLevel => _level >= MaxLevel;

        /// <summary>XP progress within current level (0-1).</summary>
        public float XpProgress
        {
            get
            {
                if (_level >= MaxLevel) return 1f;
                int currentThreshold = Levels[_level].XpRequired;
                int nextThreshold = Levels[_level + 1].XpRequired;
                int range = nextThreshold - currentThreshold;
                if (range <= 0) return 1f;
                return Mathf.Clamp01((float)(_xp - currentThreshold) / range);
            }
        }

        public event Action<int> OnLevelChanged;
        public event Action<int> OnXpGained;

        public void Init(int level, int xp)
        {
            _level = Mathf.Clamp(level, 0, MaxLevel);
            _xp = xp;
        }

        public bool IsBreedUnlocked(int minReputation) => _level >= minReputation;
        public int GetDeficit(int minReputation) => Mathf.Max(0, minReputation - _level);

        // ---- XP awarding (no auto level-up — that's manual now) ----

        public void AwardPensionXp(float catHappiness, bool isSpecial)
        {
            int xp = XpPension;
            if (catHappiness > HappyBonusThreshold) xp = Mathf.RoundToInt(xp * XpHappyBonus);
            if (isSpecial) xp = Mathf.RoundToInt(xp * XpSpecialMultiplier);
            _xp += xp;
            OnXpGained?.Invoke(xp);
        }

        public void AwardAdoptionXp(float catHappiness, bool isSpecial)
        {
            int xp = XpAdoption;
            if (catHappiness > HappyBonusThreshold) xp = Mathf.RoundToInt(xp * XpHappyBonus);
            if (isSpecial) xp = Mathf.RoundToInt(xp * XpSpecialMultiplier);
            _xp += xp;
            OnXpGained?.Invoke(xp);
        }

        // ---- Manual level-up (called by LevelUnlockPanel) ----

        public enum LevelUpResult
        {
            Success,
            AlreadyMax,
            NotEnoughXp,
            NotEnoughHappyCats,
            NotEnoughCoins
        }

        public LevelUpResult CanLevelUp(int currentHappyCats, int currentCoins)
        {
            if (_level >= MaxLevel) return LevelUpResult.AlreadyMax;
            var next = Levels[_level + 1];
            if (_xp < next.XpRequired) return LevelUpResult.NotEnoughXp;
            if (currentHappyCats < next.HappyCatsRequired) return LevelUpResult.NotEnoughHappyCats;
            if (currentCoins < next.CoinCost) return LevelUpResult.NotEnoughCoins;
            return LevelUpResult.Success;
        }

        /// <summary>Try to level up. spendCoins is invoked atomically — if it returns false, level-up aborts.</summary>
        public LevelUpResult TryLevelUp(int currentHappyCats, Func<int, bool> spendCoins)
        {
            if (_level >= MaxLevel) return LevelUpResult.AlreadyMax;
            var next = Levels[_level + 1];
            if (_xp < next.XpRequired) return LevelUpResult.NotEnoughXp;
            if (currentHappyCats < next.HappyCatsRequired) return LevelUpResult.NotEnoughHappyCats;
            if (!spendCoins(next.CoinCost)) return LevelUpResult.NotEnoughCoins;

            _level++;
            OnLevelChanged?.Invoke(_level);
            return LevelUpResult.Success;
        }

        public int SaveLevel() => _level;
        public int SaveXp() => _xp;
    }

    public readonly struct ReputationLevel
    {
        public readonly int Index;
        public readonly string NameKey;
        public readonly int XpRequired;
        public readonly int HappyCatsRequired;
        public readonly int CoinCost;
        public readonly int TotalCapacity;

        public ReputationLevel(int index, string nameKey, int xpRequired,
            int happyCatsRequired, int coinCost, int totalCapacity)
        {
            Index = index;
            NameKey = nameKey;
            XpRequired = xpRequired;
            HappyCatsRequired = happyCatsRequired;
            CoinCost = coinCost;
            TotalCapacity = totalCapacity;
        }
    }
}
