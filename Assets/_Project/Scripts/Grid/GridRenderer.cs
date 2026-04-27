using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CatHotel.Grid
{
    public class GridRenderer : MonoBehaviour
    {
        [Header("Tilemaps")]
        [SerializeField] private Tilemap _emptyTilemap;
        [SerializeField] private Tilemap _floorTilemap;
        [SerializeField] private Tilemap _wallTilemap;
        [SerializeField] private Tilemap _intWallCornerTilemap; // corners (anchor 0,0, order 5)
        [SerializeField] private Tilemap _intWallHSegTilemap;   // H segments (anchor 0,0, order 3)
        [SerializeField] private Tilemap _intWallVSegTilemap;   // V segments (anchor 0,0, order 4)
        [SerializeField] private Tilemap _previewTilemap;

        [Header("Tiles")]
        [SerializeField] private TileBase _emptyTile;
        [SerializeField] private TileBase[] _floorTiles;

        private TileBase _floorTile;

        [Header("Exterior Wall Tiles")]
        [SerializeField] private TileBase _wallTopTile;
        [SerializeField] private TileBase _wallBotTile;
        [SerializeField] private TileBase _wallLeftTopTile;
        [SerializeField] private TileBase _wallLeftMidTile;
        [SerializeField] private TileBase _wallRightTopTile;
        [SerializeField] private TileBase _wallRightMidTile;
        [SerializeField] private TileBase _wallTopLeftTile;
        [SerializeField] private TileBase _wallTopRightTile;

        [Header("Interior Wall Tiles")]
        [SerializeField] private TileBase _intWallH;
        [SerializeField] private TileBase _intWallV;
        [SerializeField] private TileBase _intCroix;
        [SerializeField] private TileBase _intTShape;
        [SerializeField] private TileBase _intCornerLB;
        [SerializeField] private TileBase _intCornerLT;
        [SerializeField] private TileBase _intCornerRB;
        [SerializeField] private TileBase _intCornerRT;

        [Header("Preview Colors")]
        [SerializeField] private Color _validColor   = new(0.2f, 0.8f, 0.2f, 0.5f);
        [SerializeField] private Color _invalidColor = new(0.8f, 0.2f, 0.2f, 0.5f);

        // Per-floor state
        public const int FloorCount = 6;

        private readonly GridData[] _floorGrids = new GridData[FloorCount];
        private readonly RoomRegistry[] _floorRegistries = new RoomRegistry[FloorCount];
        private readonly List<Vector2Int>[] _floorCentralCells = new List<Vector2Int>[FloorCount];
        private readonly int[] _floorTileIndexPerFloor = new int[FloorCount];

        // Currently active references — all internal methods read/write these.
        // BuildFloor temporarily points them at each floor during initial layout.
        private GridData _gridData;
        private RoomRegistry _roomRegistry;
        private int _floorTileIndex = -1;
        private int _currentFloor = 0;

        /// <summary>Index of the floor tile for the current floor. -1 if not yet chosen.</summary>
        public int FloorTileIndex => _floorTileIndex;

        /// <summary>Grid data for the currently active floor.</summary>
        public GridData Data      => _gridData;
        public RoomRegistry Rooms => _roomRegistry;

        /// <summary>Grid data for a specific floor (cats use this).</summary>
        public GridData GetFloorData(int floorIndex)
        {
            if (floorIndex < 0 || floorIndex >= FloorCount) return null;
            return _floorGrids[floorIndex];
        }

        public int CurrentFloor => _currentFloor;

        /// <summary>Set floor tile from save. RDC is currently locked to FLOOR_01; ignored.</summary>
        public void SetFloorTileIndex(int index)
        {
            // Ground floor tile is locked to FLOOR_01 — do not apply saved alternatives.
        }

        public List<Vector2Int> Entrances { get; private set; } = new();
        public List<Vector2Int> Exits { get; private set; } = new();

        /// <summary>Top-left entrance (pension cats arrive here). RDC only.</summary>
        public Vector2Int PensionEntrance { get; private set; }
        /// <summary>Bottom-left entrance (refuge cats arrive here). RDC only.</summary>
        public Vector2Int RefugeEntrance { get; private set; }
        /// <summary>Single right exit (unhappy cats leave here). RDC only.</summary>
        public Vector2Int UnhappyExit { get; private set; }

        /// <summary>Animated doors at each entrance/exit. Key = grid position of the door wall cell.</summary>
        public CatHotel.Hotel.AnimatedDoor PensionDoor { get; private set; }
        public CatHotel.Hotel.AnimatedDoor RefugeDoor { get; private set; }
        public CatHotel.Hotel.AnimatedDoor ExitDoor { get; private set; }

        [Header("Doors")]
        [SerializeField] private Sprite[] _doorFrames;

        [Header("Entrance Logos")]
        [SerializeField] private Sprite _pensionLogoSprite;
        [SerializeField] private Sprite _refugeLogoSprite;

        [Header("Stairs")]
        [SerializeField] private CatHotel.Core.HotelObjectData _stairsData;
        [Tooltip("Bottom-left grid position of the 2x2 stairs footprint on RDC")]
        [SerializeField] private Vector2Int _stairsBottomLeft = new(12, 8);
        /// <summary>Central room interior floor cells for the currently active floor.</summary>
        public List<Vector2Int> CentralRoomFloorCells => _floorCentralCells[_currentFloor];

        public List<Vector2Int> GetCentralRoomFloorCells(int floorIndex)
        {
            if (floorIndex < 0 || floorIndex >= FloorCount) return null;
            return _floorCentralCells[floorIndex];
        }

        // Rotation matrices for T-shape corners
        static readonly Matrix4x4 Rot0   = Matrix4x4.identity;
        static readonly Matrix4x4 Rot90  = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90),  Vector3.one);
        static readonly Matrix4x4 Rot180 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 180), Vector3.one);
        static readonly Matrix4x4 Rot270 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 270), Vector3.one);

        private void Awake()
        {
            for (int i = 0; i < FloorCount; i++)
            {
                _floorGrids[i] = new GridData();
                _floorRegistries[i] = new RoomRegistry(_floorGrids[i]);
                _floorCentralCells[i] = new List<Vector2Int>();
            }

            _currentFloor = 0;
            _gridData = _floorGrids[0];
            _roomRegistry = _floorRegistries[0];
        }

        private void Start()
        {
            if (_emptyTilemap == null || _emptyTile == null)
            {
                Debug.LogError("[GridRenderer] Missing references! " +
                    "Run 'Cat Hotel > Setup Proto Scene' from the menu.");
                return;
            }

            if (_floorTiles == null || _floorTiles.Length == 0)
                return;

            // FloorSpriteNames layout: pairs of (base, var) per visual style.
            // Indices: FLOOR_Basic(0/1), FLOOR_01(2/3), FLOOR_02(4/5), FLOOR_03(6/7), FLOOR_04(8/9), FLOOR_05(10/11)
            // Map floors 0..5 to distinct visuals.
            int[] perFloor = { 2, 8, 4, 6, 0, 10 }; // RDC=01, F1=04, F2=02, F3=03, F4=Basic, F5=05
            for (int i = 0; i < FloorCount; i++)
                _floorTileIndexPerFloor[i] = Mathf.Min(perFloor[i % perFloor.Length], _floorTiles.Length - 1);

            // Build each floor by temporarily pointing _gridData/_roomRegistry at it
            for (int i = 0; i < FloorCount; i++)
            {
                SetActiveFloorRefs(i);
                DrawBorderWalls();
                BuildFloorLayout(i);
            }

            // Start on ground floor
            SetActiveFloorRefs(0);
            _floorTile = _floorTiles[_floorTileIndex];
            RefreshAll();
        }

        private void SetActiveFloorRefs(int floorIndex)
        {
            _currentFloor = floorIndex;
            _gridData = _floorGrids[floorIndex];
            _roomRegistry = _floorRegistries[floorIndex];
            _floorTileIndex = _floorTileIndexPerFloor[floorIndex];
        }

        /// <summary>Switch active floor: swap data refs, update floor tile, redraw everything.
        /// Visibility of objects/cats is handled by FloorManager.</summary>
        public void SetCurrentFloor(int floorIndex)
        {
            if (floorIndex < 0 || floorIndex >= FloorCount) return;
            if (floorIndex == _currentFloor) return;

            SetActiveFloorRefs(floorIndex);
            if (_floorTiles != null && _floorTileIndex >= 0 && _floorTileIndex < _floorTiles.Length)
                _floorTile = _floorTiles[_floorTileIndex];
            RefreshAll();
        }

        private void BuildFloorLayout(int floorIndex)
        {
            var centralRect = new RectInt(2, 2, 22, 14);
            FillRoom(centralRect);
            _roomRegistry.RegisterRoom(centralRect);

            var cells = _floorCentralCells[floorIndex];
            for (int y = centralRect.yMin + 1; y < centralRect.yMax - 1; y++)
                for (int x = centralRect.xMin + 1; x < centralRect.xMax - 1; x++)
                    cells.Add(new Vector2Int(x, y));

            // Entrances/exit are RDC only
            if (floorIndex == 0)
            {
                int wallXL = centralRect.xMin;
                int entranceBottomY = centralRect.yMin + 4; // refuge
                int entranceTopY = centralRect.yMax - 5;    // pension

                PunchCorridor(wallXL, entranceBottomY, -1);
                PunchCorridor(wallXL, entranceTopY, -1);

                var refuseEntr = new Vector2Int(wallXL - 2, entranceBottomY);
                var pensionEntr = new Vector2Int(wallXL - 2, entranceTopY);
                Entrances.Add(refuseEntr);
                Entrances.Add(pensionEntr);
                RefugeEntrance = refuseEntr;
                PensionEntrance = pensionEntr;

                int wallXR = centralRect.xMax - 1;
                int exitY = entranceBottomY;
                PunchCorridor(wallXR, exitY, +1);

                var unhappyExit = new Vector2Int(wallXR + 2, exitY);
                Exits.Add(unhappyExit);
                UnhappyExit = unhappyExit;

                CreateEntranceLogo("LogoPension", wallXL, entranceTopY + 1, _pensionLogoSprite);
                CreateEntranceLogo("LogoRefuge", wallXL, entranceBottomY + 1, _refugeLogoSprite);
            }

            // Stairs at the same position on every floor
            SpawnStairs(floorIndex);
        }

        /// <summary>Stairs bottom-left, public read-only for FloorManager / CatEntity.</summary>
        public Vector2Int StairsBottomLeft => _stairsBottomLeft;
        public Vector2Int StairsSize => _stairsData != null ? _stairsData.size : new Vector2Int(2, 2);

        /// <summary>Props visible only on the ground floor (entrance logos, doors, ...).</summary>
        private readonly List<SpriteRenderer> _groundFloorOnlyProps = new();

        /// <summary>Show/hide ground-floor-only props (entrance logos, doors).
        /// Called by FloorManager when switching floors.</summary>
        public void SetGroundFloorPropsVisible(bool visible)
        {
            foreach (var sr in _groundFloorOnlyProps)
                if (sr != null) sr.enabled = visible;
        }

        private void SpawnStairs(int floorIndex)
        {
            if (_stairsData == null) return;

            var size = _stairsData.size;
            if (size.x <= 0 || size.y <= 0) size = new Vector2Int(2, 2);

            // Mark cells as Stairs (blocks pathfinding; still drawn as floor)
            for (int dy = 0; dy < size.y; dy++)
            for (int dx = 0; dx < size.x; dx++)
            {
                int x = _stairsBottomLeft.x + dx;
                int y = _stairsBottomLeft.y + dy;
                if (!_gridData.InBounds(x, y)) return;
                _gridData.SetCell(x, y, CellType.Stairs);
            }

            var go = new GameObject($"Obj_{_stairsData.displayName}_F{floorIndex}");
            go.transform.SetParent(transform);
            float cx = _stairsBottomLeft.x + size.x * 0.5f;
            float cy = _stairsBottomLeft.y + 0.25f;
            go.transform.position = new Vector3(cx, cy, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _stairsData.worldSprite != null ? _stairsData.worldSprite : _stairsData.icon;
            sr.sortingLayerName = "Objects";

            if (sr.sprite != null)
            {
                float spriteW = sr.sprite.bounds.size.x;
                float spriteH = sr.sprite.bounds.size.y;
                if (spriteW > 0f && spriteH > 0f)
                {
                    float scale = Mathf.Min(size.x / spriteW, size.y / spriteH) * _stairsData.visualScale;
                    go.transform.localScale = new Vector3(scale, scale, 1f);
                }
            }

            go.AddComponent<Core.SortByY>();

            var hotelObj = go.AddComponent<CatHotel.Hotel.HotelObject>();
            hotelObj.Init(_stairsData, _stairsBottomLeft, floorIndex);
            // Hide stairs on non-ground floors at startup (FloorManager will re-sync)
            if (floorIndex != 0)
                sr.enabled = false;
        }

        /// <summary>
        /// Punch a 2-high corridor through a wall.
        /// wallX = the wall cell X, baseY = bottom Y of the 2-high opening.
        /// dir = -1 for left, +1 for right.
        /// Creates: 2 Door cells through the wall, 2 Door cells in the gap, 2 Door cells outside.
        /// Also sets Wall cells above and below the corridor for proper framing.
        /// </summary>
        private CatHotel.Hotel.AnimatedDoor CreateDoor(string name, int wallX, int baseY)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            // Position at bottom-left of the door cell
            go.transform.position = new Vector3(wallX, baseY, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Objects";
            sr.sortingOrder = 500;

            var door = go.AddComponent<CatHotel.Hotel.AnimatedDoor>();
            door.Init(_doorFrames);
            return door;
        }

        private void CreateEntranceLogo(string name, int wallX, int wallY, Sprite sprite)
        {
            if (sprite == null) return;

            var go = new GameObject(name);
            go.transform.SetParent(transform);
            // Center on the wall cell above the corridor entrance
            go.transform.position = new Vector3(wallX + 0.5f, wallY + 0.5f, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingLayerName = "Objects";
            sr.sortingOrder = 100; // above wall tiles
            _groundFloorOnlyProps.Add(sr);

            // Scale to fit within 1 tile width
            float spriteW = sprite.bounds.size.x;
            float spriteH = sprite.bounds.size.y;
            if (spriteW > 0f && spriteH > 0f)
            {
                float scale = Mathf.Min(1f / spriteW, 1f / spriteH);
                go.transform.localScale = new Vector3(scale, scale, 1f);
            }
        }

        /// <summary>
        /// Punch a 1-high corridor through a wall.
        /// wallX = the wall cell X, baseY = Y of the opening.
        /// dir = -1 for left, +1 for right.
        /// Creates 3 Door cells (wall, gap, outside) and frames with walls above and below.
        /// </summary>
        private void PunchCorridor(int wallX, int baseY, int dir)
        {
            int wallAbove = baseY + 1;
            int wallBelow = baseY - 1;

            // 3 columns: wall, gap, outside
            for (int step = 0; step <= 2; step++)
            {
                int x = wallX + dir * step;
                if (!_gridData.InBounds(x, baseY)) continue;
                _gridData.SetCell(x, baseY, CellType.Door);

                // Frame the corridor with walls above and below
                if (_gridData.InBounds(x, wallAbove) && !_gridData.IsWalkable(x, wallAbove))
                    _gridData.SetCell(x, wallAbove, CellType.Wall);
                if (_gridData.InBounds(x, wallBelow) && !_gridData.IsWalkable(x, wallBelow))
                    _gridData.SetCell(x, wallBelow, CellType.Wall);
            }
        }

        private void FillRoom(RectInt rect)
        {
            for (int y = rect.yMin; y < rect.yMax; y++)
            {
                for (int x = rect.xMin; x < rect.xMax; x++)
                {
                    bool isBorder = y == rect.yMin || y == rect.yMax - 1
                                 || x == rect.xMin || x == rect.xMax - 1;
                    if (isBorder)
                    {
                        if (!_gridData.IsWalkable(x, y))
                            _gridData.SetCell(x, y, CellType.Wall);
                    }
                    else
                    {
                        _gridData.SetCell(x, y, CellType.Floor);
                    }
                }
            }
        }

        public void ShowGrid()
        {
            int minX = GridData.Width, maxX = 0;
            int minY = GridData.Height, maxY = 0;
            for (int y = 0; y < GridData.Height; y++)
                for (int x = 0; x < GridData.Width; x++)
                    if (_gridData.IsWalkable(x, y) || _gridData.GetCell(x, y) == CellType.InternalWall)
                    {
                        if (x < minX) minX = x;
                        if (x > maxX) maxX = x;
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                    }

            int yStart = Mathf.Max(0, minY - 1);
            int yEnd   = Mathf.Min(GridData.Height - 1, maxY + 1);
            int xStart = Mathf.Max(0, minX - 1);
            int xEnd   = Mathf.Min(GridData.Width - 1, maxX + 1);

            for (int y = yStart; y <= yEnd; y++)
                for (int x = xStart; x <= xEnd; x++)
                    _emptyTilemap.SetTile(new Vector3Int(x, y, 0), _emptyTile);

            _emptyTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
        }

        public void HideGrid()
        {
            _emptyTilemap.ClearAllTiles();
            _emptyTilemap.GetComponent<TilemapRenderer>().sortingOrder = 0;
        }

        private void DrawBorderWalls()
        {
            for (int x = 0; x < GridData.Width; x++)
            {
                _gridData.SetCell(x, 0, CellType.Wall);
                _gridData.SetCell(x, GridData.Height - 1, CellType.Wall);
            }
            for (int y = 1; y < GridData.Height - 1; y++)
            {
                _gridData.SetCell(0, y, CellType.Wall);
                _gridData.SetCell(GridData.Width - 1, y, CellType.Wall);
            }
        }

        public void RefreshAll()
        {
            _floorTilemap.ClearAllTiles();
            _wallTilemap.ClearAllTiles();

            for (int y = 0; y < GridData.Height; y++)
            {
                for (int x = 0; x < GridData.Width; x++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    var cell = _gridData.GetCell(x, y);

                    if (cell == CellType.Floor || cell == CellType.Door)
                    {
                        _floorTilemap.SetTile(pos, _floorTile);

                        bool edgeLeft  = !_gridData.InBounds(x - 1, y) || !IsFloorLike(x - 1, y);
                        bool edgeRight = !_gridData.InBounds(x + 1, y) || !IsFloorLike(x + 1, y);

                        if (edgeLeft && !edgeRight)
                            _wallTilemap.SetTile(pos, _wallRightMidTile);
                        else if (edgeRight && !edgeLeft)
                            _wallTilemap.SetTile(pos, _wallLeftMidTile);
                    }
                    else if (cell == CellType.Stairs)
                    {
                        _floorTilemap.SetTile(pos, _floorTile);
                    }
                    else if (cell == CellType.Wall)
                    {
                        var tile = GetExteriorWallTile(x, y);
                        if (tile != null)
                            _wallTilemap.SetTile(pos, tile);
                    }
                    else if (cell == CellType.InternalWall)
                    {
                        _floorTilemap.SetTile(pos, _floorTile);
                    }
                }
            }

            RebuildInteriorWalls();
        }

        private void RebuildInteriorWalls()
        {
            if (_intWallCornerTilemap != null) _intWallCornerTilemap.ClearAllTiles();
            if (_intWallHSegTilemap != null)   _intWallHSegTilemap.ClearAllTiles();
            if (_intWallVSegTilemap != null)   _intWallVSegTilemap.ClearAllTiles();

            for (int y = 0; y < GridData.Height; y++)
                for (int x = 0; x < GridData.Width; x++)
                    if (_gridData.GetCell(x, y) == CellType.InternalWall)
                        PlaceInteriorWallVisuals(x, y);
        }

        /// <summary>
        /// Place a single InternalWall cell incrementally. Never removes existing tiles.
        /// Floor → becomes InternalWall. Wall → keeps type but acts as anchor (corner + segments).
        /// </summary>
        public bool PlaceInternalWall(Vector2Int pos)
        {
            if (!_gridData.InBounds(pos.x, pos.y)) return false;

            var cell = _gridData.GetCell(pos.x, pos.y);

            if (cell == CellType.Floor)
            {
                _gridData.SetCell(pos.x, pos.y, CellType.InternalWall);
                _floorTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), _floorTile);
                PlaceInteriorWallVisuals(pos.x, pos.y);
                return true;
            }

            if (cell == CellType.Wall)
            {
                // Wall keeps its type. No corner on it (it has its own visual).
                // Just place segments to adjacent InternalWall cells and update their corners.
                PlaceSegmentsFrom(pos.x, pos.y);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update corner + segments for one InternalWall cell, and refresh neighbor corners.
        /// </summary>
        private void PlaceInteriorWallVisuals(int x, int y)
        {
            // Update this corner
            UpdateCornerAt(x, y);

            // Update neighbor corners only if they are InternalWall (not exterior Wall)
            if (IsIntWallOnly(x + 1, y)) UpdateCornerAt(x + 1, y);
            if (IsIntWallOnly(x - 1, y)) UpdateCornerAt(x - 1, y);
            if (IsIntWallOnly(x, y + 1)) UpdateCornerAt(x, y + 1);
            if (IsIntWallOnly(x, y - 1)) UpdateCornerAt(x, y - 1);

            PlaceSegmentsFrom(x, y);
        }

        /// <summary>
        /// Place segments from cell (x,y) to adjacent wall anchors.
        /// Also updates neighbor InternalWall corners.
        /// Used for both InternalWall and Wall cells.
        /// </summary>
        private void PlaceSegmentsFrom(int x, int y)
        {
            // H segment: placed at the LEFT cell of the pair
            if (IsWallAnchor(x + 1, y) && _intWallHSegTilemap != null && _intWallH != null)
                _intWallHSegTilemap.SetTile(new Vector3Int(x, y, 0), _intWallH);
            if (IsWallAnchor(x - 1, y) && _intWallHSegTilemap != null && _intWallH != null)
                _intWallHSegTilemap.SetTile(new Vector3Int(x - 1, y, 0), _intWallH);

            // V segment: placed at the BOTTOM cell of the pair
            if (IsWallAnchor(x, y + 1) && _intWallVSegTilemap != null && _intWallV != null)
                _intWallVSegTilemap.SetTile(new Vector3Int(x, y, 0), _intWallV);
            if (IsWallAnchor(x, y - 1) && _intWallVSegTilemap != null && _intWallV != null)
                _intWallVSegTilemap.SetTile(new Vector3Int(x, y - 1, 0), _intWallV);

            // Update neighbor InternalWall corners (they may now connect to this wall)
            if (IsIntWallOnly(x + 1, y)) UpdateCornerAt(x + 1, y);
            if (IsIntWallOnly(x - 1, y)) UpdateCornerAt(x - 1, y);
            if (IsIntWallOnly(x, y + 1)) UpdateCornerAt(x, y + 1);
            if (IsIntWallOnly(x, y - 1)) UpdateCornerAt(x, y - 1);
        }

        private void UpdateCornerAt(int x, int y)
        {
            if (_intWallCornerTilemap == null) return;

            var pos = new Vector3Int(x, y, 0);
            var (tile, matrix) = GetCornerTileAndMatrix(x, y);
            _intWallCornerTilemap.SetTile(pos, tile);
            _intWallCornerTilemap.SetTransformMatrix(pos, matrix);
        }

        private (TileBase tile, Matrix4x4 matrix) GetCornerTileAndMatrix(int x, int y)
        {
            // Use WallAnchor so interior walls can connect to exterior walls
            bool r = IsWallAnchor(x + 1, y);
            bool l = IsWallAnchor(x - 1, y);
            bool u = IsWallAnchor(x, y + 1);
            bool d = IsWallAnchor(x, y - 1);
            int count = (r ? 1 : 0) + (l ? 1 : 0) + (u ? 1 : 0) + (d ? 1 : 0);

            if (count == 4)
                return (_intCroix, Rot0);

            if (count == 3 && _intTShape != null)
            {
                // T-shape: base sprite has connections L,R,D (missing U)
                if (!u) return (_intTShape, Rot0);
                if (!r) return (_intTShape, Rot90);
                if (!d) return (_intTShape, Rot180);
                if (!l) return (_intTShape, Rot270);
            }

            if (count == 2)
            {
                // L-corners (adjacent walls forming a 90° angle)
                if (r && u && _intCornerLB != null) return (_intCornerLB, Rot0);
                if (l && u && _intCornerRB != null) return (_intCornerRB, Rot0);
                if (r && d && _intCornerLT != null) return (_intCornerLT, Rot0);
                if (l && d && _intCornerRT != null) return (_intCornerRT, Rot0);
                // Straight line (opposite walls): no corner needed
                return (null, Rot0);
            }

            // 0 or 1 neighbor: cross
            return (_intCroix, Rot0);
        }

        /// <summary>Walkable floor or stairs — used for inner edge wall detection to avoid
        /// spawning wall-column sprites around stairs cells.</summary>
        private bool IsFloorLike(int x, int y)
        {
            if (!_gridData.InBounds(x, y)) return false;
            var c = _gridData.GetCell(x, y);
            return c == CellType.Floor || c == CellType.Door || c == CellType.Stairs;
        }

        /// <summary>Returns true for InternalWall only (for corner updates).</summary>
        private bool IsIntWallOnly(int x, int y)
        {
            return _gridData.InBounds(x, y) && _gridData.GetCell(x, y) == CellType.InternalWall;
        }

        /// <summary>Returns true for InternalWall or exterior Wall (valid anchor for segments).</summary>
        private bool IsWallAnchor(int x, int y)
        {
            if (!_gridData.InBounds(x, y)) return false;
            var c = _gridData.GetCell(x, y);
            return c == CellType.InternalWall || c == CellType.Wall;
        }

        private TileBase GetExteriorWallTile(int x, int y)
        {
            bool floorBelow = _gridData.InBounds(x, y - 1) && _gridData.IsWalkable(x, y - 1);
            bool floorAbove = _gridData.InBounds(x, y + 1) && _gridData.IsWalkable(x, y + 1);

            if (floorBelow)
            {
                bool topWallLeft  = _gridData.InBounds(x - 1, y - 1) && _gridData.IsWalkable(x - 1, y - 1);
                bool topWallRight = _gridData.InBounds(x + 1, y - 1) && _gridData.IsWalkable(x + 1, y - 1);

                if (!topWallLeft && topWallRight)  return _wallTopLeftTile;
                if (topWallLeft  && !topWallRight) return _wallTopRightTile;
                return _wallTopTile;
            }

            if (floorAbove) return _wallBotTile;
            return null;
        }

        public void ShowPreview(RectInt rect, bool valid)
        {
            _previewTilemap.ClearAllTiles();
            _previewTilemap.color = valid ? _validColor : _invalidColor;

            for (int y = rect.yMin; y < rect.yMax; y++)
                for (int x = rect.xMin; x < rect.xMax; x++)
                {
                    if (!_gridData.InBounds(x, y)) continue;
                    _previewTilemap.SetTile(new Vector3Int(x, y, 0), _floorTile);
                }
        }

        public void ClearPreview()
        {
            _previewTilemap.ClearAllTiles();
        }
    }
}
