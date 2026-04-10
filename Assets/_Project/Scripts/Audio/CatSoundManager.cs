using System.Collections.Generic;
using UnityEngine;
using CatHotel.Cats;
using CatHotel.UI;

namespace CatHotel.Audio
{
    /// <summary>
    /// Centralized cat sound player. Singleton created by BootManager, persists across scenes.
    /// Handles eat/drink/litter action sounds and ambient + tap meows (neutral and sad).
    /// Respects ParametersPanel.EffectsVolume.
    /// </summary>
    public class CatSoundManager : MonoBehaviour
    {
        public static CatSoundManager Instance { get; private set; }

        private AudioClip[] _eatClips;
        private AudioClip[] _drinkClips;
        private AudioClip[] _litterClips;
        private AudioClip[] _meowNeutralClips;
        private AudioClip[] _meowSadClips;

        private AudioSource _source;

        // Ambient meow: one random cat meows every _ambientInterval seconds
        private const float AmbientMinInterval = 20f;
        private const float AmbientMaxInterval = 40f;
        private float _ambientTimer;

        // Cooldown to prevent meow spam (tap + ambient overlap)
        private float _meowCooldown;
        private const float MeowCooldownDuration = 3f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;

            LoadBank();
            ResetAmbientTimer();
        }

        private void LoadBank()
        {
            var bank = Resources.Load<CatSoundBank>("CatSoundBank");
            if (bank != null)
            {
                _eatClips = bank.eatClips;
                _drinkClips = bank.drinkClips;
                _litterClips = bank.litterClips;
                _meowNeutralClips = bank.meowNeutralClips;
                _meowSadClips = bank.meowSadClips;
                return;
            }

#if UNITY_EDITOR
            _eatClips = LoadEditorClips("Assets/_Project/Audio/SFX/Cats/Cat_Eat ST-{0}.ogg", 5);
            _drinkClips = LoadEditorClips("Assets/_Project/Audio/SFX/Cats/Cat_Drink ST-{0}.ogg", 3);
            _litterClips = LoadEditorClips("Assets/_Project/Audio/SFX/Cats/Cat_Litter-{0}.ogg", 2);
            _meowNeutralClips = LoadEditorClips("Assets/_Project/Audio/SFX/Cats/Meow_Neutral-{0}.ogg", 7);
            _meowSadClips = LoadEditorClips("Assets/_Project/Audio/SFX/Cats/Meow_Sad-{0}.ogg", 4);
#else
            Debug.LogError("[CatSoundManager] CatSoundBank not found in Resources/. " +
                "Create one via Cat Hotel > Audio > Create Cat Sound Bank.");
#endif
        }

#if UNITY_EDITOR
        private static AudioClip[] LoadEditorClips(string pathFormat, int count)
        {
            var clips = new AudioClip[count];
            for (int i = 0; i < count; i++)
                clips[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(
                    string.Format(pathFormat, (i + 1).ToString("D3")));
            return clips;
        }
#endif

        private void Update()
        {
            if (_meowCooldown > 0f)
                _meowCooldown -= Time.deltaTime;

            _ambientTimer -= Time.deltaTime;
            if (_ambientTimer <= 0f)
            {
                TryAmbientMeow();
                ResetAmbientTimer();
            }
        }

        private void ResetAmbientTimer()
        {
            _ambientTimer = Random.Range(AmbientMinInterval, AmbientMaxInterval);
        }

        private void TryAmbientMeow()
        {
            if (_meowCooldown > 0f) return;

            var spawner = FindAnyObjectByType<CatSpawner>();
            if (spawner == null || spawner.AllCats == null || spawner.AllCats.Count == 0) return;

            // Pick a random idle cat, then play neutral or sad meow based on happiness
            var candidates = new List<CatEntity>();
            foreach (var cat in spawner.AllCats)
            {
                if (cat != null && !cat.IsUsingObject)
                    candidates.Add(cat);
            }
            if (candidates.Count == 0) return;

            var chosen = candidates[Random.Range(0, candidates.Count)];
            var happiness = chosen.GetComponent<CatHappiness>();
            if (happiness != null && happiness.IsUnhappy)
                PlayMeowSad();
            else
                PlayMeow();
        }

        /// <summary>Play a random eat sound.</summary>
        public void PlayEat() => PlayRandom(_eatClips);

        /// <summary>Play a random drink sound.</summary>
        public void PlayDrink() => PlayRandom(_drinkClips);

        /// <summary>Play a random litter/clean sound.</summary>
        public void PlayLitter() => PlayRandom(_litterClips);

        /// <summary>Play a random neutral meow. Respects cooldown.</summary>
        public void PlayMeow()
        {
            if (_meowCooldown > 0f) return;
            PlayRandom(_meowNeutralClips);
            _meowCooldown = MeowCooldownDuration;
        }

        /// <summary>Play a random sad meow. Respects cooldown.</summary>
        public void PlayMeowSad()
        {
            if (_meowCooldown > 0f) return;
            PlayRandom(_meowSadClips);
            _meowCooldown = MeowCooldownDuration;
        }

        /// <summary>
        /// Play the appropriate meow for a cat based on happiness.
        /// Called on tap — picks sad if unhappy, neutral otherwise.
        /// </summary>
        public void PlayMeowForCat(CatEntity cat)
        {
            if (cat == null) { PlayMeow(); return; }
            var happiness = cat.GetComponent<CatHappiness>();
            if (happiness != null && happiness.IsUnhappy)
                PlayMeowSad();
            else
                PlayMeow();
        }

        private void PlayRandom(AudioClip[] clips)
        {
            if (_source == null || clips == null || clips.Length == 0) return;
            var clip = clips[Random.Range(0, clips.Length)];
            if (clip != null)
                _source.PlayOneShot(clip, ParametersPanel.EffectsVolume);
        }
    }
}
