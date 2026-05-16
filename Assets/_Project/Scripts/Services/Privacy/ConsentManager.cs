using UnityEngine;

namespace CatHotel.Services
{
    /// <summary>
    /// Stocke et expose le choix RGPD de l'utilisateur (PlayerPrefs).
    /// Lu par AdManager avant LevelPlay.Init().
    /// </summary>
    public static class ConsentManager
    {
        private const string ChoiceMadeKey = "GDPR_ChoiceMade";
        private const string ConsentKey = "GDPR_Consent";

        public static bool HasMadeChoice => PlayerPrefs.GetInt(ChoiceMadeKey, 0) == 1;
        public static bool ConsentGiven => PlayerPrefs.GetInt(ConsentKey, 0) == 1;

        public static void SetConsent(bool accepted)
        {
            PlayerPrefs.SetInt(ChoiceMadeKey, 1);
            PlayerPrefs.SetInt(ConsentKey, accepted ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log($"[Consent] User choice saved: {(accepted ? "ACCEPTED" : "REFUSED")}");
        }

        public static void ResetForDebug()
        {
            PlayerPrefs.DeleteKey(ChoiceMadeKey);
            PlayerPrefs.DeleteKey(ConsentKey);
            PlayerPrefs.Save();
            Debug.Log("[Consent] Choice reset");
        }
    }
}
