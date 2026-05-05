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

        [Header("Litter / Clean (random variant)")]
        public AudioClip[] litterClips;

        [Header("Meow Neutral (random variant)")]
        public AudioClip[] meowNeutralClips;

        [Header("Meow Sad (random variant)")]
        public AudioClip[] meowSadClips;

        [Header("Cat arrival (pension entrance)")]
        public AudioClip[] arrivalClips;

        [Header("Cat shelter arrival")]
        public AudioClip[] shelterArrivalClips;

        [Header("Cat departure (happy / pension end)")]
        public AudioClip[] departureClips;

        [Header("Cat escape (unhappy departure)")]
        public AudioClip[] escapeClips;

        [Header("Cat purring (petting)")]
        public AudioClip[] purringClips;

        [Header("Item drop (placement)")]
        public AudioClip[] itemDropClips;

        [Header("Item delete (sell)")]
        public AudioClip[] itemDeleteClips;
    }
}
