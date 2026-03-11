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

        private GridData _gridData;
        private RoomRegistry _roomRegistry;

        public GridData Data      => _gridData;
        public RoomRegistry Rooms => _roomRegistry;

        public List<Vector2Int> Entrances { get; private set; } = new();
        public List<Vector2Int> CentralRoomFloorCells { get; private set; } = new();

        // Rotation matrices for T-shape corners
        static readonly Matrix4x4 Rot0   = Matrix4x4.identity;
        static readonly Matrix4x4 Rot90  = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90),  Vector3.one);
        static readonly Matrix4x4 Rot180 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 180), Vector3.one);
        static readonly Matrix4x4 Rot270 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 270), Vector3.one);

        private void Awake()
        {
            _gridData = new GridData();
            _roomRegistry = new RoomRegistry(_gridData);
        }

        private void Start()
        {
            if (_emptyTilemap == null || _emptyTile == null)
            {
                Debug.LogError("[GridRenderer] Missing references! " +
                    "Run 'Cat Hotel > Setup Proto Scene' from the menu.");
                return;
            }

            if (_floorTiles != null && _floorTiles.Length > 0)
                _floorTile = _floorTiles[Random.Range(0, _floorTiles.Length)];

            DrawBorderWalls();
            BuildInitialLayout();
            RefreshAll();

            Debug.Log($"[GridRenderer] Grid initialized: {GridData.Width}x{GridData.Height}, " +
                $"CentralFloor: {CentralRoomFloorCells.Count}");
        }

        private void BuildInitialLayout()
        {
            var centralRect = new RectInt(5, 9, 17, 16);
            FillRoom(centralRect);
            _roomRegistry.RegisterRoom(centralRect);

            for (int y = centralRect.yMin + 1; y < centralRect.yMax - 1; y++)
                for (int x = centralRect.xMin; x < centralRect.xMax; x++)
                    CentralRoomFloorCells.Add(new Vector2Int(x, y));
        }

        private void FillRoom(RectInt rect)
        {
            for (int y = rect.yMin; y < rect.yMax; y++)
            {
                for (int x = rect.xMin; x < rect.xMax; x++)
                {
                    bool isTopOrBottom = y == rect.yMin || y == rect.yMax - 1;
                    if (isTopOrBottom)
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

                        bool edgeLeft  = !_gridData.InBounds(x - 1, y) || _gridData.GetCell(x - 1, y) == CellType.Empty;
                        bool edgeRight = !_gridData.InBounds(x + 1, y) || _gridData.GetCell(x + 1, y) == CellType.Empty;

                        if (edgeLeft && !edgeRight)
                            _wallTilemap.SetTile(pos, _wallRightMidTile);
                        else if (edgeRight && !edgeLeft)
                            _wallTilemap.SetTile(pos, _wallLeftMidTile);
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
