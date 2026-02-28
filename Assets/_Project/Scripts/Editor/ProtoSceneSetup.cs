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

        private const string CatControllerPath = AnimRoot + "/CatEuropeen.controller";

        // (sheetFile, slicePrefix, stateName, frames, fps)
        // Walk: 8 frames, 0.67s loop → 12 FPS
        // Idle (6f): 1s → 6 FPS | Idle (8f): 1s → 8 FPS
        // Rest (sleep 9f): 2s → 4.5 FPS | (eat 10f): 2s → 5 FPS | (drink 8f): 2s → 4 FPS
        private static readonly (string file, string prefix, string state, int frames, float fps)[] AnimConfigs =
        {
            // Walk (8f, 12 FPS)
            ("base_walk_face.png",  "walk_face",  "Walk_Front", 8, 12f),
            ("base_walk_back.png",  "walk_back",  "Walk_Back",  8, 12f),
            ("base_walk_left.png",  "walk_left",  "Walk_Left",  8, 12f),
            ("base_walk_right.png", "walk_right", "Walk_Right", 8, 12f),

            // Idle1 (6f, 6 FPS)
            ("base_idle1_face.png",  "idle1_face",  "Idle1_Front", 6, 6f),
            ("base_idle1_back.png",  "idle1_back",  "Idle1_Back",  6, 6f),
            ("base_idle1_left.png",  "idle1_left",  "Idle1_Left",  6, 6f),
            ("base_idle1_right.png", "idle1_right", "Idle1_Right", 6, 6f),

            // Idle2 (6f, 6 FPS)
            ("base_idle2_face.png",  "idle2_face",  "Idle2_Front", 6, 6f),
            ("base_idle2_left.png",  "idle2_left",  "Idle2_Left",  6, 6f),
            ("base_idle2_right.png", "idle2_right", "Idle2_Right", 6, 6f),

            // Idle3 (8f, 8 FPS)
            ("base_idle3_face.png",  "idle3_face",  "Idle3_Front", 8, 8f),
            ("base_idle3_back.png",  "idle3_back",  "Idle3_Back",  8, 8f),
            ("base_idle3_left.png",  "idle3_left",  "Idle3_Left",  8, 8f),
            ("base_idle3_right.png", "idle3_right", "Idle3_Right", 8, 8f),

            // Sleep (9f, 4.5 FPS)
            ("base_sleeping_face.png",  "sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("base_sleeping_left.png",  "sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("base_sleeping_right.png", "sleeping_right", "Sleep_Right", 9, 4.5f),

            // Eat (face 12f → 6 FPS, left/right 10f → 5 FPS, all 2s loop)
            ("base_eating_face.png",  "eating_face",  "Eat_Front", 12, 6f),
            ("base_eating_left.png",  "eating_left",  "Eat_Left",  10, 5f),
            ("base_eating_right.png", "eating_right", "Eat_Right", 10, 5f),

            // Drink (8f, 4 FPS)
            ("base_drunking_face.png",  "drunking_face",  "Drink_Front", 8, 4f),
            ("base_drunking_left.png",  "drunking_left",  "Drink_Left",  8, 4f),
            ("base_drunking_right.png", "drunking_right", "Drink_Right", 8, 4f),

            // Cleaning (11f, 5.5 FPS → 2s loop)
            ("base_cleaning_face.png",  "cleaning_face",  "Clean_Front", 11, 5.5f),
            ("base_cleaning_left.png",  "cleaning_left",  "Clean_Left",  11, 5.5f),
            ("base_cleaning_right.png", "cleaning_right", "Clean_Right", 11, 5.5f),

            // Happy (20f, 10 FPS → 2s one-shot)
            ("base_happy_face.png",  "happy_face",  "Happy_Front", 20, 10f),
            ("base_happy_left.png",  "happy_left",  "Happy_Left",  20, 10f),
            ("base_happy_right.png", "happy_right", "Happy_Right", 20, 10f),

            // Unhappy (6f, 6 FPS → 1s one-shot)
            ("base_unhappy_face.png",  "unhappy_face",  "Unhappy_Front", 6, 6f),
            ("base_unhappy_left.png",  "unhappy_left",  "Unhappy_Left",  6, 6f),
            ("base_unhappy_right.png", "unhappy_right", "Unhappy_Right", 6, 6f),
        };

        [MenuItem("Cat Hotel/Setup Proto Scene")]
        public static void SetupScene()
        {
            ConfigureSpriteImports();
            ConfigureCatSpriteImports();

            foreach (var cfg in AnimConfigs)
                ConfigureSpritesheet($"{AnimRoot}/{cfg.file}", cfg.prefix, cfg.frames);
            AssetDatabase.Refresh();

            var tiles = CreateTileAssets();
            var catController = CreateCatAnimationAssets();
            BuildSceneHierarchy(tiles, catController);
            Debug.Log($"Proto scene setup complete. {AnimConfigs.Length} animation clips configured.");
        }

        private static void ConfigureSpriteImports()
        {
            // tile_empty is 32x32 → PPU 32
            ConfigureSprite($"{SpritesRoot}/Tiles/tile_empty.png", 32, FilterMode.Point);

            // parquet + walls are 256x256 → PPU 256 so 1 sprite = 1 tile
            ConfigureSprite($"{SpritesRoot}/Floors/parquet01.png", 256, FilterMode.Point);
            ConfigureSprite($"{SpritesRoot}/Walls/murHorizontal.png", 256, FilterMode.Point);
            ConfigureSprite($"{SpritesRoot}/Walls/murVert.png", 256, FilterMode.Point);
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
            // Clean old clips
            AssetDatabase.DeleteAsset(AnimRoot + "/Idle_Front.anim");
            AssetDatabase.DeleteAsset(AnimRoot + "/Idle_Left.anim");
            AssetDatabase.DeleteAsset(AnimRoot + "/Idle_Right.anim");

            // Create all clips and states
            AssetDatabase.DeleteAsset(CatControllerPath);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(CatControllerPath);
            var rootSM = controller.layers[0].stateMachine;
            bool defaultSet = false;

            foreach (var cfg in AnimConfigs)
            {
                string sheetPath = $"{AnimRoot}/{cfg.file}";
                string clipPath = $"{AnimRoot}/{cfg.state}.anim";

                var clip = CreateAnimClip(sheetPath, clipPath, cfg.state, cfg.frames, cfg.fps);
                if (clip == null) continue;

                var state = rootSM.AddState(cfg.state);
                state.motion = clip;

                if (!defaultSet)
                {
                    rootSM.defaultState = state;
                    defaultSet = true;
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[ProtoSceneSetup] Created AnimatorController with {rootSM.states.Length} states");
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

        private static (TileBase empty, TileBase floor, TileBase wallH, TileBase wallV) CreateTileAssets()
        {
            var empty = CreateTile(
                $"{SpritesRoot}/Tiles/tile_empty.png",
                $"{SpritesRoot}/Tiles/EmptyTile.asset");
            var floor = CreateTile(
                $"{SpritesRoot}/Floors/parquet01.png",
                $"{SpritesRoot}/Floors/FloorTile.asset");
            var wallH = CreateTile(
                $"{SpritesRoot}/Walls/murHorizontal.png",
                $"{SpritesRoot}/Walls/WallHTile.asset");
            var wallV = CreateTile(
                $"{SpritesRoot}/Walls/murVert.png",
                $"{SpritesRoot}/Walls/WallVTile.asset");

            return (empty, floor, wallH, wallV);
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
            (TileBase empty, TileBase floor, TileBase wallH, TileBase wallV) tiles,
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
            so.FindProperty("_wallHTile").objectReferenceValue      = tiles.wallH;
            so.FindProperty("_wallVTile").objectReferenceValue      = tiles.wallV;
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
