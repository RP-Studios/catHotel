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
        [Header("Race Trait Keys (resolved via LocalizedStrings)")]
        public string traitHunger = "trait.hunger";
        public string traitThirst = "trait.thirst";
        public string traitSleep = "trait.sleep";
        public string traitPlay = "trait.play";
        public string traitClean = "trait.clean";

        [Header("Physical Trait Keys")]
        public string traitAggressive = "trait.aggressive";
        public string traitBig = "trait.big";
        public string traitSmall = "trait.small";
        public string traitFast = "trait.fast";
        public string traitSlow = "trait.slow";

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
            new() { traitName = "personality.cuddly",       petBonusMult = 1.4f },
            new() { traitName = "personality.independent",   petBonusMult = 0.6f },
            new() { traitName = "personality.curious",       speedMult = 1.15f },
            new() { traitName = "personality.fearful",       fightPenaltyMult = 1.5f },
            new() { traitName = "personality.affectionate",  petBonusMult = 1.5f, happinessOffset = 2f },
            new() { traitName = "personality.observer" },
            new() { traitName = "personality.discreet" },
            new() { traitName = "personality.clingy",        petBonusMult = 1.6f },
            new() { traitName = "personality.adventurer",    speedMult = 1.2f },
            new() { traitName = "personality.territorial",   fightPenaltyMult = 0.7f },
            new() { traitName = "personality.sociable",      happinessOffset = 3f },
            new() { traitName = "personality.solitary",      happinessOffset = -2f },
            new() { traitName = "personality.clever" },
            new() { traitName = "personality.lazy",          speedMult = 0.8f, sleepDecayMult = 1.3f },
            new() { traitName = "personality.loyal",         happinessOffset = 2f },
            new() { traitName = "personality.capricious",    hungerDecayMult = 1.15f, thirstDecayMult = 1.15f,
                                                              sleepDecayMult = 1.15f, playDecayMult = 1.15f, cleanDecayMult = 1.15f }
        };

        [Header("Quirky Traits (pool 3)")]
        [Range(0f, 1f)] public float quirkyChance = 0.33f;
        public CatTraitEffect[] quirkyPool =
        {
            new() { traitName = "quirk.grudge",          fightRecoveryMult = 0.4f },
            new() { traitName = "quirk.touchy",          fightPenaltyMult = 1.4f, happinessOffset = -2f },
            new() { traitName = "quirk.dumb",            speedMult = 0.9f },
            new() { traitName = "quirk.big_eater",       hungerDecayMult = 1.4f },
            new() { traitName = "quirk.drinker",         thirstDecayMult = 1.4f },
            new() { traitName = "quirk.food_thief",      hungerDecayMult = 0.7f },
            new() { traitName = "quirk.snorer",          sleepDecayMult = 0.8f },
            new() { traitName = "quirk.litter_fear",     cleanDecayMult = 1.3f },
            new() { traitName = "quirk.jealous",         fightPenaltyMult = 1.3f, happinessOffset = -3f },
            new() { traitName = "quirk.show_off",        petBonusMult = 1.3f },
            new() { traitName = "quirk.drama_queen",     fightPenaltyMult = 1.5f, petBonusMult = 1.4f },
            new() { traitName = "quirk.hair_collector" },
            new() { traitName = "quirk.grumpy",          happinessOffset = -5f },
            new() { traitName = "quirk.clumsy" },
            new() { traitName = "quirk.hypochondriac",   cleanDecayMult = 1.4f },
            new() { traitName = "quirk.couch_king",      sleepDecayMult = 1.3f, speedMult = 0.85f },
            new() { traitName = "quirk.snob",            happinessOffset = -3f, petBonusMult = 0.7f },
            new() { traitName = "quirk.pilferer",        hungerDecayMult = 0.8f, thirstDecayMult = 0.8f }
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

            // Resolve keys to localized text
            for (int i = 0; i < parts.Count; i++)
                parts[i] = ResolveTraitText(parts[i]);
            return (string.Join(", ", parts), mods);
        }

        // Keep backward compat
        public string GenerateDescription(CatBreedData breed, int seed)
        {
            return GeneratePersonality(breed, seed).description;
        }

        // Fallback: maps old French trait values (from un-updated assets) to localization keys
        private static readonly System.Collections.Generic.Dictionary<string, string> FrenchToKey = new()
        {
            // Race traits
            { "Gourmand", "trait.hunger" }, { "Assoiffé", "trait.thirst" },
            { "Dormeur", "trait.sleep" }, { "Joueur", "trait.play" },
            { "Maniaque", "trait.clean" }, { "Bagarreur", "trait.aggressive" },
            { "Imposant", "trait.big" }, { "Petit gabarit", "trait.small" },
            { "Rapide", "trait.fast" }, { "Nonchalant", "trait.slow" },
            // Personality
            { "Câlin", "personality.cuddly" }, { "Indépendant", "personality.independent" },
            { "Curieux", "personality.curious" }, { "Craintif", "personality.fearful" },
            { "Affectueux", "personality.affectionate" }, { "Observateur", "personality.observer" },
            { "Discret", "personality.discreet" }, { "Pot de colle", "personality.clingy" },
            { "Aventurier", "personality.adventurer" }, { "Territorial", "personality.territorial" },
            { "Sociable", "personality.sociable" }, { "Solitaire", "personality.solitary" },
            { "Malin", "personality.clever" }, { "Paresseux", "personality.lazy" },
            { "Fidèle", "personality.loyal" }, { "Capricieux", "personality.capricious" },
            // Quirky
            { "Rancunier", "quirk.grudge" }, { "Susceptible", "quirk.touchy" },
            { "Idiot", "quirk.dumb" }, { "Gros mangeur", "quirk.big_eater" },
            { "Soiffard", "quirk.drinker" }, { "Voleur de croquettes", "quirk.food_thief" },
            { "Ronfleur", "quirk.snorer" }, { "Peureux des litières", "quirk.litter_fear" },
            { "Jaloux", "quirk.jealous" }, { "Frimeur", "quirk.show_off" },
            { "Drama queen", "quirk.drama_queen" }, { "Collectionneur de poils", "quirk.hair_collector" },
            { "Grognon", "quirk.grumpy" }, { "Maladroit", "quirk.clumsy" },
            { "Hypocondriaque", "quirk.hypochondriac" }, { "Roi du canapé", "quirk.couch_king" },
            { "Snob", "quirk.snob" }, { "Chapardeur", "quirk.pilferer" },
        };

        /// <summary>
        /// Resolve a trait text: if it's a key (trait.play), localize it.
        /// If it's a French text from an old asset (Joueur), find the key first then localize.
        /// </summary>
        private static string ResolveTraitText(string text)
        {
            // Already a proper key? (contains a dot)
            if (text.Contains("."))
            {
                string resolved = LocalizedStrings.Get(text);
                if (!resolved.StartsWith("["))
                    return resolved;
            }

            // Old French value from un-updated asset → find key → localize
            if (FrenchToKey.TryGetValue(text, out var key))
                return LocalizedStrings.Get(key);

            // Unknown — return as-is
            return text;
        }
    }
}
