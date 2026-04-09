using System.Collections.Generic;
using UnityEngine;
using CatHotel.Cats;
using CatHotel.UI;

namespace CatHotel.Audio
{
    /// <summary>
    /// Centralized cat sound player. Singleton created by BootManager, persists across scenes.
    /// Handles eat/drink action sounds and ambient + tap meows.
    /// Respects ParametersPanel.EffectsVolume.
    /// </summary>
    public class CatSoundManager : MonoBehaviour
    {
        public static CatSoundManager Instance { get; private set; }

        private AudioClip[] _eatClips;
        private AudioClip[] _drinkClips;
        private AudioClip[] _meowClips;

        private AudioSource _source;

        // Ambient meow: one random cat meows every _ambientInterval seconds
        [Header("Ambient Meow")]
        private float _ambientMinInterval = 20f;
        private float _ambientMaxInterval = 40f;
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
                _meowClips = bank.meowNeutralClips;
                return;
            }

#if UNITY_EDITOR
            _eatClips = new AudioClip[5];
            for (int i = 0; i < 5; i++)
                _eatClips[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(
                    $"Assets/_Project/Audio/SFX/Cats/Cat_Eat ST-{(i + 1):D3}.ogg");

            _drinkClips = new AudioClip[3];
            for (int i = 0; i < 3; i++)
                _drinkClips[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(
                    $"Assets/_Project/Audio/SFX/Cats/Cat_Drink ST-{(i + 1):D3}.ogg");

            _meowClips = new AudioClip[7];
            for (int i = 0; i < 7; i++)
                _meowClips[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(
                    $"Assets/_Project/Audio/SFX/Cats/Meow_Neutral-{(i + 1):D3}.ogg");
#else
            Debug.LogError("[CatSoundManager] CatSoundBank not found in Resources/. " +
                "Create one via Cat Hotel > Audio > Create Cat Sound Bank.");
#endif
        }

        private void Update()
        {
            if (_meowCooldown > 0f)
                _meowCooldown -= Time.deltaTime;

            // Ambient meow timer
            _ambientTimer -= Time.deltaTime;
            if (_ambientTimer <= 0f)
            {
                TryAmbientMeow();
                ResetAmbientTimer();
            }
        }

        private void ResetAmbientTimer()
        {
            _ambientTimer = Random.Range(_ambientMinInterval, _ambientMaxInterval);
        }

        private void TryAmbientMeow()
        {
            if (_meowCooldown > 0f) return;

            // Find all cats in scene
            var spawner = FindAnyObjectByType<CatSpawner>();
            if (spawner == null || spawner.AllCats == null || spawner.AllCats.Count == 0) return;

            // Pick a random cat that is idle (not using object / not in combat)
            var candidates = new List<CatEntity>();
            foreach (var cat in spawner.AllCats)
            {
                if (cat != null && !cat.IsUsingObject)
                    candidates.Add(cat);
            }
            if (candidates.Count == 0) return;

            PlayMeow();
        }

        /// <summary>Play a random eat sound.</summary>
        public void PlayEat()
        {
            PlayRandom(_eatClips);
        }

        /// <summary>Play a random drink sound.</summary>
        public void PlayDrink()
        {
            PlayRandom(_drinkClips);
        }

        /// <summary>Play a random meow. Respects cooldown to avoid cacophony.</summary>
        public void PlayMeow()
        {
            if (_meowCooldown > 0f) return;
            PlayRandom(_meowClips);
            _meowCooldown = MeowCooldownDuration;
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
