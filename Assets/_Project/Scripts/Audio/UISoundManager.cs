using UnityEngine;
using CatHotel.UI;

namespace CatHotel.Audio
{
    public enum UISoundType
    {
        None,
        TapPositive,
        TapNeutral,
        TapNegative,
        OpenSection,
        CloseSection,
    }

    /// <summary>
    /// Centralized UI sound player. Singleton created by BootManager, persists across scenes.
    /// Loads clips from a UISoundBank ScriptableObject in Resources/.
    /// Respects ParametersPanel.EffectsVolume.
    /// </summary>
    public class UISoundManager : MonoBehaviour
    {
        public static UISoundManager Instance { get; private set; }

        private AudioClip[] _tapPositiveClips;
        private AudioClip[] _tapNeutralClips;
        private AudioClip _tapNegativeClip;
        private AudioClip _openSectionClip;
        private AudioClip _closeSectionClip;

        private AudioSource _source;

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
        }

        private void LoadBank()
        {
            var bank = Resources.Load<UISoundBank>("UISoundBank");
            if (bank != null)
            {
                _tapPositiveClips = bank.tapPositiveClips;
                _tapNeutralClips = bank.tapNeutralClips;
                _tapNegativeClip = bank.tapNegativeClip;
                _openSectionClip = bank.openSectionClip;
                _closeSectionClip = bank.closeSectionClip;
                return;
            }

            // Editor fallback: load clips directly by path
#if UNITY_EDITOR
            _tapPositiveClips = new AudioClip[6];
            for (int i = 0; i < 6; i++)
                _tapPositiveClips[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(
                    $"Assets/_Project/Audio/SFX/UI/UI_TapPositive-{(i + 1):D3}.ogg");

            _tapNeutralClips = new AudioClip[5];
            _tapNeutralClips[0] = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/_Project/Audio/SFX/UI/UI_TapNeutral.ogg");
            for (int i = 1; i <= 4; i++)
                _tapNeutralClips[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(
                    $"Assets/_Project/Audio/SFX/UI/UI_TapNeutral-{i:D3}.ogg");

            _tapNegativeClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/_Project/Audio/SFX/UI/UI_TapNegative.ogg");
            _openSectionClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/_Project/Audio/SFX/UI/UI_OpenSection.ogg");
            _closeSectionClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/_Project/Audio/SFX/UI/UI_CloseSection.ogg");
#else
            Debug.LogError("[UISoundManager] UISoundBank not found in Resources/. " +
                "Create one via Assets > Create > Cat Hotel > UI Sound Bank and place it in Resources/.");
#endif
        }

        /// <summary>Play a UI sound by type. Respects EffectsVolume from settings.</summary>
        public void Play(UISoundType type)
        {
            if (_source == null) return;

            var clip = GetClip(type);
            if (clip == null) return;

            _source.PlayOneShot(clip, ParametersPanel.EffectsVolume);
        }

        public void PlayTapPositive() => Play(UISoundType.TapPositive);
        public void PlayTapNeutral() => Play(UISoundType.TapNeutral);
        public void PlayTapNegative() => Play(UISoundType.TapNegative);
        public void PlayOpenSection() => Play(UISoundType.OpenSection);
        public void PlayCloseSection() => Play(UISoundType.CloseSection);

        private AudioClip GetClip(UISoundType type)
        {
            switch (type)
            {
                case UISoundType.TapPositive:
                    return PickRandom(_tapPositiveClips);
                case UISoundType.TapNeutral:
                    return PickRandom(_tapNeutralClips);
                case UISoundType.TapNegative:
                    return _tapNegativeClip;
                case UISoundType.OpenSection:
                    return _openSectionClip;
                case UISoundType.CloseSection:
                    return _closeSectionClip;
                default:
                    return null;
            }
        }

        private static AudioClip PickRandom(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0) return null;
            return clips[Random.Range(0, clips.Length)];
        }
    }
}
