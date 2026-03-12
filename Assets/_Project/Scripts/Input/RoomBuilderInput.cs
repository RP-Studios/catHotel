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

            if (pointer.press.wasPressedThisFrame)
            {
                Vector2Int gridPos = ScreenToGrid(screenPos);
                TryPlaceWall(gridPos);
            }
        }

        private static readonly Vector2Int[] CardinalDirs =
        {
            Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down
        };

        private void TryPlaceWall(Vector2Int pos)
        {
            if (_gridRenderer.PlaceInternalWall(pos))
            {

#if UNITY_EDITOR
                Debug.Log($"[Build] Wall placed at {pos}");
#endif
                return;
            }

            // If click landed on Empty cell (e.g. just outside room edge), try neighbors
            foreach (var dir in CardinalDirs)
            {
                var neighbor = pos + dir;
                if (_gridRenderer.PlaceInternalWall(neighbor))
                {
#if UNITY_EDITOR
                    Debug.Log($"[Build] Wall placed at {neighbor} (snapped from {pos})");
#endif
                    return;
                }
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

#if UNITY_EDITOR
                Debug.Log("[Build] Build mode ON (B to exit)");
#endif
            }
            else
            {
                _gridRenderer.ClearPreview();
                _gridRenderer.HideGrid();
                if (_camController != null) _camController.PanLocked = false;
#if UNITY_EDITOR
                Debug.Log("[Build] Build mode OFF");
#endif
            }
        }

        /// <summary>
        /// Snap to the nearest cell corner (grid intersection) rather than cell center.
        /// </summary>
        private Vector2Int ScreenToGrid(Vector2 screenPos)
        {
            Vector3 worldPos = _camera.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, 0f));
            return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
        }
    }
}
