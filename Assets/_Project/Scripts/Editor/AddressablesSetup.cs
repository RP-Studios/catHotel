using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System.IO;

namespace CatHotel.Editor
{
    /// <summary>
    /// One-click Addressables setup for Meowtel.
    /// Marks CatBreedData assets as addressable — their referenced sprites and
    /// AnimatorControllers are automatically pulled into the same bundle.
    /// DO NOT mark individual sprites separately (causes duplication).
    /// Run via Cat Hotel > Setup Addressables.
    /// </summary>
    public static class AddressablesSetup
    {
        [MenuItem("Cat Hotel/Build/Setup Addressables", false, 1)]
        public static void Setup()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
                Debug.Log("[Addressables] Created AddressableAssetSettings");
            }

            // Single group for all breed assets (each breed SO + its sprites + animators)
            var breedsGroup = FindOrCreateGroup(settings, "Breeds");
            ConfigureGroupForLocal(breedsGroup);

            // Mark each CatBreedData asset as addressable
            // Address = filename without extension (e.g. "Breed_Europeen")
            string[] guids = AssetDatabase.FindAssets("t:CatBreedData",
                new[] { "Assets/_Project/Data/Breeds" });

            int count = 0;
            foreach (string guid in guids)
            {
                var entry = settings.FindAssetEntry(guid);
                if (entry == null)
                {
                    entry = settings.CreateOrMoveEntry(guid, breedsGroup,
                        readOnly: false, postEvent: false);
                    count++;
                }
                else
                {
                    // Move to correct group if needed
                    settings.CreateOrMoveEntry(guid, breedsGroup,
                        readOnly: false, postEvent: false);
                }

                string path = AssetDatabase.GUIDToAssetPath(guid);
                entry.address = Path.GetFileNameWithoutExtension(path);
            }

            settings.SetDirty(
                AddressableAssetSettings.ModificationEvent.EntryMoved, null, true, true);
            AssetDatabase.SaveAssets();

            Debug.Log($"[Addressables] Setup complete. {guids.Length} breeds marked " +
                      $"({count} new). Group: Breeds (LZ4, local).\n" +
                      "Sprites and AnimatorControllers are auto-included via breed references.\n" +
                      "Build bundles: Window > Asset Management > Addressables > Build > New Build");
        }

        /// <summary>
        /// Verify that no individual sprites are marked as addressable (causes duplication).
        /// Run if build size seems too large.
        /// </summary>
        [MenuItem("Cat Hotel/Build/Check Addressables Duplicates", false, 2)]
        public static void CheckDuplicates()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.Log("[Addressables] No settings found.");
                return;
            }

            int spriteCount = 0;
            int animCount = 0;

            foreach (var group in settings.groups)
            {
                if (group == null) continue;
                foreach (var entry in group.entries)
                {
                    string path = AssetDatabase.GUIDToAssetPath(entry.guid);
                    if (path.EndsWith(".png") || path.EndsWith(".jpg"))
                    {
                        Debug.LogWarning($"[Addressables] Sprite marked individually: {path} " +
                                         "— remove it, sprites should be pulled in via breed SOs");
                        spriteCount++;
                    }
                    else if (path.EndsWith(".controller") || path.EndsWith(".overrideController"))
                    {
                        Debug.LogWarning($"[Addressables] Animator marked individually: {path} " +
                                         "— remove it, animators should be pulled in via breed SOs");
                        animCount++;
                    }
                }
            }

            if (spriteCount == 0 && animCount == 0)
                Debug.Log("[Addressables] No duplicates found. Build size should be optimal.");
            else
                Debug.LogError($"[Addressables] Found {spriteCount} sprites + {animCount} animators " +
                               "marked individually. Remove them to avoid duplication!");
        }

        private static AddressableAssetGroup FindOrCreateGroup(
            AddressableAssetSettings settings, string groupName)
        {
            var group = settings.FindGroup(groupName);
            if (group != null) return group;

            group = settings.CreateGroup(groupName, false, false, false, null,
                typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            Debug.Log($"[Addressables] Created group: {groupName}");
            return group;
        }

        private static void ConfigureGroupForLocal(AddressableAssetGroup group)
        {
            var schema = group.GetSchema<BundledAssetGroupSchema>();
            if (schema == null) return;

            schema.BuildPath.SetVariableByName(group.Settings, "LocalBuildPath");
            schema.LoadPath.SetVariableByName(group.Settings, "LocalLoadPath");
            schema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            schema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
        }
    }
}
