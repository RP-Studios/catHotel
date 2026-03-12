using UnityEngine;

namespace CatHotel.Services
{
    [CreateAssetMenu(fileName = "AuthConfig", menuName = "Meowtel/Auth Config")]
    public class AuthConfig : ScriptableObject
    {
        [Header("Google Play Games")]
        [Tooltip("OAuth Web Client ID depuis Google Cloud Console")]
        public string webClientId;

        [Header("UGS")]
        public string environmentName = "production";
    }
}
