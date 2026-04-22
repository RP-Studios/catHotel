using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;

namespace CatHotel.Editor
{
    /// <summary>
    /// Fixes import settings for Lick spritesheets to match the rest of the cat animations:
    ///   - spriteMode: Multiple (for slicing)
    ///   - pixelsPerUnit: 200
    ///   - pivot: center (0.5, 0.5)
    ///   - filterMode: Point (pixel art)
    ///   - wrap: Clamp
    /// Also re-slices each sheet into equal horizontal frames based on the known frame
    /// counts (face/left/right). Run this once after dropping new Lick PNGs into the project.
    /// </summary>
    public static class LickSpriteImporter
    {
        // Root folders to scan for Lick subfolders
        private static readonly string[] SearchRoots =
        {
            "Assets/_Project/Art/Cats",
            "Assets/_Project/Art/SpecialCats",
        };

        // Frame counts per direction suffix. Key = substring in filename, Value = frame count.
        // Special cats override via the per-file overrides below.
        private const int DefaultFaceFrames = 18;
        private const int DefaultSideFrames = 16;

        // Per-file overrides (special cats have custom lengths).
        // Key = filename (without extension), Value = (frames).
        private static readonly Dictionary<string, int> FrameOverrides = new()
        {
            // Aristote
            { "europeen_aristote_lick_face",  36 },
            { "europeen_aristote_lick_left",  32 },
            { "europeen_aristote_lick_right", 32 },
            // Napoleon
            { "chartreu_napoleon_lick_face",  28 },
            { "chartreu_napoleon_lick_left",  20 },
            { "chartreu_napoleon_lick_right", 20 },
            // Orion
            { "ragdoll_orion_lick_face",  18 },
            { "ragdoll_orion_lick_left",  18 },
            { "ragdoll_orion_lick_right", 18 },
            // Cleo
            { "siamois_cleo_lick_face",  36 },
            { "siamois_cleo_lick_left",  20 },
            { "siamois_cleo_lick_right", 20 },
        };

        [MenuItem("Cat Hotel/Art/Fix Lick Sprite Imports", false, 20)]
        public static void FixLickImports()
        {
            var guids = AssetDatabase.FindAssets("t:Texture2D lick", SearchRoots);
            int processed = 0;
            int reSliced = 0;

            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (var guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!path.Contains("/Lick/")) continue;
                    if (!path.EndsWith(".png")) continue;

                    var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (importer == null) continue;

                    bool dirty = false;

                    if (importer.spriteImportMode != SpriteImportMode.Multiple)
                    {
                        importer.spriteImportMode = SpriteImportMode.Multiple;
                        dirty = true;
                    }
                    if (!Mathf.Approximately(importer.spritePixelsPerUnit, 200f))
                    {
                        importer.spritePixelsPerUnit = 200f;
                        dirty = true;
                    }
                    if (importer.filterMode != FilterMode.Point)
                    {
                        importer.filterMode = FilterMode.Point;
                        dirty = true;
                    }
                    if (importer.wrapMode != TextureWrapMode.Clamp)
                    {
                        importer.wrapMode = TextureWrapMode.Clamp;
                        dirty = true;
                    }
                    if (importer.mipmapEnabled)
                    {
                        importer.mipmapEnabled = false;
                        dirty = true;
                    }

                    if (dirty)
                    {
                        importer.SaveAndReimport();
                        processed++;
                    }

                    if (ReSliceSheet(path, importer))
                        reSliced++;
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }

            Debug.Log($"[LickImport] Done — updated import: {processed}, re-sliced: {reSliced}");
        }

        /// <summary>
        /// Slices the sheet into N equal horizontal frames with centered pivots.
        /// Uses the ISpriteEditorDataProvider API (same approach as ProtoSceneSetup).
        /// </summary>
        private static bool ReSliceSheet(string path, TextureImporter importer)
        {
            int frames = GetFrameCount(path);
            if (frames <= 0) return false;

            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null) return false;

            int width = tex.width;
            int height = tex.height;
            int frameW = width / frames;
            if (frameW <= 0) return false;

            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var provider = factory.GetSpriteEditorDataProviderFromObject(importer);
            if (provider == null) return false;

            provider.InitSpriteEditorDataProvider();

            string baseName = System.IO.Path.GetFileNameWithoutExtension(path);
            var rects = new List<SpriteRect>(frames);

            for (int i = 0; i < frames; i++)
            {
                rects.Add(new SpriteRect
                {
                    name = $"{baseName}_{i}",
                    spriteID = GUID.Generate(),
                    rect = new Rect(i * frameW, 0, frameW, height),
                    pivot = new Vector2(0.5f, 0.5f),
                    alignment = SpriteAlignment.Center,
                    border = Vector4.zero,
                });
            }

            provider.SetSpriteRects(rects.ToArray());

            var nameFileIdProvider = provider.GetDataProvider<ISpriteNameFileIdDataProvider>();
            if (nameFileIdProvider != null)
            {
                var pairs = new List<SpriteNameFileIdPair>(frames);
                foreach (var r in rects)
                    pairs.Add(new SpriteNameFileIdPair(r.name, r.spriteID));
                nameFileIdProvider.SetNameFileIdPairs(pairs);
            }

            provider.Apply();
            importer.SaveAndReimport();
            return true;
        }

        private static int GetFrameCount(string assetPath)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            if (FrameOverrides.TryGetValue(fileName, out int overrideCount))
                return overrideCount;

            if (fileName.EndsWith("_face"))
                return DefaultFaceFrames;
            if (fileName.EndsWith("_left") || fileName.EndsWith("_right"))
                return DefaultSideFrames;

            return 0;
        }
    }
}
