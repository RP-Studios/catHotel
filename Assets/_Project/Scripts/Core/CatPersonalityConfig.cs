using System;
using UnityEngine;

namespace CatHotel.Core
{
    /// <summary>
    /// Gameplay modifiers for a single personality/quirky trait.
    /// All multipliers default to 1 (no effect). Designers tweak per trait in the Inspector.
    /// </summary>
    [Serializable]
    public class CatTraitEffect
    {
        public string traitName;

        [Header("Need Decay Multipliers (1 = normal, 1.3 = +30% faster)")]
        public float hungerDecayMult = 1f;
        public float thirstDecayMult = 1f;
        public float sleepDecayMult = 1f;
        public float playDecayMult = 1f;
        public float cleanDecayMult = 1f;

        [Header("Behavior")]
        [Tooltip("Movement speed multiplier (1 = normal)")]
        public float speedMult = 1f;
        [Tooltip("Petting happiness bonus multiplier (1 = normal, 1.5 = +50%)")]
        public float petBonusMult = 1f;
        [Tooltip("Fight penalty multiplier (1 = normal, 1.5 = +50% penalty)")]
        public float fightPenaltyMult = 1f;
        [Tooltip("Fight penalty decay speed multiplier (1 = normal, 0.5 = stays angry longer)")]
        public float fightRecoveryMult = 1f;
        [Tooltip("Flat happiness offset applied constantly (-5 = grumpy, +5 = cheerful)")]
        public float happinessOffset = 0f;
    }

    /// <summary>
    /// Aggregated modifiers from all active traits on a single cat.
    /// Multipliers are stacked multiplicatively, offsets are summed.
    /// </summary>
    public struct CatTraitModifiers
    {
        public float HungerDecayMult;
        public float ThirstDecayMult;
        public float SleepDecayMult;
        public float PlayDecayMult;
        public float CleanDecayMult;
        public float SpeedMult;
        public float PetBonusMult;
        public float FightPenaltyMult;
        public float FightRecoveryMult;
        public float HappinessOffset;

        public static CatTraitModifiers Default => new()
        {
            HungerDecayMult = 1f,
            ThirstDecayMult = 1f,
            SleepDecayMult = 1f,
            PlayDecayMult = 1f,
            CleanDecayMult = 1f,
            SpeedMult = 1f,
            PetBonusMult = 1f,
            FightPenaltyMult = 1f,
            FightRecoveryMult = 1f,
            HappinessOffset = 0f
        };

        public void Stack(CatTraitEffect e)
        {
            if (e == null) return;
            HungerDecayMult *= e.hungerDecayMult;
            ThirstDecayMult *= e.thirstDecayMult;
            SleepDecayMult *= e.sleepDecayMult;
            PlayDecayMult *= e.playDecayMult;
            CleanDecayMult *= e.cleanDecayMult;
            SpeedMult *= e.speedMult;
            PetBonusMult *= e.petBonusMult;
            FightPenaltyMult *= e.fightPenaltyMult;
            FightRecoveryMult *= e.fightRecoveryMult;
            HappinessOffset += e.happinessOffset;
        }

        public float GetNeedDecayMult(NeedType need)
        {
            return need switch
            {
                NeedType.Hunger => HungerDecayMult,
                NeedType.Thirst => ThirstDecayMult,
                NeedType.Sleep => SleepDecayMult,
                NeedType.Play => PlayDecayMult,
                NeedType.Clean => CleanDecayMult,
                _ => 1f
            };
        }
    }

    [CreateAssetMenu(fileName = "CatPersonalityConfig", menuName = "Cat Hotel/Cat Personality Config")]
    public class CatPersonalityConfig : ScriptableObject
    {
        [Header("Race Trait Labels")]
        public string traitHunger = "Gourmand";
        public string traitThirst = "Assoiffé";
        public string traitSleep = "Dormeur";
        public string traitPlay = "Joueur";
        public string traitClean = "Maniaque";

        [Header("Physical Trait Labels")]
        public string traitAggressive = "Bagarreur";
        public string traitBig = "Imposant";
        public string traitSmall = "Petit gabarit";
        public string traitFast = "Rapide";
        public string traitSlow = "Nonchalant";

        [Header("Physical Thresholds")]
        public float bigSizeThreshold = 1.1f;
        public float smallSizeThreshold = 0.95f;
        public float fastSpeedThreshold = 1.1f;
        public float slowSpeedThreshold = 0.85f;
        public float dominantTraitThreshold = 1.2f;

        [Header("Personality Traits (pool 2)")]
        [Range(0f, 1f)] public float personalityChance = 0.5f;
        public CatTraitEffect[] personalityPool =
        {
            new() { traitName = "Câlin",        petBonusMult = 1.4f },
            new() { traitName = "Indépendant",  petBonusMult = 0.6f },
            new() { traitName = "Curieux",      speedMult = 1.15f },
            new() { traitName = "Craintif",     fightPenaltyMult = 1.5f },
            new() { traitName = "Affectueux",   petBonusMult = 1.5f, happinessOffset = 2f },
            new() { traitName = "Observateur" },
            new() { traitName = "Discret" },
            new() { traitName = "Pot de colle", petBonusMult = 1.6f },
            new() { traitName = "Aventurier",   speedMult = 1.2f },
            new() { traitName = "Territorial",  fightPenaltyMult = 0.7f },
            new() { traitName = "Sociable",     happinessOffset = 3f },
            new() { traitName = "Solitaire",    happinessOffset = -2f },
            new() { traitName = "Malin" },
            new() { traitName = "Paresseux",    speedMult = 0.8f, sleepDecayMult = 1.3f },
            new() { traitName = "Fidèle",       happinessOffset = 2f },
            new() { traitName = "Capricieux",   hungerDecayMult = 1.15f, thirstDecayMult = 1.15f,
                                                 sleepDecayMult = 1.15f, playDecayMult = 1.15f, cleanDecayMult = 1.15f }
        };

        [Header("Quirky Traits (pool 3)")]
        [Range(0f, 1f)] public float quirkyChance = 0.33f;
        public CatTraitEffect[] quirkyPool =
        {
            new() { traitName = "Rancunier",            fightRecoveryMult = 0.4f },
            new() { traitName = "Susceptible",          fightPenaltyMult = 1.4f, happinessOffset = -2f },
            new() { traitName = "Idiot",                speedMult = 0.9f },
            new() { traitName = "Gros mangeur",         hungerDecayMult = 1.4f },
            new() { traitName = "Soiffard",             thirstDecayMult = 1.4f },
            new() { traitName = "Voleur de croquettes", hungerDecayMult = 0.7f },
            new() { traitName = "Ronfleur",             sleepDecayMult = 0.8f },
            new() { traitName = "Peureux des litières", cleanDecayMult = 1.3f },
            new() { traitName = "Jaloux",               fightPenaltyMult = 1.3f, happinessOffset = -3f },
            new() { traitName = "Frimeur",              petBonusMult = 1.3f },
            new() { traitName = "Drama queen",          fightPenaltyMult = 1.5f, petBonusMult = 1.4f },
            new() { traitName = "Collectionneur de poils" },
            new() { traitName = "Grognon",              happinessOffset = -5f },
            new() { traitName = "Maladroit" },
            new() { traitName = "Hypocondriaque",       cleanDecayMult = 1.4f },
            new() { traitName = "Roi du canapé",        sleepDecayMult = 1.3f, speedMult = 0.85f },
            new() { traitName = "Snob",                 happinessOffset = -3f, petBonusMult = 0.7f },
            new() { traitName = "Chapardeur",           hungerDecayMult = 0.8f, thirstDecayMult = 0.8f }
        };

        public string GetRaceTrait(CatBreedData breed)
        {
            if (breed.isAggressive)
                return traitAggressive;

            string dominantLabel = null;
            float dominantValue = dominantTraitThreshold;

            if (breed.hungerTrait > dominantValue) { dominantValue = breed.hungerTrait; dominantLabel = traitHunger; }
            if (breed.thirstTrait > dominantValue) { dominantValue = breed.thirstTrait; dominantLabel = traitThirst; }
            if (breed.sleepTrait > dominantValue)  { dominantValue = breed.sleepTrait;  dominantLabel = traitSleep; }
            if (breed.playTrait > dominantValue)   { dominantValue = breed.playTrait;   dominantLabel = traitPlay; }
            if (breed.cleanTrait > dominantValue)  { dominantValue = breed.cleanTrait;  dominantLabel = traitClean; }

            if (dominantLabel != null)
                return dominantLabel;

            if (breed.size > bigSizeThreshold) return traitBig;
            if (breed.size < smallSizeThreshold) return traitSmall;
            if (breed.speed > fastSpeedThreshold) return traitFast;
            if (breed.speed < slowSpeedThreshold) return traitSlow;

            return traitPlay;
        }

        /// <summary>
        /// Generate description + aggregated gameplay modifiers for a cat.
        /// </summary>
        public (string description, CatTraitModifiers modifiers) GeneratePersonality(CatBreedData breed, int seed)
        {
            var rng = new System.Random(seed);
            var parts = new System.Collections.Generic.List<string>(3);
            var mods = CatTraitModifiers.Default;

            // Trait 1: race trait (no gameplay modifier — breed stats handle this)
            parts.Add(GetRaceTrait(breed));

            // Trait 2: personality
            if (personalityPool.Length > 0 && rng.NextDouble() < personalityChance)
            {
                var trait = personalityPool[rng.Next(personalityPool.Length)];
                if (!parts.Contains(trait.traitName))
                {
                    parts.Add(trait.traitName);
                    mods.Stack(trait);
                }
            }

            // Trait 3: quirky
            if (quirkyPool.Length > 0 && rng.NextDouble() < quirkyChance)
            {
                var trait = quirkyPool[rng.Next(quirkyPool.Length)];
                if (!parts.Contains(trait.traitName))
                {
                    parts.Add(trait.traitName);
                    mods.Stack(trait);
                }
            }

            return (string.Join(", ", parts), mods);
        }

        // Keep backward compat
        public string GenerateDescription(CatBreedData breed, int seed)
        {
            return GeneratePersonality(breed, seed).description;
        }
    }
}
