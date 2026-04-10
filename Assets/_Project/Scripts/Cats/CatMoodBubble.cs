using UnityEngine;
using CatHotel.Core;

namespace CatHotel.Cats
{
    public enum CatMood
    {
        Normal,      // no bubble
        Happy,       // content (Joyous)
        Ecstatic,    // enthousiaste (Very Happy)
        Depressed,   // déprimé (Upset)
        Aggressive,  // bagarreur (Fights) — not yet triggered
        Angry        // colérique (Mad) — not yet triggered
    }

    /// <summary>
    /// Displays two bubbles above the cat:
    /// - Mood bubble (right): emotion based on happiness thresholds
    /// - Need bubble (left, slightly lower): most urgent unmet need
    /// </summary>
    public class CatMoodBubble : MonoBehaviour
    {
        // Mood sprites
        private Sprite _happySprite;
        private Sprite _ecstaticSprite;
        private Sprite _depressedSprite;
        private Sprite _aggressiveSprite;
        private Sprite _angrySprite;

        // Need sprites
        private Sprite _needHungrySprite;
        private Sprite _needThirstySprite;
        private Sprite _needTiredSprite;
        private Sprite _needBoredSprite;
        private Sprite _needDirtySprite;

        private CatHappiness _happiness;
        private CatNeeds _needs;
        private GameConfig _config;

        // Mood bubble (right)
        private GameObject _moodObj;
        private SpriteRenderer _moodSr;
        private CatMood _currentMood = CatMood.Normal;

        // Need bubble (left, slightly lower)
        private GameObject _needObj;
        private SpriteRenderer _needSr;
        private NeedType? _currentNeed;

        private const float TickInterval = 0.3f;
        private float _tickTimer;

        // Mood bubble offset to the right, need bubble offset to the left
        private const float MoodOffsetX = 0.35f;
        private const float NeedOffsetX = -0.35f;
        private const float NeedOffsetY = -0.1f;

        // Scale: 2x native size
        private const float BubbleScale = 2f;

        // Gentle floating animation
        private const float BobAmplitude = 0.04f;
        private const float BobSpeed = 1.8f;
        private float _moodBaseY;
        private float _needBaseY;

        public CatMood CurrentMood => _currentMood;

        public void Init(CatHappiness happiness, CatNeeds needs, GameConfig config,
            Sprite happy, Sprite ecstatic, Sprite depressed, Sprite aggressive, Sprite angry,
            Sprite needHungry, Sprite needThirsty, Sprite needTired, Sprite needBored, Sprite needDirty)
        {
            _happiness = happiness;
            _needs = needs;
            _config = config;
            _happySprite = happy;
            _ecstaticSprite = ecstatic;
            _depressedSprite = depressed;
            _aggressiveSprite = aggressive;
            _angrySprite = angry;
            _needHungrySprite = needHungry;
            _needThirstySprite = needThirsty;
            _needTiredSprite = needTired;
            _needBoredSprite = needBored;
            _needDirtySprite = needDirty;

            float h = _config.moodBubbleHeight;

            // Compensate parent scale so bubbles are always the same world size
            float parentScale = transform.localScale.x;
            float compensated = (parentScale > 0.01f) ? BubbleScale / parentScale : BubbleScale;

            _moodBaseY = h;
            _needBaseY = h - 0.25f;

            // Mood bubble (right / center)
            _moodObj = new GameObject("MoodBubble");
            _moodObj.transform.SetParent(transform);
            _moodObj.transform.localPosition = new Vector3(MoodOffsetX, _moodBaseY, 0f);
            _moodObj.transform.localScale = Vector3.one * compensated;
            _moodSr = _moodObj.AddComponent<SpriteRenderer>();
            _moodSr.sortingLayerName = "Bubbles";
            _moodSr.sortingOrder = 0;
            _moodObj.SetActive(false);

            // Need bubble (left of mood, slightly lower)
            _needObj = new GameObject("NeedBubble");
            _needObj.transform.SetParent(transform);
            _needObj.transform.localPosition = new Vector3(NeedOffsetX, _needBaseY, 0f);
            _needObj.transform.localScale = Vector3.one * compensated;
            _needSr = _needObj.AddComponent<SpriteRenderer>();
            _needSr.sortingLayerName = "Bubbles";
            _needSr.sortingOrder = 0;
            _needObj.SetActive(false);
        }

        private void Update()
        {
            if (_happiness == null || _config == null) return;

            // Gentle bob animation
            float t = Time.time * BobSpeed;
            if (_moodObj.activeSelf)
            {
                var p = _moodObj.transform.localPosition;
                p.y = _moodBaseY + Mathf.Sin(t) * BobAmplitude;
                _moodObj.transform.localPosition = p;
            }
            if (_needObj.activeSelf)
            {
                // Phase offset of 0.8 for organic feel
                var p = _needObj.transform.localPosition;
                p.y = _needBaseY + Mathf.Sin(t + 0.8f) * BobAmplitude;
                _needObj.transform.localPosition = p;
            }

            _tickTimer += Time.deltaTime;
            if (_tickTimer < TickInterval) return;
            _tickTimer = 0f;

            // Mood
            var newMood = EvaluateMood();
            if (newMood != _currentMood)
            {
                _currentMood = newMood;
                ApplyMood();
            }

            // Need
            var urgentNeed = _needs != null ? EvaluateNeed() : null;
            if (urgentNeed != _currentNeed)
            {
                _currentNeed = urgentNeed;
                ApplyNeed();
            }
        }

        private CatMood EvaluateMood()
        {
            float h = _happiness.Value;

            if (h < _config.moodDepressedThreshold)
                return CatMood.Depressed;
            if (h >= _config.moodEcstaticThreshold)
                return CatMood.Ecstatic;
            if (h >= _config.moodHappyThreshold)
                return CatMood.Happy;

            return CatMood.Normal;
        }

        private NeedType? EvaluateNeed()
        {
            // Show the most urgent need that is below the seek threshold
            NeedType? worst = null;
            float worstVal = float.MaxValue;
            float threshold = _config.seekThreshold;

            CheckNeed(NeedType.Hunger, _needs.Hunger, threshold, ref worst, ref worstVal);
            CheckNeed(NeedType.Thirst, _needs.Thirst, threshold, ref worst, ref worstVal);
            CheckNeed(NeedType.Sleep, _needs.Sleep, threshold, ref worst, ref worstVal);
            CheckNeed(NeedType.Play, _needs.Play, threshold, ref worst, ref worstVal);
            CheckNeed(NeedType.Clean, _needs.Clean, threshold, ref worst, ref worstVal);

            return worst;
        }

        private static void CheckNeed(NeedType type, float value, float threshold,
            ref NeedType? worst, ref float worstVal)
        {
            if (value < threshold && value < worstVal)
            {
                worst = type;
                worstVal = value;
            }
        }

        private void ApplyMood()
        {
            if (_currentMood == CatMood.Normal)
            {
                _moodObj.SetActive(false);
                return;
            }

            var sprite = _currentMood switch
            {
                CatMood.Happy => _happySprite,
                CatMood.Ecstatic => _ecstaticSprite,
                CatMood.Depressed => _depressedSprite,
                CatMood.Aggressive => _aggressiveSprite,
                CatMood.Angry => _angrySprite,
                _ => null
            };

            if (sprite == null) { _moodObj.SetActive(false); return; }
            _moodSr.sprite = sprite;
            _moodObj.SetActive(true);
        }

        private void ApplyNeed()
        {
            if (_currentNeed == null)
            {
                _needObj.SetActive(false);
                return;
            }

            var sprite = _currentNeed switch
            {
                NeedType.Hunger => _needHungrySprite,
                NeedType.Thirst => _needThirstySprite,
                NeedType.Sleep => _needTiredSprite,
                NeedType.Play => _needBoredSprite,
                NeedType.Clean => _needDirtySprite,
                _ => null
            };

            if (sprite == null) { _needObj.SetActive(false); return; }
            _needSr.sprite = sprite;
            _needObj.SetActive(true);
        }

        /// <summary>Force a mood override (for combat, caprices, etc.).</summary>
        public void SetOverrideMood(CatMood mood)
        {
            if (mood == _currentMood) return;
            _currentMood = mood;
            ApplyMood();
        }

        /// <summary>Clear override and return to happiness-based evaluation.</summary>
        public void ClearOverride()
        {
            var newMood = EvaluateMood();
            if (newMood == _currentMood) return;
            _currentMood = newMood;
            ApplyMood();
        }

        private void OnDestroy()
        {
            if (_moodObj != null) Destroy(_moodObj);
            if (_needObj != null) Destroy(_needObj);
        }
    }
}
