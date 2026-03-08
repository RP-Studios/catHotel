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
        [SerializeField] private Tilemap _previewTilemap;

        [Header("Tiles")]
        [SerializeField] private TileBase _emptyTile;
        [SerializeField] private TileBase[] _floorTiles;

        private TileBase _floorTile;

        [Header("Wall Tiles")]
        [SerializeField] private TileBase _wallTopTile;        // WALL_H
        [SerializeField] private TileBase _wallBotTile;        // WALL_BOT_Middle
        [SerializeField] private TileBase _wallLeftTopTile;    // WALL_LEFT_Top
        [SerializeField] private TileBase _wallLeftMidTile;    // WALL_LEFT_Middle
        [SerializeField] private TileBase _wallRightTopTile;   // WALL_RIGHT_Top
        [SerializeField] private TileBase _wallRightMidTile;   // WALL_RIGHT_Middle
        [SerializeField] private TileBase _wallTopLeftTile;    // WALL_H_Left
        [SerializeField] private TileBase _wallTopRightTile;   // WALL_H_Right

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

            // Pick a random floor tile at launch
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
            // Single main room: floor first, then walls around it
            var centralRect = new RectInt(5, 9, 17, 16);
            FillRoom(centralRect);
            _roomRegistry.RegisterRoom(centralRect);

            for (int y = centralRect.yMin + 1; y < centralRect.yMax - 1; y++)
                for (int x = centralRect.xMin; x < centralRect.xMax; x++)
                    CentralRoomFloorCells.Add(new Vector2Int(x, y));
        }

        /// <summary>
        /// Only top/bottom perimeter = Wall. Left/right edges + interior = Floor.
        /// Vertical walls are rendered as overlays on edge floor cells.
        /// </summary>
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
            for (int y = 0; y < GridData.Height; y++)
                for (int x = 0; x < GridData.Width; x++)
                    if (_gridData.GetCell(x, y) == CellType.Empty)
                        _emptyTilemap.SetTile(new Vector3Int(x, y, 0), _emptyTile);
        }

        public void HideGrid()
        {
            _emptyTilemap.ClearAllTiles();
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

                        // Vertical wall overlay on edge floor cells (adjacent to empty/out-of-bounds)
                        bool edgeLeft  = !_gridData.InBounds(x - 1, y) || _gridData.GetCell(x - 1, y) == CellType.Empty;
                        bool edgeRight = !_gridData.InBounds(x + 1, y) || _gridData.GetCell(x + 1, y) == CellType.Empty;

                        if (edgeLeft && !edgeRight)
                            _wallTilemap.SetTile(pos, _wallRightMidTile);
                        else if (edgeRight && !edgeLeft)
                            _wallTilemap.SetTile(pos, _wallLeftMidTile);
                    }
                    else if (cell == CellType.Wall)
                    {
                        var tile = GetWallTile(x, y);
                        if (tile != null)
                            _wallTilemap.SetTile(pos, tile);
                    }
                }
            }
        }

        /// <summary>
        /// Simple cardinal check: which side has floor?
        /// - Floor below → WALL_H
        /// - Floor above → WALL_BOT_Middle
        /// - Floor right → WALL_LEFT_Middle
        /// - Floor left  → WALL_RIGHT_Middle
        /// </summary>
        private TileBase GetWallTile(int x, int y)
        {
            bool floorBelow = _gridData.InBounds(x, y - 1) && _gridData.IsWalkable(x, y - 1);
            bool floorAbove = _gridData.InBounds(x, y + 1) && _gridData.IsWalkable(x, y + 1);
            bool floorRight = _gridData.InBounds(x + 1, y) && _gridData.IsWalkable(x + 1, y);
            bool floorLeft  = _gridData.InBounds(x - 1, y) && _gridData.IsWalkable(x - 1, y);

            // Top wall row: replace leftmost/rightmost with corner tiles
            if (floorBelow)
            {
                // Is the neighbor also a top-wall tile? (= also has floor below it)
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
            {
                for (int x = rect.xMin; x < rect.xMax; x++)
                {
                    if (!_gridData.InBounds(x, y)) continue;
                    _previewTilemap.SetTile(new Vector3Int(x, y, 0), _floorTile);
                }
            }
        }

        public void ClearPreview()
        {
            _previewTilemap.ClearAllTiles();
        }
    }
}
