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
        [SerializeField] private TileBase _wallHTile;
        [SerializeField] private TileBase _wallVTile;

        [Header("Preview Colors")]
        [SerializeField] private Color _validColor   = new(0.2f, 0.8f, 0.2f, 0.5f);
        [SerializeField] private Color _invalidColor = new(0.8f, 0.2f, 0.2f, 0.5f);

        // Flip matrix for bottom horizontal walls (Y scale −1, offset +1 to stay in cell)
        private static readonly Matrix4x4 FlipY = Matrix4x4.TRS(
            new Vector3(0, 1, 0), Quaternion.identity, new Vector3(1, -1, 1));

        private GridData _gridData;
        private RoomRegistry _roomRegistry;

        public GridData Data      => _gridData;
        public RoomRegistry Rooms => _roomRegistry;

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
                    $"Tilemap={_emptyTilemap != null}, Tile={_emptyTile != null}. " +
                    "Run 'Cat Hotel > Setup Proto Scene' from the menu.");
                return;
            }

            DrawBorderWalls();

            Debug.Log($"[GridRenderer] Grid initialized: {GridData.Width}x{GridData.Height}");
        }

        /// <summary>
        /// Show the empty cell grid (build mode).
        /// Places empty tiles only on cells that are still Empty.
        /// </summary>
        public void ShowGrid()
        {
            for (int y = 0; y < GridData.Height; y++)
                for (int x = 0; x < GridData.Width; x++)
                    if (_gridData.GetCell(x, y) == CellType.Empty)
                        _emptyTilemap.SetTile(new Vector3Int(x, y, 0), _emptyTile);
        }

        /// <summary>
        /// Hide the empty cell grid (normal mode).
        /// </summary>
        public void HideGrid()
        {
            _emptyTilemap.ClearAllTiles();
        }

        /// <summary>
        /// Draw wall tiles around the entire grid perimeter.
        /// </summary>
        private void DrawBorderWalls()
        {
            // Top & bottom
            for (int x = 0; x < GridData.Width; x++)
            {
                // Wall tiles temporarily disabled (visuals not final)
                // _wallTilemap.SetTile(new Vector3Int(x, 0, 0), _wallHTile);
                // _wallTilemap.SetTransformMatrix(new Vector3Int(x, 0, 0), FlipY);
                // _wallTilemap.SetTile(new Vector3Int(x, GridData.Height - 1, 0), _wallHTile);

                _gridData.SetCell(x, 0, CellType.Wall);
                _gridData.SetCell(x, GridData.Height - 1, CellType.Wall);
            }
            // Left & right
            for (int y = 1; y < GridData.Height - 1; y++)
            {
                // _wallTilemap.SetTile(new Vector3Int(0, y, 0), _wallVTile);
                // _wallTilemap.SetTile(new Vector3Int(GridData.Width - 1, y, 0), _wallVTile);
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
                            _floorTilemap.SetTile(pos, _floorTile);
                            break;
                        // Wall tiles temporarily disabled (visuals not final)
                        // case CellType.Wall:
                        //     var wallTile = GetWallTile(x, y);
                        //     _wallTilemap.SetTile(pos, wallTile);
                        //     if (wallTile == _wallHTile && IsBottomHWall(x, y))
                        //         _wallTilemap.SetTransformMatrix(pos, FlipY);
                        //     break;
                    }
                }
            }
        }

        /// <summary>
        /// A horizontal wall is "bottom" if floor is directly above it.
        /// </summary>
        private bool IsBottomHWall(int x, int y)
        {
            return _gridData.InBounds(x, y + 1) && _gridData.GetCell(x, y + 1) == CellType.Floor;
        }

        /// <summary>
        /// Determine wall orientation: horizontal if floor is above/below,
        /// vertical if floor is left/right only.
        /// </summary>
        private TileBase GetWallTile(int x, int y)
        {
            bool floorAboveOrBelow =
                (_gridData.InBounds(x, y - 1) && _gridData.GetCell(x, y - 1) == CellType.Floor) ||
                (_gridData.InBounds(x, y + 1) && _gridData.GetCell(x, y + 1) == CellType.Floor);

            if (floorAboveOrBelow) return _wallHTile;

            bool floorLeftOrRight =
                (_gridData.InBounds(x - 1, y) && _gridData.GetCell(x - 1, y) == CellType.Floor) ||
                (_gridData.InBounds(x + 1, y) && _gridData.GetCell(x + 1, y) == CellType.Floor);

            if (floorLeftOrRight) return _wallVTile;

            return _wallHTile; // default (border, corners)
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

                    // Wall preview temporarily disabled — show floor for all cells
                    var pos = new Vector3Int(x, y, 0);
                    _previewTilemap.SetTile(pos, _floorTile);
                }
            }
        }

        public void ClearPreview()
        {
            _previewTilemap.ClearAllTiles();
        }
    }
}
