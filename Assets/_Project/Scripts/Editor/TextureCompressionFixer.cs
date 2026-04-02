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
    }
}
