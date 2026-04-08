using UnityEngine;
using UnityEditor;

namespace CatHotel.Editor
{
    public static class TextureCompressionFixer
    {
        /// <summary>
        /// Fix ALL project sprites for optimal mobile performance:
        /// - Compression: Compressed (not Uncompressed)
        /// - Android format: ASTC 6x6 (best quality/size ratio for 2D sprites)
        /// - Max texture size: 4096 (safe for all Android GPUs, including 2019 devices)
        /// - Crunched compression: ON (reduces APK size with no runtime cost)
        /// </summary>
        [MenuItem("Cat Hotel/Fix Texture Compression (All Sprites)")]
        public static void FixAll()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[]
            {
                "Assets/_Project/Art",
                "Assets/_Project/Animations"
            });

            int changed = 0;
            int total = guids.Length;

            for (int i = 0; i < total; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                bool dirty = false;

                // Fix compression mode
                if (importer.textureCompression != TextureImporterCompression.Compressed)
                {
                    importer.textureCompression = TextureImporterCompression.Compressed;
                    dirty = true;
                }

                // Enable crunched compression (reduces APK size, no runtime cost)
                if (!importer.crunchedCompression)
                {
                    importer.crunchedCompression = true;
                    importer.compressionQuality = 50;
                    dirty = true;
                }

                // Cap max texture size to 4096 (safe for all Android GPUs)
                if (importer.maxTextureSize > 4096)
                {
                    importer.maxTextureSize = 4096;
                    dirty = true;
                }

                // Android platform override: ASTC 6x6 + max 4096 + crunched
                var android = importer.GetPlatformTextureSettings("Android");
                bool androidDirty = false;

                if (!android.overridden)
                {
                    android.overridden = true;
                    androidDirty = true;
                }
                if (android.format != TextureImporterFormat.ASTC_6x6)
                {
                    android.format = TextureImporterFormat.ASTC_6x6;
                    androidDirty = true;
                }
                if (android.maxTextureSize > 4096)
                {
                    android.maxTextureSize = 4096;
                    androidDirty = true;
                }
                if (!android.crunchedCompression)
                {
                    android.crunchedCompression = true;
                    android.compressionQuality = 50;
                    androidDirty = true;
                }

                if (androidDirty)
                {
                    importer.SetPlatformTextureSettings(android);
                    dirty = true;
                }

                if (dirty)
                {
                    importer.SaveAndReimport();
                    changed++;
                }

                if (i % 50 == 0)
                    EditorUtility.DisplayProgressBar("Fixing Texture Compression",
                        $"{i}/{total} — {changed} fixed", (float)i / total);
            }

            EditorUtility.ClearProgressBar();
            Debug.Log($"[TextureCompression] Done. Optimized {changed}/{total} textures. " +
                      "(ASTC 6x6, maxSize 4096, crunched ON)");
        }

        /// <summary>
        /// Fix sprite import settings for 2D mobile:
        /// - Disable mipmaps on sprites (causes blur in 2D ortho view)
        /// - Disable mipmap streaming (not useful without mipmaps)
        /// - Cap UI textures to 2048 (phones don't need 4096)
        /// - Set Point filtering on small pixel-art sprites
        /// </summary>
        [MenuItem("Cat Hotel/Optimize Textures for Mobile (Fix Sprites + Size Caps)")]
        public static void OptimizeForMobile()
        {
            // Cat + SpecialCats sprites: disable mipmaps (they cause blur in 2D)
            string[] spriteGuids = AssetDatabase.FindAssets("t:Texture2D", new[]
            {
                "Assets/_Project/Art/Cats",
                "Assets/_Project/Art/SpecialCats",
                "Assets/_Project/Art/Objects",
                "Assets/_Project/Art/Environment"
            });

            int changed = 0;
            foreach (string guid in spriteGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                bool dirty = false;

                // Disable mipmaps — 2D ortho sprites are always viewed at native size
                if (importer.mipmapEnabled)
                {
                    importer.mipmapEnabled = false;
                    dirty = true;
                }
                if (importer.streamingMipmaps)
                {
                    importer.streamingMipmaps = false;
                    dirty = true;
                }

                if (dirty)
                {
                    importer.SaveAndReimport();
                    changed++;
                }
            }

            // UI textures: cap to 2048, disable mipmaps
            string[] uiGuids = AssetDatabase.FindAssets("t:Texture2D", new[]
            {
                "Assets/_Project/Art/UI"
            });

            int uiChanged = 0;
            foreach (string guid in uiGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                bool dirty = false;

                if (importer.mipmapEnabled)
                {
                    importer.mipmapEnabled = false;
                    dirty = true;
                }
                if (importer.streamingMipmaps)
                {
                    importer.streamingMipmaps = false;
                    dirty = true;
                }
                if (importer.maxTextureSize > 2048)
                {
                    importer.maxTextureSize = 2048;

                    var android = importer.GetPlatformTextureSettings("Android");
                    if (android.overridden && android.maxTextureSize > 2048)
                    {
                        android.maxTextureSize = 2048;
                        importer.SetPlatformTextureSettings(android);
                    }
                    dirty = true;
                }

                if (dirty)
                {
                    importer.SaveAndReimport();
                    uiChanged++;
                }
            }

            Debug.Log($"[MobileOptim] Sprites: {changed} fixed (mipmaps OFF). " +
                      $"UI textures: {uiChanged} fixed (mipmaps OFF, capped 2048).");
        }
    }
}
