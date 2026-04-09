using UnityEngine;

namespace CatHotel.Audio
{
    /// <summary>
    /// ScriptableObject holding cat sound clip references.
    /// Lives in Resources/ so CatSoundManager can load it at runtime in builds.
    /// Create via Cat Hotel > Audio > Create Cat Sound Bank.
    /// </summary>
    [CreateAssetMenu(fileName = "CatSoundBank", menuName = "Cat Hotel/Cat Sound Bank")]
    public class CatSoundBank : ScriptableObject
    {
        [Header("Eat (random variant)")]
        public AudioClip[] eatClips;

        [Header("Drink (random variant)")]
        public AudioClip[] drinkClips;

        [Header("Meow Neutral (random variant)")]
        public AudioClip[] meowNeutralClips;
    }
}
