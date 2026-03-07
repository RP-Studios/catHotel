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
        [SerializeField] private TileBase _floorTile;

        [Header("Wall Tiles")]
        [SerializeField] private TileBase _wallTopTile;        // WALL_H: top wall face (visible height)
        [SerializeField] private TileBase _wallBotTile;        // WALL_BOT_Middle: bottom wall (thin line)
        [SerializeField] private TileBase _wallLeftTopTile;    // WALL_LEFT_Top: left wall cap
        [SerializeField] private TileBase _wallLeftMidTile;    // WALL_LEFT_Middle: left wall body
        [SerializeField] private TileBase _wallRightTopTile;   // WALL_RIGHT_Top: right wall cap
        [SerializeField] private TileBase _wallRightMidTile;   // WALL_RIGHT_Middle: right wall body

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

            DrawBorderWalls();
            BuildInitialLayout();
            RefreshAll();

            Debug.Log($"[GridRenderer] Grid initialized: {GridData.Width}x{GridData.Height}, " +
                $"Entrances: {Entrances.Count}, CentralFloor: {CentralRoomFloorCells.Count}");
        }

        private void BuildInitialLayout()
        {
            // Pension (5x5): x=1..5, y=18..22
            var pensionRect = new RectInt(1, 18, 5, 5);
            FillRoom(pensionRect);
            _roomRegistry.RegisterRoom(pensionRect);

            // Refuge (5x5): x=1..5, y=11..15
            var refugeRect = new RectInt(1, 11, 5, 5);
            FillRoom(refugeRect);
            _roomRegistry.RegisterRoom(refugeRect);

            // Central room: x=5..21, y=9..24 (shares wall x=5 with accueils)
            var centralRect = new RectInt(5, 9, 17, 16);
            FillRoom(centralRect);
            _roomRegistry.RegisterRoom(centralRect);

            // Doors: connect accueils → central at shared wall x=5
            _gridData.SetCell(5, 20, CellType.Door); // pension
            _gridData.SetCell(5, 13, CellType.Door); // refuge

            // Entrance doors (left wall x=1)
            _gridData.SetCell(1, 20, CellType.Door);
            Entrances.Add(new Vector2Int(1, 20));

            _gridData.SetCell(1, 13, CellType.Door);
            Entrances.Add(new Vector2Int(1, 13));

            // Cache central room floor cells
            for (int y = centralRect.yMin + 1; y < centralRect.yMax - 1; y++)
                for (int x = centralRect.xMin + 1; x < centralRect.xMax - 1; x++)
                    CentralRoomFloorCells.Add(new Vector2Int(x, y));
        }

        private void FillRoom(RectInt rect)
        {
            for (int y = rect.yMin; y < rect.yMax; y++)
            {
                for (int x = rect.xMin; x < rect.xMax; x++)
                {
                    bool isPerimeter = x == rect.xMin || x == rect.xMax - 1 ||
                                       y == rect.yMin || y == rect.yMax - 1;

                    if (isPerimeter)
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
                    switch (_gridData.GetCell(x, y))
                    {
                        case CellType.Floor:
                        case CellType.Door:
                            _floorTilemap.SetTile(pos, _floorTile);
                            break;
                        case CellType.Wall:
                            var wallTile = GetWallTile(x, y);
                            if (wallTile != null)
                                _wallTilemap.SetTile(pos, wallTile);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Pick the correct wall tile based on where adjacent floor/door cells are.
        /// - Floor below → top wall (WALL_H, visible face)
        /// - Floor above → bottom wall (WALL_BOT, thin line from above)
        /// - Floor to the right → left wall (WALL_LEFT variants)
        /// - Floor to the left → right wall (WALL_RIGHT variants)
        /// Returns null for wall cells with no adjacent floor (invisible border).
        /// </summary>
        private TileBase GetWallTile(int x, int y)
        {
            bool floorBelow = _gridData.InBounds(x, y - 1) && _gridData.IsWalkable(x, y - 1);
            bool floorAbove = _gridData.InBounds(x, y + 1) && _gridData.IsWalkable(x, y + 1);
            bool floorRight = _gridData.InBounds(x + 1, y) && _gridData.IsWalkable(x + 1, y);
            bool floorLeft  = _gridData.InBounds(x - 1, y) && _gridData.IsWalkable(x - 1, y);

            // Top wall (most prominent — visible wall face from above)
            if (floorBelow) return _wallTopTile;

            // Bottom wall (thin top-down line)
            if (floorAbove) return _wallBotTile;

            // Left wall (floor is to the right of this wall)
            if (floorRight)
            {
                // Use _Top variant if the cell above is NOT also a left-wall
                bool aboveIsLeftWall = _gridData.InBounds(x, y + 1)
                    && _gridData.GetCell(x, y + 1) == CellType.Wall
                    && _gridData.InBounds(x + 1, y + 1)
                    && _gridData.IsWalkable(x + 1, y + 1);
                return aboveIsLeftWall ? _wallLeftMidTile : _wallLeftTopTile;
            }

            // Right wall (floor is to the left of this wall)
            if (floorLeft)
            {
                bool aboveIsRightWall = _gridData.InBounds(x, y + 1)
                    && _gridData.GetCell(x, y + 1) == CellType.Wall
                    && _gridData.InBounds(x - 1, y + 1)
                    && _gridData.IsWalkable(x - 1, y + 1);
                return aboveIsRightWall ? _wallRightMidTile : _wallRightTopTile;
            }

            // No adjacent floor — border wall with no nearby rooms → invisible
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
