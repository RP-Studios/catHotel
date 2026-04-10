using UnityEngine;
using UnityEditor;
using CatHotel.Audio;

namespace CatHotel.Editor
{
    public static class UISoundBankCreator
    {
        [MenuItem("Cat Hotel/Audio/Create UI Sound Bank", false, 2)]
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

            bank.tapNeutralClips = new AudioClip[5];
            bank.tapNeutralClips[0] = AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/_Project/Audio/SFX/UI/UI_TapNeutral.ogg");
            for (int i = 1; i <= 4; i++)
                bank.tapNeutralClips[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(
                    $"Assets/_Project/Audio/SFX/UI/UI_TapNeutral-{i:D3}.ogg");
            bank.tapNegativeClip = AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/_Project/Audio/SFX/UI/UI_TapNegative.ogg");
            bank.openSectionClip = AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/_Project/Audio/SFX/UI/UI_OpenSection.ogg");
            bank.closeSectionClip = AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/_Project/Audio/SFX/UI/UI_CloseSection.ogg");
        }
    }
}
