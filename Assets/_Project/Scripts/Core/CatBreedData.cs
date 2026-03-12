using UnityEngine;

namespace CatHotel.Core
{
    [CreateAssetMenu(fileName = "NewBreed", menuName = "Cat Hotel/Cat Breed")]
    public class CatBreedData : ScriptableObject
    {
        [Header("Identity")]
        public string breedName;
        public int minReputation;
        public bool isAggressive;

        [Header("Sprites (static fallback)")]
        public Sprite frontSprite;
        public Sprite rightSprite;
        public Sprite backSprite;

        [Header("Animator")]
        public RuntimeAnimatorController controller;

        [Header("Stats Multipliers")]
        [Tooltip("Global demand multiplier (higher = needs decay faster)")]
        public float demandMultiplier = 1f;

        [Tooltip("Per-need trait multipliers (stacked on top of demandMultiplier)")]
        public float hungerTrait = 1f;
        public float sleepTrait = 1f;
        public float playTrait = 1f;
        public float cleanTrait = 1f;

        [Header("Physical")]
        [Tooltip("Visual scale multiplier")]
        public float size = 1f;
        [Tooltip("Movement speed multiplier")]
        public float speed = 1f;

        [Header("Revenue")]
        [Tooltip("Base revenue multiplier for this breed")]
        public float revenueMultiplier = 1f;

        [Header("Pension")]
        [Tooltip("Base stay duration range in seconds")]
        public Vector2 stayDurationRange = new(3000f, 4200f); // ~50min to ~70min (avg 1h)

        [Header("Special Cat")]
        public bool hasSpecialVariant;
        public string specialName;
        [Range(0f, 1f)] public float specialChance = 0.08f;
        public float specialRevenueMult = 2f;
        public float specialDemandMult = 1.5f;
        public Sprite specialFrontSprite;
        public Sprite specialRightSprite;
        public Sprite specialBackSprite;
        public RuntimeAnimatorController specialController;

        /// <summary>Get the trait multiplier for a specific need type.</summary>
        public float GetTraitMultiplier(NeedType need)
        {
            return need switch
            {
                NeedType.Hunger => hungerTrait,
                NeedType.Sleep => sleepTrait,
                NeedType.Play => playTrait,
                NeedType.Clean => cleanTrait,
                _ => 1f
            };
        }
    }
}
