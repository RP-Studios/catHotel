using UnityEngine;
using UnityEditor;

namespace CatHotel.Editor
{
    public static class TextureCompressionFixer
    {
        [MenuItem("Cat Hotel/Fix Texture Compression (All Sprites)")]
        public static void FixAll()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[]
            {
                "Assets/_Project/Art"
            });

            int changed = 0;
            int total = guids.Length;

            for (int i = 0; i < total; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                bool dirty = false;

                // Fix compression: Uncompressed → Compressed
                if (importer.textureCompression == TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.Compressed;
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
            Debug.Log($"[TextureCompression] Done. Fixed {changed}/{total} textures.");
        }
    }
}
