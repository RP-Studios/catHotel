using UnityEngine;
using UnityEditor;
using CatHotel.Audio;

namespace CatHotel.Editor
{
    public static class UISoundBankCreator
    {
        [MenuItem("Cat Hotel/Create UI Sound Bank", false, 30)]
        public static void CreateBank()
        {
            const string assetPath = "Assets/_Project/Resources/UISoundBank.asset";

            var existing = AssetDatabase.LoadAssetAtPath<UISoundBank>(assetPath);
            if (existing != null)
            {
                Debug.Log("[UISoundBank] Already exists, updating clip references.");
                AssignClips(existing);
                EditorUtility.SetDirty(existing);
                AssetDatabase.SaveAssets();
                EditorGUIUtility.PingObject(existing);
                return;
            }

            // Ensure Resources folder exists
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Resources"))
                AssetDatabase.CreateFolder("Assets/_Project", "Resources");

            var bank = ScriptableObject.CreateInstance<UISoundBank>();
            AssignClips(bank);

            AssetDatabase.CreateAsset(bank, assetPath);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(bank);
            Debug.Log($"[UISoundBank] Created at {assetPath}");
        }

        private static void AssignClips(UISoundBank bank)
        {
            bank.tapPositiveClips = new AudioClip[6];
            for (int i = 0; i < 6; i++)
                bank.tapPositiveClips[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(
                    $"Assets/_Project/Audio/SFX/UI/UI_TapPositive-{(i + 1):D3}.ogg");

            bank.tapNeutralClip = AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/_Project/Audio/SFX/UI/UI_TapNeutral.ogg");
            bank.tapNegativeClip = AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/_Project/Audio/SFX/UI/UI_TapNegative.ogg");
        }
    }
}
