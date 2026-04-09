using UnityEngine;

namespace CatHotel.Audio
{
    /// <summary>
    /// ScriptableObject holding UI sound clip references.
    /// Lives in Resources/ so UISoundManager can load it at runtime in builds.
    /// Create via Assets > Create > Cat Hotel > UI Sound Bank.
    /// </summary>
    [CreateAssetMenu(fileName = "UISoundBank", menuName = "Cat Hotel/UI Sound Bank")]
    public class UISoundBank : ScriptableObject
    {
        [Header("Tap Positive (random variant)")]
        public AudioClip[] tapPositiveClips;

        [Header("Tap Neutral (random variant)")]
        public AudioClip[] tapNeutralClips;

        [Header("Tap Negative")]
        public AudioClip tapNegativeClip;

        [Header("Panel Open/Close")]
        public AudioClip openSectionClip;
        public AudioClip closeSectionClip;
    }
}
