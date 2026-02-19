using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.Animations;
using CatHotel.Grid;
using CatHotel.Input;
using CatHotel.Cats;
using CatHotel.UI;

namespace CatHotel.Editor
{
    public static class ProtoSceneSetup
    {
        private const string SpritesRoot = "Assets/_Project/Art/Environment";
        private const string CatSpritesRoot = "Assets/_Project/Art/Cats/Europeen";
        private const string AnimRoot = CatSpritesRoot + "/Animations";

        // Walk spritesheets (8 frames)
        private const string WalkFrontSheet = AnimRoot + "/base_walk_face.png";
        private const string WalkBackSheet  = AnimRoot + "/base_walk_back.png";

        // Idle3 spritesheets (8 frames)
        private const string Idle3FrontSheet = AnimRoot + "/base_idle3_face.png";
        private const string Idle3BackSheet  = AnimRoot + "/base_idle3_back.png";
        private const string Idle3LeftSheet  = AnimRoot + "/base_idle3_left.png";
        private const string Idle3RightSheet = AnimRoot + "/base_idle3_right.png";

        // Idle2 spritesheets (6 frames)
        private const string Idle2FrontSheet = AnimRoot + "/base_idle2_face.png";
        private const string Idle2LeftSheet  = AnimRoot + "/base_idle2_left.png";
        private const string Idle2RightSheet = AnimRoot + "/base_idle2_right.png";

        private const string CatControllerPath = AnimRoot + "/CatEuropeen.controller";

        private const int WalkFrameCount  = 8;
        private const float WalkFPS       = 12f;
        private const int Idle3FrameCount = 8;
        private const float Idle3FPS      = 8f;
        private const int Idle2FrameCount = 6;
        private const float Idle2FPS      = 6f;

        [MenuItem("Cat Hotel/Setup Proto Scene")]
        public static void SetupScene()
        {
            ConfigureSpriteImports();
            ConfigureCatSpriteImports();
            ConfigureSpritesheet(WalkFrontSheet, "walk_face", WalkFrameCount);
            ConfigureSpritesheet(WalkBackSheet, "walk_back", WalkFrameCount);
            ConfigureSpritesheet(Idle3FrontSheet, "idle3_face", Idle3FrameCount);
            ConfigureSpritesheet(Idle3BackSheet, "idle3_back", Idle3FrameCount);
            ConfigureSpritesheet(Idle3LeftSheet, "idle3_left", Idle3FrameCount);
            ConfigureSpritesheet(Idle3RightSheet, "idle3_right", Idle3FrameCount);
            ConfigureSpritesheet(Idle2FrontSheet, "idle2_face", Idle2FrameCount);
            ConfigureSpritesheet(Idle2LeftSheet, "idle2_left", Idle2FrameCount);
            ConfigureSpritesheet(Idle2RightSheet, "idle2_right", Idle2FrameCount);
            AssetDatabase.Refresh();
            var tiles = CreateTileAssets();
            var catController = CreateCatAnimationAssets();
            BuildSceneHierarchy(tiles, catController);
            Debug.Log("Proto scene setup complete. " +
                "Controls: Pan = drag, B = toggle build mode, " +
                "Scroll = zoom, UI buttons = Build/Spawn/Zoom");
        }

        private static void ConfigureSpriteImports()
        {
            string[] paths =
            {
                $"{SpritesRoot}/Tiles/tile_empty.png",
                $"{SpritesRoot}/Floors/tile_floor.png",
                $"{SpritesRoot}/Walls/tile_wall.png"
            };

            foreach (string path in paths)
                ConfigureSprite(path, 32, FilterMode.Point);
        }

        private static void ConfigureCatSpriteImports()
        {
            string[] paths =
            {
                $"{CatSpritesRoot}/CAT_EUR_FRONT.png",
                $"{CatSpritesRoot}/CAT_EUR_RIGHT.png",
                $"{CatSpritesRoot}/CAT_EUR_BACK.png"
            };

            foreach (string path in paths)
                ConfigureSprite(path, 200, FilterMode.Bilinear);
        }

        private static void ConfigureSpritesheet(string sheetPath, string namePrefix, int frameCount)
        {
            var importer = AssetImporter.GetAtPath(sheetPath) as TextureImporter;
            if (importer == null)
            {
                Debug.LogWarning($"Spritesheet not found: {sheetPath}");
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = 200;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            importer.GetSourceTextureWidthAndHeight(out int texW, out int texH);
            int frameW = texW / frameCount;

            var metas = new SpriteMetaData[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                metas[i] = new SpriteMetaData
                {
                    name = $"{namePrefix}_{i}",
                    rect = new Rect(i * frameW, 0, frameW, texH),
                    alignment = (int)SpriteAlignment.Center,
                    pivot = new Vector2(0.5f, 0.5f)
                };
            }
            importer.spritesheet = metas;
            importer.SaveAndReimport();
        }

        private static RuntimeAnimatorController CreateCatAnimationAssets()
        {
            // Walk clips
            var walkFront  = CreateAnimClip(WalkFrontSheet, AnimRoot + "/Walk_Front.anim", "Walk_Front", WalkFrameCount, WalkFPS);
            var walkBack   = CreateAnimClip(WalkBackSheet, AnimRoot + "/Walk_Back.anim", "Walk_Back", WalkFrameCount, WalkFPS);

            // Idle3 clips (8 frames)
            var idle3Front = CreateAnimClip(Idle3FrontSheet, AnimRoot + "/Idle3_Front.anim", "Idle3_Front", Idle3FrameCount, Idle3FPS);
            var idle3Back  = CreateAnimClip(Idle3BackSheet, AnimRoot + "/Idle3_Back.anim", "Idle3_Back", Idle3FrameCount, Idle3FPS);
            var idle3Left  = CreateAnimClip(Idle3LeftSheet, AnimRoot + "/Idle3_Left.anim", "Idle3_Left", Idle3FrameCount, Idle3FPS);
            var idle3Right = CreateAnimClip(Idle3RightSheet, AnimRoot + "/Idle3_Right.anim", "Idle3_Right", Idle3FrameCount, Idle3FPS);

            // Idle2 clips (6 frames)
            var idle2Front = CreateAnimClip(Idle2FrontSheet, AnimRoot + "/Idle2_Front.anim", "Idle2_Front", Idle2FrameCount, Idle2FPS);
            var idle2Left  = CreateAnimClip(Idle2LeftSheet, AnimRoot + "/Idle2_Left.anim", "Idle2_Left", Idle2FrameCount, Idle2FPS);
            var idle2Right = CreateAnimClip(Idle2RightSheet, AnimRoot + "/Idle2_Right.anim", "Idle2_Right", Idle2FrameCount, Idle2FPS);

            // Clean old clips
            AssetDatabase.DeleteAsset(AnimRoot + "/Idle_Front.anim");
            AssetDatabase.DeleteAsset(AnimRoot + "/Idle_Left.anim");
            AssetDatabase.DeleteAsset(AnimRoot + "/Idle_Right.anim");

            // --- Create AnimatorController ---
            AssetDatabase.DeleteAsset(CatControllerPath);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(CatControllerPath);
            var rootSM = controller.layers[0].stateMachine;

            void AddState(string name, AnimationClip clip, bool isDefault = false)
            {
                if (clip == null) return;
                var state = rootSM.AddState(name);
                state.motion = clip;
                if (isDefault) rootSM.defaultState = state;
            }

            AddState("Idle3_Front", idle3Front, true);
            AddState("Idle3_Back",  idle3Back);
            AddState("Idle3_Right", idle3Right);
            AddState("Idle3_Left",  idle3Left);
            AddState("Idle2_Front", idle2Front);
            AddState("Idle2_Right", idle2Right);
            AddState("Idle2_Left",  idle2Left);
            AddState("Walk_Front",  walkFront);
            AddState("Walk_Back",   walkBack);

            AssetDatabase.SaveAssets();
            Debug.Log("[ProtoSceneSetup] Created cat AnimatorController (9 states: 7 idle + 2 walk)");
            return controller;
        }

        private static AnimationClip CreateAnimClip(
            string sheetPath, string clipPath, string clipName,
            int frameCount, float fps)
        {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(sheetPath)
                .OfType<Sprite>()
                .OrderBy(s => s.name)
                .ToList();

            if (sprites.Count < frameCount)
            {
                Debug.LogWarning($"{clipName}: expected {frameCount} sprites in {sheetPath}, got {sprites.Count}. Skipping.");
                return null;
            }

            var clip = new AnimationClip { frameRate = fps };

            var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
            var keyframes = new ObjectReferenceKeyframe[frameCount + 1];
            for (int i = 0; i <= frameCount; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe
                {
                    time = i / fps,
                    value = sprites[i % frameCount]
                };
            }
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

            var clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            clipSettings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, clipSettings);

            AssetDatabase.DeleteAsset(clipPath);
            AssetDatabase.CreateAsset(clip, clipPath);
            return clip;
        }

        private static void ConfigureSprite(string path, int ppu, FilterMode filter)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                Debug.LogWarning($"Sprite not found: {path}");
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = ppu;
            importer.filterMode = filter;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        private static (TileBase empty, TileBase floor, TileBase wall) CreateTileAssets()
        {
            var empty = CreateTile(
                $"{SpritesRoot}/Tiles/tile_empty.png",
                $"{SpritesRoot}/Tiles/EmptyTile.asset");
            var floor = CreateTile(
                $"{SpritesRoot}/Floors/tile_floor.png",
                $"{SpritesRoot}/Floors/FloorTile.asset");
            var wall = CreateTile(
                $"{SpritesRoot}/Walls/tile_wall.png",
                $"{SpritesRoot}/Walls/WallTile.asset");

            return (empty, floor, wall);
        }

        private static TileBase CreateTile(string spritePath, string tilePath)
        {
            var existing = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
            if (existing != null)
            {
                existing.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                EditorUtility.SetDirty(existing);
                AssetDatabase.SaveAssets();
                return existing;
            }

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite == null)
            {
                Debug.LogError($"Cannot load sprite at {spritePath}");
                return null;
            }

            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.color = Color.white;

            AssetDatabase.CreateAsset(tile, tilePath);
            AssetDatabase.SaveAssets();
            return tile;
        }

        private static void BuildSceneHierarchy(
            (TileBase empty, TileBase floor, TileBase wall) tiles,
            RuntimeAnimatorController catController)
        {
            // --- Camera ---
            var camObj = Camera.main != null ? Camera.main.gameObject : null;
            if (camObj == null)
            {
                Debug.LogError("No Main Camera in scene.");
                return;
            }

            var cam = camObj.GetComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 19f;
            cam.backgroundColor = new Color(0.12f, 0.12f, 0.15f);
            camObj.transform.position = new Vector3(24f, 16f, -10f);

            if (camObj.GetComponent<CameraController>() == null)
                camObj.AddComponent<CameraController>();

            // --- Grid parent ---
            var gridObj = FindOrCreate("Grid");
            var grid = gridObj.GetComponent<UnityEngine.Grid>();
            if (grid == null)
                grid = gridObj.AddComponent<UnityEngine.Grid>();
            grid.cellSize = Vector3.one;

            // --- Tilemaps ---
            var tmEmpty   = CreateTilemapChild(gridObj, "Tilemap_Empty",   0);
            var tmFloor   = CreateTilemapChild(gridObj, "Tilemap_Floor",   1);
            var tmWall    = CreateTilemapChild(gridObj, "Tilemap_Wall",    2);
            var tmPreview = CreateTilemapChild(gridObj, "Tilemap_Preview", 3);

            // --- GridManager ---
            var mgrObj = FindOrCreate("GridManager");

            var renderer = mgrObj.GetComponent<GridRenderer>();
            if (renderer == null)
                renderer = mgrObj.AddComponent<GridRenderer>();

            var so = new SerializedObject(renderer);
            so.FindProperty("_emptyTilemap").objectReferenceValue   = tmEmpty;
            so.FindProperty("_floorTilemap").objectReferenceValue   = tmFloor;
            so.FindProperty("_wallTilemap").objectReferenceValue    = tmWall;
            so.FindProperty("_previewTilemap").objectReferenceValue = tmPreview;
            so.FindProperty("_emptyTile").objectReferenceValue      = tiles.empty;
            so.FindProperty("_floorTile").objectReferenceValue      = tiles.floor;
            so.FindProperty("_wallTile").objectReferenceValue       = tiles.wall;
            so.ApplyModifiedProperties();

            var builder = mgrObj.GetComponent<RoomBuilderInput>();
            if (builder == null)
                builder = mgrObj.AddComponent<RoomBuilderInput>();

            var soBuilder = new SerializedObject(builder);
            soBuilder.FindProperty("_gridRenderer").objectReferenceValue = renderer;
            soBuilder.FindProperty("_camera").objectReferenceValue       = cam;
            soBuilder.ApplyModifiedProperties();

            // --- CatSpawner ---
            var spawner = mgrObj.GetComponent<CatSpawner>();
            if (spawner == null)
                spawner = mgrObj.AddComponent<CatSpawner>();

            var catFront = AssetDatabase.LoadAssetAtPath<Sprite>(
                $"{CatSpritesRoot}/CAT_EUR_FRONT.png");
            var catRight = AssetDatabase.LoadAssetAtPath<Sprite>(
                $"{CatSpritesRoot}/CAT_EUR_RIGHT.png");
            var catBack = AssetDatabase.LoadAssetAtPath<Sprite>(
                $"{CatSpritesRoot}/CAT_EUR_BACK.png");

            var soSpawner = new SerializedObject(spawner);
            soSpawner.FindProperty("_gridRenderer").objectReferenceValue    = renderer;
            soSpawner.FindProperty("_frontSprite").objectReferenceValue     = catFront;
            soSpawner.FindProperty("_rightSprite").objectReferenceValue     = catRight;
            soSpawner.FindProperty("_backSprite").objectReferenceValue      = catBack;
            soSpawner.FindProperty("_catAnimController").objectReferenceValue = catController;
            soSpawner.ApplyModifiedProperties();

            // --- ProtoUI ---
            var protoUI = mgrObj.GetComponent<ProtoUI>();
            if (protoUI == null)
                protoUI = mgrObj.AddComponent<ProtoUI>();

            var soUI = new SerializedObject(protoUI);
            soUI.FindProperty("_roomBuilder").objectReferenceValue      = builder;
            soUI.FindProperty("_catSpawner").objectReferenceValue       = spawner;
            soUI.FindProperty("_cameraController").objectReferenceValue =
                camObj.GetComponent<CameraController>();
            soUI.ApplyModifiedProperties();

            // --- Mark dirty ---
            EditorUtility.SetDirty(camObj);
            EditorUtility.SetDirty(gridObj);
            EditorUtility.SetDirty(mgrObj);

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        private static Tilemap CreateTilemapChild(GameObject parent, string name, int sortOrder)
        {
            var existing = parent.transform.Find(name);
            GameObject obj;
            if (existing != null)
            {
                obj = existing.gameObject;
            }
            else
            {
                obj = new GameObject(name);
                obj.transform.SetParent(parent.transform, false);
            }

            var tm = obj.GetComponent<Tilemap>();
            if (tm == null)
                tm = obj.AddComponent<Tilemap>();

            var tr = obj.GetComponent<TilemapRenderer>();
            if (tr == null)
                tr = obj.AddComponent<TilemapRenderer>();
            tr.sortingOrder = sortOrder;

            return tm;
        }

        private static GameObject FindOrCreate(string name)
        {
            var obj = GameObject.Find(name);
            if (obj == null)
                obj = new GameObject(name);
            return obj;
        }
    }
}
