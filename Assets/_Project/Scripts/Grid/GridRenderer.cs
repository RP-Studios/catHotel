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
        [SerializeField] private TileBase _wallTile;

        [Header("Preview Colors")]
        [SerializeField] private Color _validColor   = new(0.2f, 0.8f, 0.2f, 0.5f);
        [SerializeField] private Color _invalidColor = new(0.8f, 0.2f, 0.2f, 0.5f);

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

            for (int y = 0; y < GridData.Height; y++)
                for (int x = 0; x < GridData.Width; x++)
                    _emptyTilemap.SetTile(new Vector3Int(x, y, 0), _emptyTile);

            Debug.Log($"[GridRenderer] Grid initialized: {GridData.Width}x{GridData.Height} " +
                      $"({GridData.Width * GridData.Height} tiles placed)");
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
                        case CellType.Wall:
                            _wallTilemap.SetTile(pos, _wallTile);
                            break;
                    }
                }
            }
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

                    bool isPerimeter = x == rect.xMin || x == rect.xMax - 1 ||
                                       y == rect.yMin || y == rect.yMax - 1;
                    var pos = new Vector3Int(x, y, 0);
                    _previewTilemap.SetTile(pos, isPerimeter ? _wallTile : _floorTile);
                }
            }
        }

        public void ClearPreview()
        {
            _previewTilemap.ClearAllTiles();
        }
    }
}
