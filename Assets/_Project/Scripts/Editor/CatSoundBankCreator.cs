using UnityEngine;
using UnityEditor;
using CatHotel.Audio;

namespace CatHotel.Editor
{
    public static class CatSoundBankCreator
    {
        [MenuItem("Cat Hotel/Audio/Create Cat Sound Bank", false, 3)]
        public static void CreateBank()
        {
            const string assetPath = "Assets/_Project/Resources/CatSoundBank.asset";

            var existing = AssetDatabase.LoadAssetAtPath<CatSoundBank>(assetPath);
            if (existing != null)
            {
                Debug.Log("[CatSoundBank] Already exists, updating clip references.");
                AssignClips(existing);
                EditorUtility.SetDirty(existing);
                AssetDatabase.SaveAssets();
                EditorGUIUtility.PingObject(existing);
                return;
            }

            if (!AssetDatabase.IsValidFolder("Assets/_Project/Resources"))
                AssetDatabase.CreateFolder("Assets/_Project", "Resources");

            var bank = ScriptableObject.CreateInstance<CatSoundBank>();
            AssignClips(bank);

            AssetDatabase.CreateAsset(bank, assetPath);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(bank);
            Debug.Log($"[CatSoundBank] Created at {assetPath}");
        }

        private static void AssignClips(CatSoundBank bank)
        {
            bank.eatClips = new AudioClip[5];
            for (int i = 0; i < 5; i++)
                bank.eatClips[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(
                    $"Assets/_Project/Audio/SFX/Cats/Cat_Eat ST-{(i + 1):D3}.ogg");

            bank.drinkClips = new AudioClip[3];
            for (int i = 0; i < 3; i++)
                bank.drinkClips[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(
                    $"Assets/_Project/Audio/SFX/Cats/Cat_Drink ST-{(i + 1):D3}.ogg");

            bank.litterClips = new AudioClip[2];
            for (int i = 0; i < 2; i++)
                bank.litterClips[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(
                    $"Assets/_Project/Audio/SFX/Cats/Cat_Litter-{(i + 1):D3}.ogg");

            bank.meowNeutralClips = new AudioClip[7];
            for (int i = 0; i < 7; i++)
                bank.meowNeutralClips[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(
                    $"Assets/_Project/Audio/SFX/Cats/Meow_Neutral-{(i + 1):D3}.ogg");

            bank.meowSadClips = new AudioClip[4];
            for (int i = 0; i < 4; i++)
                bank.meowSadClips[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(
                    $"Assets/_Project/Audio/SFX/Cats/Meow_Sad-{(i + 1):D3}.ogg");
        }
    }
}
