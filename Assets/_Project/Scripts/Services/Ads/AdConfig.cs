using UnityEngine;

namespace CatHotel.Services
{
    [CreateAssetMenu(fileName = "AdConfig", menuName = "Meowtel/Ad Config")]
    public class AdConfig : ScriptableObject
    {
        [Header("LevelPlay")]
        [Tooltip("App Key depuis le dashboard LevelPlay (= Game ID Android)")]
        public string appKey = "6065046";

        [Tooltip("Ad Unit ID pour le boost x2 revenus")]
        public string rewardedAdUnitId = "1rett8r2d58j6ob4";

        [Tooltip("Ad Unit ID pour le x2 gains fin de pension")]
        public string rewardedPensionAdUnitId = "ziumr8ervpf15qi4";

        [Header("Boost Settings")]
        [Tooltip("Multiplicateur de revenus apres visionnage")]
        public float boostMultiplier = 2f;

        [Tooltip("Duree du boost en secondes")]
        public float boostDuration = 60f;

        [Header("Daily Cap")]
        [Tooltip("Nombre max de rewarded ads par jour")]
        public int dailyCap = 10;

        [Header("Debug")]
        [Tooltip("Active le mode test (pas de vraies pubs, pas de revenu)")]
        public bool testMode = true;
    }
}
