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
        [SerializeField] private Tilemap _intWallCrossTilemap;  // crosses (anchor 0,0, order 4)
        [SerializeField] private Tilemap _intWallSegTilemap;    // wall segments (anchor 0,0, order 3)
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

        [Header("Preview Colors")]
        [SerializeField] private Color _validColor   = new(0.2f, 0.8f, 0.2f, 0.5f);
        [SerializeField] private Color _invalidColor = new(0.8f, 0.2f, 0.2f, 0.5f);

        private GridData _gridData;
        private RoomRegistry _roomRegistry;

        public GridData Data      => _gridData;
        public RoomRegistry Rooms => _roomRegistry;

        public List<Vector2Int> Entrances { get; private set; } = new();
        public List<Vector2Int> CentralRoomFloorCells { get; private set; } = new();

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

            // Full rebuild of interior walls
            RebuildInteriorWalls();
        }

        /// <summary>
        /// Full rebuild of interior wall visuals (called from Start/RefreshAll).
        /// </summary>
        private void RebuildInteriorWalls()
        {
            if (_intWallCrossTilemap != null) _intWallCrossTilemap.ClearAllTiles();
            if (_intWallSegTilemap != null)   _intWallSegTilemap.ClearAllTiles();

            for (int y = 0; y < GridData.Height; y++)
                for (int x = 0; x < GridData.Width; x++)
                    if (_gridData.GetCell(x, y) == CellType.InternalWall)
                        PlaceInteriorWallVisuals(x, y);
        }

        /// <summary>
        /// Place a single InternalWall cell incrementally.
        /// Cross always stays. Wall segments added between adjacent crosses.
        /// </summary>
        public bool PlaceInternalWall(Vector2Int pos)
        {
            if (!_gridData.InBounds(pos.x, pos.y)) return false;
            if (_gridData.GetCell(pos.x, pos.y) != CellType.Floor) return false;

            _gridData.SetCell(pos.x, pos.y, CellType.InternalWall);
            _floorTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), _floorTile);

            // Place cross at this position
            PlaceInteriorWallVisuals(pos.x, pos.y);

            return true;
        }

        /// <summary>
        /// Add cross + wall segments for one InternalWall cell. Never clears existing tiles.
        /// </summary>
        private void PlaceInteriorWallVisuals(int x, int y)
        {
            var pos = new Vector3Int(x, y, 0);

            // Always place a cross at this corner
            if (_intWallCrossTilemap != null)
                _intWallCrossTilemap.SetTile(pos, _intCroix);

            // Check each neighbor: if it's also InternalWall, place a wall segment between us
            // H segment goes RIGHT from the leftmost of the two cells
            if (_gridData.InBounds(x + 1, y) && _gridData.GetCell(x + 1, y) == CellType.InternalWall)
            {
                if (_intWallSegTilemap != null && _intWallH != null)
                    _intWallSegTilemap.SetTile(pos, _intWallH);
            }
            if (_gridData.InBounds(x - 1, y) && _gridData.GetCell(x - 1, y) == CellType.InternalWall)
            {
                if (_intWallSegTilemap != null && _intWallH != null)
                    _intWallSegTilemap.SetTile(new Vector3Int(x - 1, y, 0), _intWallH);
            }

            // V segment goes UP from the lower of the two cells
            if (_gridData.InBounds(x, y + 1) && _gridData.GetCell(x, y + 1) == CellType.InternalWall)
            {
                if (_intWallSegTilemap != null && _intWallV != null)
                    _intWallSegTilemap.SetTile(pos, _intWallV);
            }
            if (_gridData.InBounds(x, y - 1) && _gridData.GetCell(x, y - 1) == CellType.InternalWall)
            {
                if (_intWallSegTilemap != null && _intWallV != null)
                    _intWallSegTilemap.SetTile(new Vector3Int(x, y - 1, 0), _intWallV);
            }
        }

        private bool IsWallLike(int x, int y)
        {
            if (!_gridData.InBounds(x, y)) return false;
            var c = _gridData.GetCell(x, y);
            return c == CellType.Wall || c == CellType.InternalWall;
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
