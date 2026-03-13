using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatHotel.Cats;
using CatHotel.Core;
using CatHotel.Economy;
using CatHotel.Grid;
using CatHotel.Services;
using CatHotel.UI;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Central game orchestrator. Manages cat arrivals (pension/refuge),
    /// service-based revenue, departures, and ties all systems together.
    /// </summary>
    public class HotelManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private GameConfig _config;
        [SerializeField] private CatBreedData[] _availableBreeds;

        [Header("References")]
        [SerializeField] private GridRenderer _gridRenderer;
        [SerializeField] private EconomyManager _economy;
        [SerializeField] private ReputationManager _reputation;
        [SerializeField] private CatSpawner _catSpawner;

        private readonly List<CatInstance> _cats = new();
        private float _arrivalTimer;

        // Tick-based update for departures + pension (no need to check every frame)
        private const float CatTickInterval = 0.5f;
        private float _catTickTimer;

        public IReadOnlyList<CatInstance> Cats => _cats;
        public GameConfig Config => _config;
        public EconomyManager Economy => _economy;
        public ReputationManager Reputation => _reputation;
        public int CatCount => _cats.Count;
        public float ArrivalTimer => _arrivalTimer;

        public event Action<CatInstance> OnCatArrived;
        public event Action<CatInstance> OnCatDeparted;

        private IEnumerator Start()
        {
            // Auth (non-bloquant, timeout 5s)
            if (AuthManager.Instance != null)
            {
                var authTask = AuthManager.Instance.InitializeAsync();
                float timeout = 5f;
                while (!authTask.IsCompleted && timeout > 0f)
                {
                    timeout -= Time.deltaTime;
                    yield return null;
                }
                if (!authTask.IsCompleted)
                    Debug.LogWarning("[Hotel] Auth timeout — continuing offline");
            }

            if (_economy != null && _config != null)
                _economy.Init(_config);

            if (_reputation != null)
                _reputation.Init(0, 0);

            // Wait one frame for GridRenderer.Start() to build the room and entrances
            yield return null;

            // Spawn first cat immediately
            TrySpawnCat();
        }

        private void Update()
        {
            if (_config == null) return;

            UpdateArrivals();

            // Departures + pension merged into a single tick (0.5s is fine for these checks)
            _catTickTimer += Time.deltaTime;
            if (_catTickTimer >= CatTickInterval)
            {
                float dt = _catTickTimer;
                _catTickTimer = 0f;
                UpdateCats(dt);
            }
        }

        private void UpdateArrivals()
        {
            _arrivalTimer += Time.deltaTime;
            if (_arrivalTimer < _config.arrivalInterval) return;
            _arrivalTimer = 0f;

            if (_cats.Count >= _config.maxCats) return;

            TrySpawnCat();
        }

        /// <summary>Single merged loop for departures + pension timers (tick-based).</summary>
        private void UpdateCats(float dt)
        {
            for (int i = _cats.Count - 1; i >= 0; i--)
            {
                var cat = _cats[i];

                // Sample happiness for average tracking
                if (cat.Happiness != null && cat.State == CatState.Idle)
                {
                    cat.HappinessSum += cat.Happiness.Value;
                    cat.HappinessSamples++;
                }

                // Departure check (skip cats already leaving or being picked up)
                if (cat.Happiness != null && cat.Happiness.ShouldLeave
                    && cat.State != CatState.Leaving && cat.State != CatState.Pickup)
                {
                    StartCatDeparture(cat, CatState.Leaving);
                    continue;
                }

                // Pension timer
                if (cat.Mode != CatMode.Pension) continue;
                if (cat.State == CatState.Leaving || cat.State == CatState.Pickup) continue;

                cat.PensionTimeRemaining -= dt;
                if (cat.PensionTimeRemaining <= 0f)
                {
                    ProcessPensionEnd(cat);
                }
            }
        }

        /// <summary>Called when a cat finishes using a service object. Spawns a floating coin.</summary>
        private void OnCatServiceUsed(CatInstance cat)
        {
            if (cat.Entity == null || cat.Happiness == null) return;
            if (cat.Happiness.IsUnhappy) return;

            _economy.ProcessRevenueTick(
                cat.Happiness, cat.Breed, cat.Entity.transform, cat.IsSpecial);
        }

        private void TrySpawnCat()
        {
            var entrances = _gridRenderer.Entrances;
            if (entrances == null || entrances.Count == 0) return;

            // Pick a breed the player has unlocked
            var breed = PickRandomBreed();
            if (breed == null) return;

            // All cats are pension for now
            var mode = CatMode.Pension;

            // Pick entrance
            var entrance = entrances[UnityEngine.Random.Range(0, entrances.Count)];

            // Special variants disabled for J1
            bool isSpecial = false;

            // Create the cat GameObject
            var go = new GameObject($"Cat_{_cats.Count}_{breed.breedName}");
            go.transform.SetParent(transform);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = isSpecial && breed.specialFrontSprite != null
                ? breed.specialFrontSprite : breed.frontSprite;
            sr.sortingOrder = 10;

            if (breed.controller != null)
            {
                var animator = go.AddComponent<Animator>();
                var ctrl = isSpecial && breed.specialController != null
                    ? breed.specialController : breed.controller;
                animator.runtimeAnimatorController = ctrl;
                animator.enabled = false;
            }

            float scale = breed.size * UnityEngine.Random.Range(0.9f, 1.1f);
            go.transform.localScale = new Vector3(scale, scale, 1f);

            // Add CatNeeds & CatHappiness BEFORE CatEntity.Init() so _needs is found
            var needs = go.AddComponent<CatNeeds>();
            needs.Init(breed, _config, isSpecial);

            int repDeficit = _reputation.GetDeficit(breed.minReputation);
            needs.SetReputationDeficit(repDeficit);

            var happiness = go.AddComponent<CatHappiness>();
            happiness.Init(needs, _config);

            // Add CatEntity last — Init() calls GetComponent<CatNeeds>()
            var entity = go.AddComponent<CatEntity>();
            var frontSpr = isSpecial && breed.specialFrontSprite != null ? breed.specialFrontSprite : breed.frontSprite;
            var rightSpr = isSpecial && breed.specialRightSprite != null ? breed.specialRightSprite : breed.rightSprite;
            var backSpr = isSpecial && breed.specialBackSprite != null ? breed.specialBackSprite : breed.backSprite;
            entity.SetSprites(frontSpr, rightSpr, backSpr);
            entity.SetBreed(breed);
            entity.Init(_gridRenderer.Data, entrance, _catSpawner);

            // Pick a name
            string catName = isSpecial ? breed.specialName : CatNames.GetRandomName();

            // Pension duration: 1-5 minutes
            float pensionDuration = UnityEngine.Random.Range(60f, 300f);

            var instance = new CatInstance
            {
                Entity = entity,
                Needs = needs,
                Happiness = happiness,
                Breed = breed,
                Mode = mode,
                IsSpecial = isSpecial,
                CatName = catName,
                State = CatState.Arriving,
                PensionTimeRemaining = pensionDuration,
                PensionDuration = pensionDuration
            };

            // Generate a floating coin each time this cat finishes using a service
            entity.OnServiceUsed += () => OnCatServiceUsed(instance);

            _cats.Add(instance);
            if (_catSpawner != null) _catSpawner.RegisterCat(entity);
            OnCatArrived?.Invoke(instance);

            // Walk from entrance to a random floor cell
            var floorCells = _gridRenderer.CentralRoomFloorCells;
            if (floorCells.Count > 0)
            {
                var target = floorCells[UnityEngine.Random.Range(0, floorCells.Count)];
                entity.WalkToTarget(target, () => instance.State = CatState.Idle);
            }
            else
            {
                instance.State = CatState.Idle;
            }

        }

        // Reusable buffer to avoid allocation per spawn
        private readonly List<CatBreedData> _unlockedBuffer = new();

        private CatBreedData PickRandomBreed()
        {
            _unlockedBuffer.Clear();
            foreach (var breed in _availableBreeds)
            {
                if (_reputation.IsBreedUnlocked(breed.minReputation))
                    _unlockedBuffer.Add(breed);
            }
            if (_unlockedBuffer.Count == 0) return null;
            return _unlockedBuffer[UnityEngine.Random.Range(0, _unlockedBuffer.Count)];
        }

        private void ProcessPensionEnd(CatInstance cat)
        {
            cat.State = CatState.Pickup;
            cat.Entity.SetDeparting();

            // Walk to exit first
            var entrances = _gridRenderer.Entrances;
            if (entrances != null && entrances.Count > 0)
            {
                var exit = entrances[UnityEngine.Random.Range(0, entrances.Count)];
                cat.Entity.WalkToTarget(exit, () => ShowPensionEndPanel(cat));
            }
            else
            {
                ShowPensionEndPanel(cat);
            }
        }

        private void ShowPensionEndPanel(CatInstance cat)
        {
            // Calculate payment
            float happiness = cat.Happiness.Value;
            float avgHappiness = cat.AverageHappiness;
            float payment = _config.pensionBaseRate * cat.PensionDuration * (happiness / 100f)
                * cat.Breed.revenueMultiplier;
            int baseCoins = Mathf.RoundToInt(payment);

            int tipCoins = 0;
            if (happiness > 80f)
                tipCoins = Mathf.RoundToInt(baseCoins * _config.tipPercent);

            int totalCoins = baseCoins + tipCoins;

            // Get front sprite
            Sprite frontSprite = cat.IsSpecial && cat.Breed.specialFrontSprite != null
                ? cat.Breed.specialFrontSprite : cat.Breed.frontSprite;

            // Pause game
            Time.timeScale = 0f;

            // Show panel
            var panel = GetComponent<EndPensionPanel>();
            if (panel != null)
            {
                panel.Show(new PensionEndData
                {
                    CatSprite = frontSprite,
                    CatName = cat.CatName,
                    AvgHappiness = avgHappiness,
                    BaseCoins = baseCoins,
                    TipCoins = tipCoins,
                    TotalCoins = totalCoins
                }, () =>
                {
                    // Collect callback — unpause, add coins, award XP, finalize departure
                    Time.timeScale = 1f;
                    _economy.AddCoins(totalCoins);

                    _reputation.AwardPensionXp(happiness, cat.IsSpecial,
                        _cats.Count, GetAverageHappiness(), _economy.TrySpend);

                    FinalizeDeparture(cat);
                });
            }
            else
            {
                // Fallback if no panel — old behavior
                Time.timeScale = 1f;
                _economy.AddCoins(totalCoins);
                _reputation.AwardPensionXp(happiness, cat.IsSpecial,
                    _cats.Count, GetAverageHappiness(), _economy.TrySpend);
                FinalizeDeparture(cat);
            }
        }

        /// <summary>Process adoption for a refuge cat (called when adopter arrives).</summary>
        public void ProcessAdoption(CatInstance cat)
        {
            if (cat.Mode != CatMode.Refuge) return;

            float fee = cat.Breed.revenueMultiplier * _config.adoptionFeeMultiplier
                * 100f * (cat.Happiness.Value / 100f);
            int coins = Mathf.RoundToInt(fee);
            _economy.AddCoins(coins);

            // Award XP for completed adoption
            _reputation.AwardAdoptionXp(cat.Happiness.Value, cat.IsSpecial,
                _cats.Count, GetAverageHappiness(), _economy.TrySpend);

            StartCatDeparture(cat, CatState.Adopted);
        }

        private void StartCatDeparture(CatInstance cat, CatState reason)
        {
            cat.State = reason;
            cat.Entity.SetDeparting();

            var entrances = _gridRenderer.Entrances;
            if (entrances != null && entrances.Count > 0)
            {
                var exit = entrances[UnityEngine.Random.Range(0, entrances.Count)];
                cat.Entity.WalkToTarget(exit, () => FinalizeDeparture(cat));
            }
            else
            {
                FinalizeDeparture(cat);
            }
        }

        private void FinalizeDeparture(CatInstance cat)
        {
            _cats.Remove(cat);
            if (_catSpawner != null && cat.Entity != null)
                _catSpawner.UnregisterCat(cat.Entity);
            CatNames.ReleaseName(cat.CatName);
            OnCatDeparted?.Invoke(cat);

            // Remove floating coin for this cat
            if (_economy != null && cat.Entity != null)
                _economy.RemoveCoinForCat(cat.Entity.transform);

            if (cat.Entity != null)
                Destroy(cat.Entity.gameObject);
        }

        /// <summary>Get average happiness across all cats.</summary>
        public float GetAverageHappiness()
        {
            if (_cats.Count == 0) return 100f;
            float sum = 0f;
            foreach (var cat in _cats)
            {
                if (cat.Happiness != null)
                    sum += cat.Happiness.Value;
            }
            return sum / _cats.Count;
        }
    }

    /// <summary>Runtime data for a single cat in the hotel.</summary>
    public class CatInstance
    {
        public CatEntity Entity;
        public CatNeeds Needs;
        public CatHappiness Happiness;
        public CatBreedData Breed;
        public CatMode Mode;
        public CatState State;
        public bool IsSpecial;
        public string CatName;

        // Pension tracking
        public float PensionDuration;
        public float PensionTimeRemaining;

        // Average happiness tracking (sampled each tick)
        public float HappinessSum;
        public int HappinessSamples;
        public float AverageHappiness => HappinessSamples > 0 ? HappinessSum / HappinessSamples : 0f;

        // Adoption tracking
        public float HappyDuration; // seconds continuously happy (for adopter trigger)
    }
}
