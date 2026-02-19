using UnityEngine;
using UnityEngine.InputSystem;
using CatHotel.Grid;

namespace CatHotel.Input
{
    public class RoomBuilderInput : MonoBehaviour
    {
        [SerializeField] private GridRenderer _gridRenderer;
        [SerializeField] private Camera _camera;

        private CameraController _camController;
        private bool _buildMode;
        private bool _isDragging;
        private Vector2Int _dragStart;

        public bool BuildMode => _buildMode;

        private void Start()
        {
            _camController = _camera.GetComponent<CameraController>();
        }

        private void Update()
        {
            HandleBuildModeToggle();

            if (!_buildMode) return;

            var pointer = Pointer.current;
            if (pointer == null) return;

            Vector2 screenPos = pointer.position.ReadValue();
            if (float.IsInfinity(screenPos.x) || float.IsNaN(screenPos.x))
                return;

            Vector2Int gridPos = ScreenToGrid(screenPos);

            if (pointer.press.wasPressedThisFrame)
            {
                if (_gridRenderer.Data.InBounds(gridPos.x, gridPos.y))
                {
                    var cell = _gridRenderer.Data.GetCell(gridPos.x, gridPos.y);
                    if (cell == CellType.Empty || cell == CellType.Wall)
                    {
                        _isDragging = true;
                        _dragStart = gridPos;
                    }
                }
            }

            if (_isDragging && pointer.press.isPressed)
            {
                RectInt rect = MakeRect(_dragStart, gridPos);
                bool valid = _gridRenderer.Rooms.CanCreateRoom(rect);
                _gridRenderer.ShowPreview(rect, valid);
            }

            if (_isDragging && pointer.press.wasReleasedThisFrame)
            {
                _isDragging = false;
                RectInt rect = MakeRect(_dragStart, gridPos);

                var room = _gridRenderer.Rooms.TryCreateRoom(rect);
                if (room != null)
                {
                    _gridRenderer.RefreshAll();
                    Debug.Log($"Room #{room.Id} created: {room.Bounds} " +
                              $"({room.TotalCells} cells, {room.InteriorCells} interior)");
                }

                _gridRenderer.ClearPreview();
            }
        }

        private void HandleBuildModeToggle()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.bKey.wasPressedThisFrame)
                SetBuildMode(!_buildMode);
        }

        public void SetBuildMode(bool active)
        {
            if (_buildMode == active) return;
            _buildMode = active;

            if (_buildMode)
            {
                _gridRenderer.ShowGrid();
                if (_camController != null) _camController.PanLocked = true;
                Debug.Log("[Build] Build mode ON (B to exit)");
            }
            else
            {
                _isDragging = false;
                _gridRenderer.ClearPreview();
                _gridRenderer.HideGrid();
                if (_camController != null) _camController.PanLocked = false;
                Debug.Log("[Build] Build mode OFF");
            }
        }

        private Vector2Int ScreenToGrid(Vector2 screenPos)
        {
            Vector3 worldPos = _camera.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, 0f));
            return new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y));
        }

        private static RectInt MakeRect(Vector2Int a, Vector2Int b)
        {
            int xMin = Mathf.Min(a.x, b.x);
            int yMin = Mathf.Min(a.y, b.y);
            int xMax = Mathf.Max(a.x, b.x);
            int yMax = Mathf.Max(a.y, b.y);
            return new RectInt(xMin, yMin, xMax - xMin + 1, yMax - yMin + 1);
        }
    }
}
