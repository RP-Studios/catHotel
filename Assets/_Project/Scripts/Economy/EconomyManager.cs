using System;
using System.Collections.Generic;
using UnityEngine;
using CatHotel.Cats;
using CatHotel.Core;
using CatHotel.Services;

namespace CatHotel.Economy
{
    /// <summary>
    /// Manages coins and gems. Handles revenue ticks and tap-to-collect.
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        [SerializeField] private GameConfig _config;

        private int _coins;
        private int _gems;

        public int Coins => _coins;
        public int Gems => _gems;

        public event Action<int> OnCoinsChanged;
        public event Action<int> OnGemsChanged;

        // One floating coin per cat (stacks up to MaxStacks)
        private readonly Dictionary<Transform, FloatingCoin> _floatingCoins = new();

        public const int MaxStacks = 5;

        public IReadOnlyDictionary<Transform, FloatingCoin> FloatingCoins => _floatingCoins;
        public event Action<FloatingCoin> OnCoinSpawned;
        public event Action<FloatingCoin> OnCoinStacked;

        public void Init(GameConfig config, int coins, int gems)
        {
            _config = config;
            _coins = coins;
            _gems = gems;
            OnCoinsChanged?.Invoke(_coins);
            OnGemsChanged?.Invoke(_gems);
        }

        public void Init(GameConfig config)
        {
            Init(config, config.startingCoins, config.startingGems);
        }

        public bool TrySpend(int amount)
        {
            if (_coins < amount) return false;
            _coins -= amount;
            OnCoinsChanged?.Invoke(_coins);
            return true;
        }

        public bool TrySpendGems(int amount)
        {
            if (_gems < amount) return false;
            _gems -= amount;
            OnGemsChanged?.Invoke(_gems);
            return true;
        }

        public void AddCoins(int amount)
        {
            _coins += amount;
            OnCoinsChanged?.Invoke(_coins);
        }

        public void AddGems(int amount)
        {
            _gems += amount;
            OnGemsChanged?.Invoke(_gems);
        }

        /// <summary>
        /// Called when a cat finishes using a service object.
        /// Spawns a new coin or stacks on existing one (max 5 stacks).
        /// </summary>
        public void ProcessRevenueTick(CatHappiness happiness, CatBreedData breed, Transform catTransform, bool isSpecial)
        {
            if (happiness.IsUnhappy) return;

            int baseRevenue = _config.coinsPerServiceUse;
            float mult = breed.revenueMultiplier;
            if (isSpecial) mult *= breed.specialRevenueMult;

            if (RevenueBoostManager.Instance != null)
                mult *= RevenueBoostManager.Instance.BoostMultiplier;

            int amount = Mathf.RoundToInt(baseRevenue * mult);
            if (amount <= 0) return;

            if (_floatingCoins.TryGetValue(catTransform, out var existing))
            {
                if (existing.Stacks < MaxStacks)
                {
                    existing.Stacks++;
                    existing.Amount += amount;
                    OnCoinStacked?.Invoke(existing);
                }
                return;
            }

            var coin = new FloatingCoin
            {
                Amount = amount,
                Stacks = 1,
                WorldPosition = catTransform.position + Vector3.up * 0.8f,
                CatTransform = catTransform,
                SpawnTime = Time.time
            };

            _floatingCoins[catTransform] = coin;
            OnCoinSpawned?.Invoke(coin);
        }

        /// <summary>
        /// Remove a coin from tracking. Called by FloatingCoinView before animating.
        /// Does NOT add coins — call DepositCoins() after animation completes.
        /// </summary>
        public void StartCollect(FloatingCoin coin)
        {
            if (coin.CatTransform != null)
                _floatingCoins.Remove(coin.CatTransform);
        }

        /// <summary>Called by the view when a coin fly animation finishes.</summary>
        public void DepositCoins(int amount)
        {
            _coins += amount;
            OnCoinsChanged?.Invoke(_coins);
        }

        /// <summary>Check if a cat has a pending floating coin.</summary>
        public bool HasCoinForCat(Transform catTransform)
        {
            return _floatingCoins.ContainsKey(catTransform);
        }
    }

    public class FloatingCoin
    {
        public int Amount;
        public int Stacks;
        public Vector3 WorldPosition;
        public Transform CatTransform;
        public float SpawnTime;
    }
}
