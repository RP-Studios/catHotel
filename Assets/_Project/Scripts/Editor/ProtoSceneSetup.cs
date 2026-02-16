using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using CatHotel.Grid;
using CatHotel.Input;
using CatHotel.Cats;

namespace CatHotel.Editor
{
    public static class ProtoSceneSetup
    {
        private const string SpritesRoot = "Assets/_Project/Art/Environment";
        private const string CatSpritesRoot = "Assets/_Project/Art/Cats/Europeen";

        [MenuItem("Cat Hotel/Setup Proto Scene")]
        public static void SetupScene()
        {
            ConfigureSpriteImports();
            ConfigureCatSpriteImports();
            var tiles = CreateTileAssets();
            BuildSceneHierarchy(tiles);
            Debug.Log("Proto scene setup complete. " +
                "Controls: Left-click drag = build room, " +
                "Right-click drag = pan, Scroll = zoom, S = spawn cats");
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
            (TileBase empty, TileBase floor, TileBase wall) tiles)
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
            cam.orthographicSize = 8.5f;
            cam.backgroundColor = new Color(0.12f, 0.12f, 0.15f);
            camObj.transform.position = new Vector3(12f, 8f, -10f);

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
            soSpawner.FindProperty("_gridRenderer").objectReferenceValue = renderer;
            soSpawner.FindProperty("_frontSprite").objectReferenceValue  = catFront;
            soSpawner.FindProperty("_rightSprite").objectReferenceValue  = catRight;
            soSpawner.FindProperty("_backSprite").objectReferenceValue   = catBack;
            soSpawner.ApplyModifiedProperties();

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
