using UnityEngine;
using CatHotel.Core;

namespace CatHotel.Cats
{
    /// <summary>
    /// Calculates and tracks happiness for a single cat.
    /// GDD formula:
    ///   happiness = avg(needs) × (1 - 0.2 × unsatisfied_caprices) - fight_penalty + (comfort - 50) × 0.2 + pet_bonus
    /// </summary>
    public class CatHappiness : MonoBehaviour
    {
        private CatNeeds _needs;
        private GameConfig _config;

        private float _happiness = 100f;
        private float _fightPenalty;
        private float _petBonus;
        private float _petBonusTimer;
        private float _comfort = 50f;
        private int _unsatisfiedCaprices;

        // Tracks time spent below leave threshold
        private float _unhappyTimer;

        // Tick-based update: happiness formula doesn't need per-frame precision
        private const float TickInterval = 0.25f;
        private float _tickAccumulator;

        public float Value => _happiness;
        public bool IsHappy => _happiness >= _config.happyThreshold;
        public bool IsNeutral => _happiness >= _config.neutralThreshold && _happiness < _config.happyThreshold;
        public bool IsUnhappy => _happiness < _config.neutralThreshold;
        public bool ShouldLeave => _unhappyTimer >= _config.leaveDelay;

        public void Init(CatNeeds needs, GameConfig config)
        {
            _needs = needs;
            _config = config;
            _happiness = 100f;
            _unhappyTimer = 0f;
        }

        public void SetComfort(float comfort) => _comfort = comfort;
        public void SetUnsatisfiedCaprices(int count) => _unsatisfiedCaprices = count;

        public void ApplyFightPenalty()
        {
            _fightPenalty += _config.fightPenalty;
        }

        public void ApplyPetBonus()
        {
            _petBonus = _config.petHappinessBoost;
            _petBonusTimer = _config.petCooldown;
        }

        private void Update()
        {
            if (_needs == null || _config == null) return;

            _tickAccumulator += Time.deltaTime;
            if (_tickAccumulator < TickInterval) return;

            float dt = _tickAccumulator;
            _tickAccumulator = 0f;

            // Decay pet bonus over cooldown period
            if (_petBonusTimer > 0f)
            {
                _petBonusTimer -= dt;
                if (_petBonusTimer <= 0f)
                    _petBonus = 0f;
            }

            // Decay fight penalty over time (recover 5/s)
            if (_fightPenalty > 0f)
                _fightPenalty = Mathf.Max(0f, _fightPenalty - 5f * dt);

            // GDD formula
            float needsAvg = _needs.Average;
            float capriceMultiplier = 1f - 0.2f * _unsatisfiedCaprices;
            float comfortBonus = (_comfort - _config.comfortNeutral) * _config.comfortHappinessScale;

            _happiness = Mathf.Clamp(
                needsAvg * capriceMultiplier - _fightPenalty + comfortBonus + _petBonus,
                0f, 100f);

            // Track time below leave threshold
            if (_happiness < _config.unhappyLeaveThreshold)
                _unhappyTimer += dt;
            else
                _unhappyTimer = 0f;
        }
    }
}
