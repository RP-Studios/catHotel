using UnityEngine;
using CatHotel.Core;

namespace CatHotel.Cats
{
    /// <summary>
    /// Tracks the 4 needs of a single cat. Attached alongside CatEntity.
    /// Needs decay over time based on breed traits and game config.
    /// </summary>
    public class CatNeeds : MonoBehaviour
    {
        private float _hunger = 100f;
        private float _thirst = 100f;
        private float _sleep = 100f;
        private float _play = 100f;
        private float _clean = 100f;

        private CatBreedData _breed;
        private GameConfig _config;
        private bool _isSpecial;
        private int _reputationDeficit; // how many levels below min reputation
        private float _sizeMultiplier = 1f; // small cats have higher needs
        private CatTraitModifiers _traitMods = CatTraitModifiers.Default;

        // Tick-based update: needs don't need per-frame precision
        private const float TickInterval = 0.2f;
        private float _tickAccumulator;

        public float Hunger => _hunger;
        public float Thirst => _thirst;
        public float Sleep => _sleep;
        public float Play => _play;
        public float Clean => _clean;

        /// <summary>Average of all 5 needs (0-100).</summary>
        public float Average => (_hunger + _thirst + _sleep + _play + _clean) / 5f;

        public void Init(CatBreedData breed, GameConfig config, bool isSpecial = false)
        {
            _breed = breed;
            _config = config;
            _isSpecial = isSpecial;
            _hunger = Random.Range(70f, 100f);
            _thirst = Random.Range(70f, 100f);
            _sleep = Random.Range(70f, 100f);
            _play = Random.Range(70f, 100f);
            _clean = Random.Range(70f, 100f);
        }

        public void SetReputationDeficit(int deficit)
        {
            _reputationDeficit = Mathf.Max(0, deficit);
        }

        /// <summary>
        /// Small cats (&lt;0.7 scale) have slightly higher needs.
        /// scale 0.6 → multiplier 1.2, scale 0.7+ → 1.0
        /// </summary>
        public void SetSizeMultiplier(float scale)
        {
            _sizeMultiplier = scale < 0.7f ? Mathf.Lerp(1.2f, 1f, (scale - 0.6f) / 0.1f) : 1f;
        }

        public void SetTraitModifiers(CatTraitModifiers mods)
        {
            _traitMods = mods;
        }

        /// <summary>Set initial need values for refuge cats: 1-2 random gauges are low.</summary>
        public void SetRefugeStartValues()
        {
            int lowCount = Random.Range(1, 3); // 1 or 2 low gauges
            float[] vals = { _hunger, _thirst, _sleep, _play, _clean };
            var indices = new System.Collections.Generic.List<int> { 0, 1, 2, 3, 4 };

            for (int i = 0; i < lowCount && indices.Count > 0; i++)
            {
                int pick = Random.Range(0, indices.Count);
                vals[indices[pick]] = Random.Range(10f, 30f);
                indices.RemoveAt(pick);
            }

            _hunger = vals[0];
            _thirst = vals[1];
            _sleep = vals[2];
            _play = vals[3];
            _clean = vals[4];
        }

        private void Update()
        {
            if (_config == null) return;

            _tickAccumulator += Time.deltaTime;
            if (_tickAccumulator < TickInterval) return;

            float dt = _tickAccumulator;
            _tickAccumulator = 0f;

            _hunger = Mathf.Max(0f, _hunger - GetDecayRate(NeedType.Hunger) * dt);
            _thirst = Mathf.Max(0f, _thirst - GetDecayRate(NeedType.Thirst) * dt);
            _sleep = Mathf.Max(0f, _sleep - GetDecayRate(NeedType.Sleep) * dt);
            _play = Mathf.Max(0f, _play - GetDecayRate(NeedType.Play) * dt);
            _clean = Mathf.Max(0f, _clean - GetDecayRate(NeedType.Clean) * dt);
        }

        /// <summary>
        /// Decay formula from GDD:
        /// decay = base × trait × demandMult × specialDemand × (1 + 0.3 × reputationDeficit)
        /// </summary>
        public float GetDecayRate(NeedType need)
        {
            float baseRate = _config.GetBaseDecay(need);
            float trait = _breed.GetTraitMultiplier(need);
            float demand = _breed.demandMultiplier;
            float special = _isSpecial ? _breed.specialDemandMult : 1f;
            float repPenalty = 1f + 0.3f * _reputationDeficit;

            float personality = _traitMods.GetNeedDecayMult(need);
            return baseRate * trait * demand * special * repPenalty * _sizeMultiplier * personality;
        }

        public float GetNeed(NeedType need)
        {
            return need switch
            {
                NeedType.Hunger => _hunger,
                NeedType.Thirst => _thirst,
                NeedType.Sleep => _sleep,
                NeedType.Play => _play,
                NeedType.Clean => _clean,
                _ => 100f
            };
        }

        /// <summary>Returns the most critical need below seekThreshold, or null if all OK.</summary>
        public NeedType? GetMostUrgentNeed()
        {
            NeedType? worst = null;
            float worstVal = float.MaxValue;

            CheckNeed(NeedType.Hunger, _hunger, ref worst, ref worstVal);
            CheckNeed(NeedType.Thirst, _thirst, ref worst, ref worstVal);
            CheckNeed(NeedType.Sleep, _sleep, ref worst, ref worstVal);
            CheckNeed(NeedType.Play, _play, ref worst, ref worstVal);
            CheckNeed(NeedType.Clean, _clean, ref worst, ref worstVal);

            return worst;
        }

        private void CheckNeed(NeedType type, float value, ref NeedType? worst, ref float worstVal)
        {
            if (value < _config.seekThreshold && value < worstVal)
            {
                worst = type;
                worstVal = value;
            }
        }

        /// <summary>Satisfy a need by a given amount (from using an object).</summary>
        public void Satisfy(NeedType need, float amount)
        {
            switch (need)
            {
                case NeedType.Hunger: _hunger = Mathf.Min(100f, _hunger + amount); break;
                case NeedType.Thirst: _thirst = Mathf.Min(100f, _thirst + amount); break;
                case NeedType.Sleep: _sleep = Mathf.Min(100f, _sleep + amount); break;
                case NeedType.Play: _play = Mathf.Min(100f, _play + amount); break;
                case NeedType.Clean: _clean = Mathf.Min(100f, _clean + amount); break;
            }
        }

        /// <summary>Check if any need is critically low.</summary>
        public bool HasCriticalNeed()
        {
            return _hunger < _config.criticalThreshold
                || _thirst < _config.criticalThreshold
                || _sleep < _config.criticalThreshold
                || _play < _config.criticalThreshold
                || _clean < _config.criticalThreshold;
        }

        /// <summary>For save/load.</summary>
        public float[] ToArray() => new[] { _hunger, _thirst, _sleep, _play, _clean };

        public void FromArray(float[] values)
        {
            if (values == null || values.Length < 5) return;
            _hunger = values[0];
            _thirst = values[1];
            _sleep = values[2];
            _play = values[3];
            _clean = values[4];
        }
    }
}
