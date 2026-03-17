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
        public event Action<FloatingCoin> OnCoinCollected;

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
            Debug.Log($"[Economy] ProcessRevenueTick: unhappy={happiness.IsUnhappy}, coinsPerUse={_config.coinsPerServiceUse}, catTransform={catTransform != null}");
            if (happiness.IsUnhappy) return;

            int baseRevenue = _config.coinsPerServiceUse;
            float mult = breed.revenueMultiplier;
            if (isSpecial) mult *= breed.specialRevenueMult;

            // Apply ad boost multiplier
            if (RevenueBoostManager.Instance != null)
                mult *= RevenueBoostManager.Instance.BoostMultiplier;

            int amount = Mathf.RoundToInt(baseRevenue * mult);
            if (amount <= 0) return;

            // Check if this cat already has a floating coin
            if (_floatingCoins.TryGetValue(catTransform, out var existing))
            {
                // Stack if under max
                if (existing.Stacks < MaxStacks)
                {
                    existing.Stacks++;
                    existing.Amount += amount;
                    OnCoinStacked?.Invoke(existing);
                }
                // At max stacks — no more revenue until collected
                return;
            }

            // Spawn new coin for this cat
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
        /// Start collecting a coin: removes from dict and fires event.
        /// Coins are NOT added yet — call DepositCoins() after animation.
        /// </summary>
        public bool StartCollect(FloatingCoin coin)
        {
            if (coin.CatTransform != null)
                _floatingCoins.Remove(coin.CatTransform);
            OnCoinCollected?.Invoke(coin);
            return true;
        }

        /// <summary>
        /// Start collecting ALL coins: removes from dict and fires events.
        /// Returns the list of coins to animate. Call DepositCoins() after each animation.
        /// </summary>
        public List<FloatingCoin> StartCollectAll()
        {
            var coins = new List<FloatingCoin>(_floatingCoins.Values);
            foreach (var coin in coins)
                OnCoinCollected?.Invoke(coin);
            _floatingCoins.Clear();
            return coins;
        }

        /// <summary>Called by the view when a coin fly animation finishes.</summary>
        public void DepositCoins(int amount)
        {
            _coins += amount;
            OnCoinsChanged?.Invoke(_coins);
        }

        /// <summary>Remove coin for a cat that left the hotel.</summary>
        public void RemoveCoinForCat(Transform catTransform)
        {
            _floatingCoins.Remove(catTransform);
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
