using UnityEngine;
using UnityEditor;

namespace CatHotel.Editor
{
    public static class TextureCompressionFixer
    {
        /// <summary>
        /// Full texture optimization pipeline for mobile:
        /// 1. Compression: ASTC 6x6, crunched, max 4096
        /// 2. Mipmaps OFF on all sprites (causes blur in 2D)
        /// 3. UI textures capped to 2048
        /// </summary>
        [MenuItem("Cat Hotel/Build/Optimize All Textures", false, 10)]
        public static void OptimizeAll()
        {
            FixCompression();
            FixMipmapsAndSizeCaps();

            Debug.Log("[Textures] Full optimization complete.");
        }

        /// <summary>
        /// Pass 1 — Compression: ASTC 6x6 + crunched for all sprites in Art/ and Animations/.
        /// </summary>
        private static void FixCompression()
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

                if (importer.textureCompression != TextureImporterCompression.Compressed)
                {
                    importer.textureCompression = TextureImporterCompression.Compressed;
                    dirty = true;
                }

                if (!importer.crunchedCompression)
                {
                    importer.crunchedCompression = true;
                    importer.compressionQuality = 50;
                    dirty = true;
                }

                if (importer.maxTextureSize > 4096)
                {
                    importer.maxTextureSize = 4096;
                    dirty = true;
                }

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
                    EditorUtility.DisplayProgressBar("Pass 1/2 — Compression",
                        $"{i}/{total} — {changed} fixed", (float)i / total);
            }

            EditorUtility.ClearProgressBar();
            Debug.Log($"[Textures] Pass 1 — Compression: {changed}/{total} fixed " +
                      "(ASTC 6x6, crunched, max 4096).");
        }

        /// <summary>
        /// Pass 2 — Disable mipmaps on all sprites, cap UI to 2048.
        /// </summary>
        private static void FixMipmapsAndSizeCaps()
        {
            // Sprites: disable mipmaps (2D ortho = always viewed at native size)
            string[] spriteGuids = AssetDatabase.FindAssets("t:Texture2D", new[]
            {
                "Assets/_Project/Art/Cats",
                "Assets/_Project/Art/SpecialCats",
                "Assets/_Project/Art/Objects",
                "Assets/_Project/Art/Environment"
            });

            int changed = 0;
            int total = spriteGuids.Length;

            for (int i = 0; i < total; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(spriteGuids[i]);
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

                if (dirty)
                {
                    importer.SaveAndReimport();
                    changed++;
                }

                if (i % 50 == 0)
                    EditorUtility.DisplayProgressBar("Pass 2/2 — Mipmaps & Size Caps",
                        $"Sprites: {i}/{total}", (float)i / total);
            }

            // UI textures: disable mipmaps + cap to 2048
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

            EditorUtility.ClearProgressBar();
            Debug.Log($"[Textures] Pass 2 — Mipmaps OFF: {changed} sprites, {uiChanged} UI " +
                      "(UI capped 2048).");
        }
    }
}
