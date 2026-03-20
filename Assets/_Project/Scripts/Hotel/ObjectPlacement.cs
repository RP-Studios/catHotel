using UnityEngine;
using UnityEngine.InputSystem;
using CatHotel.Core;
using CatHotel.Grid;
using CatHotel.Economy;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Handles buying objects from the shop and placing them on the grid.
    /// Flow: ShopPanel calls BeginPlacement → object follows pointer → tap to place →
    /// hold+drag to reposition → Ready to confirm, Cancel to abort.
    /// </summary>
    public class ObjectPlacement : MonoBehaviour
    {
        [SerializeField] private GridRenderer _gridRenderer;
        [SerializeField] private EconomyManager _economy;

        [Header("Button Sprites")]
        [SerializeField] private Sprite[] _readyFrames;
        [SerializeField] private Sprite[] _cancelFrames;

        private HotelObjectData _currentData;
        private GameObject _previewObj;
        private SpriteRenderer _previewSr;
        private Vector2Int _currentGridPos;
        private bool _isPlacing;
        private bool _isDragging;
        private bool _isValid;

        // Animated action buttons (world-space sprites)
        private GameObject _readyBtnObj;
        private GameObject _cancelBtnObj;
        private SpriteRenderer _readySr;
        private SpriteRenderer _cancelSr;
        private float _animTimer;
        private const float FrameInterval = 0.1f; // 10 FPS
        private int _animFrame;

        // Button sizes in world units
        private const float ReadySize = 0.7f;
        private const float CancelSize = 0.5f;
        private const float BtnOffsetY = 0.1f; // gap between object top and buttons

        private Camera _cam;

        public bool IsPlacing => _isPlacing;

        private void Start()
        {
            _cam = Camera.main;
        }

        private void Update()
        {
            if (!_isPlacing) return;

            HandleDrag();
            AnimateButtons();
            HandleTap();
        }

        /// <summary>
        /// Start placement mode for a given object.
        /// Called by ShopPanel after purchase tap.
        /// </summary>
        public void BeginPlacement(HotelObjectData data)
        {
            if (_isPlacing) CancelPlacement();

            _currentData = data;
            _isPlacing = true;
            _isDragging = false;

            // Create preview object
            _previewObj = new GameObject($"Preview_{data.displayName}");
            _previewSr = _previewObj.AddComponent<SpriteRenderer>();
            _previewSr.sprite = data.icon;
            _previewSr.sortingOrder = 15;
            _previewSr.color = new Color(1f, 1f, 1f, 0.7f);

            // Scale to fit grid size
            ScaleToFit(_previewObj, _previewSr, data);

            // Place at the center of the camera view, on the nearest floor cell
            _currentGridPos = FindVisibleFloorCell();

            UpdatePreviewPosition();
            CreateButtons();
            UpdateValidity();
        }

        private void HandleDrag()
        {
            var pointer = Pointer.current;
            if (pointer == null) return;

            // Start drag on press
            if (pointer.press.wasPressedThisFrame)
            {
                Vector2 screenPos = pointer.position.ReadValue();
                Vector3 worldPos = _cam.ScreenToWorldPoint(screenPos);

                // Check if tapping on the preview object
                float dist = Vector2.Distance(worldPos, _previewObj.transform.position);
                if (dist < Mathf.Max(_currentData.size.x, _currentData.size.y) * 0.8f)
                {
                    _isDragging = true;
                }
            }

            // End drag on release
            if (pointer.press.wasReleasedThisFrame)
            {
                _isDragging = false;
            }

            // Move while dragging
            if (_isDragging)
            {
                Vector2 screenPos = pointer.position.ReadValue();
                Vector3 worldPos = _cam.ScreenToWorldPoint(screenPos);
                int gx = Mathf.FloorToInt(worldPos.x);
                int gy = Mathf.FloorToInt(worldPos.y);
                var newPos = new Vector2Int(gx, gy);

                if (newPos != _currentGridPos)
                {
                    _currentGridPos = newPos;
                    UpdatePreviewPosition();
                    UpdateValidity();
                }
            }
        }

        private void HandleTap()
        {
            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;
            if (_isDragging) return;

            Vector2 screenPos = pointer.position.ReadValue();
            Vector3 worldPos = _cam.ScreenToWorldPoint(screenPos);

            // Check Ready button tap
            if (_readyBtnObj != null)
            {
                float dist = Vector2.Distance(worldPos, _readyBtnObj.transform.position);
                if (dist < ReadySize * 0.6f)
                {
                    if (_isValid) ConfirmPlacement();
                    return;
                }
            }

            // Check Cancel button tap
            if (_cancelBtnObj != null)
            {
                float dist = Vector2.Distance(worldPos, _cancelBtnObj.transform.position);
                if (dist < CancelSize * 0.6f)
                {
                    CancelPlacement();
                    return;
                }
            }

            // Tap on grid = reposition
            int gx = Mathf.FloorToInt(worldPos.x);
            int gy = Mathf.FloorToInt(worldPos.y);
            var gridData = _gridRenderer.Data;
            if (gridData != null && gridData.InBounds(gx, gy))
            {
                _currentGridPos = new Vector2Int(gx, gy);
                UpdatePreviewPosition();
                UpdateValidity();
            }
        }

        private void UpdatePreviewPosition()
        {
            if (_previewObj == null) return;
            float cx = _currentGridPos.x + _currentData.size.x * 0.5f;
            float cy = _currentGridPos.y + _currentData.size.y * 0.5f;
            _previewObj.transform.position = new Vector3(cx, cy, 0f);
            UpdateButtonPositions();
        }

        private void UpdateValidity()
        {
            var gridData = _gridRenderer.Data;
            if (gridData == null) { _isValid = false; return; }

            var rect = new RectInt(_currentGridPos, _currentData.size);

            // Check all cells are walkable floor
            bool allFloor = true;
            for (int y = rect.yMin; y < rect.yMax; y++)
                for (int x = rect.xMin; x < rect.xMax; x++)
                {
                    if (!gridData.InBounds(x, y) || !gridData.IsWalkable(x, y))
                    {
                        allFloor = false;
                        break;
                    }
                    // Don't place on Door cells
                    if (gridData.GetCell(x, y) == CellType.Door)
                    {
                        allFloor = false;
                        break;
                    }
                }

            // Check no other object occupies the area
            bool areaFree = ObjectRegistry.IsAreaFree(rect);

            _isValid = allFloor && areaFree;

            // Update preview tilemap
            _gridRenderer.ShowPreview(rect, _isValid);

            // Tint preview sprite
            _previewSr.color = _isValid
                ? new Color(1f, 1f, 1f, 0.8f)
                : new Color(1f, 0.4f, 0.4f, 0.6f);

            // Dim ready button if invalid
            if (_readySr != null)
                _readySr.color = _isValid ? Color.white : new Color(1f, 1f, 1f, 0.3f);
        }

        private void ConfirmPlacement()
        {
            if (!_isValid || _currentData == null) return;

            // Debit coins
            if (!_economy.TrySpend(_currentData.cost))
            {
                Debug.LogWarning("[Placement] Not enough coins");
                return;
            }

            // Create the real HotelObject
            var go = new GameObject($"Obj_{_currentData.displayName}");
            go.transform.position = new Vector3(
                _currentGridPos.x + _currentData.size.x * 0.5f,
                _currentGridPos.y + _currentData.size.y * 0.5f, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _currentData.worldSprite != null ? _currentData.worldSprite : _currentData.icon;
            sr.sortingOrder = 5;

            // Scale to fit
            ScaleToFit(go, sr, _currentData);

            var hotelObj = go.AddComponent<HotelObject>();
            hotelObj.Init(_currentData, _currentGridPos);
            // HotelObject.OnEnable will call ObjectRegistry.Register automatically

            string placedName = _currentData.displayName;
            Vector2Int placedPos = _currentGridPos;
            CleanupPlacement();
            Debug.Log($"[Placement] Placed {placedName} at {placedPos}");
        }

        public void CancelPlacement()
        {
            CleanupPlacement();
        }

        private void CleanupPlacement()
        {
            _isPlacing = false;
            _isDragging = false;
            _currentData = null;

            if (_previewObj != null) Destroy(_previewObj);
            if (_readyBtnObj != null) Destroy(_readyBtnObj);
            if (_cancelBtnObj != null) Destroy(_cancelBtnObj);

            _gridRenderer.ClearPreview();
        }

        private void CreateButtons()
        {
            // Ready button (top-right of object)
            _readyBtnObj = new GameObject("ReadyBtn");
            _readySr = _readyBtnObj.AddComponent<SpriteRenderer>();
            _readySr.sortingOrder = 25;
            if (_readyFrames != null && _readyFrames.Length > 0)
                _readySr.sprite = _readyFrames[0];
            SetSpriteWorldSize(_readyBtnObj, _readySr, ReadySize);

            // Cancel button (bottom-right of object)
            _cancelBtnObj = new GameObject("CancelBtn");
            _cancelSr = _cancelBtnObj.AddComponent<SpriteRenderer>();
            _cancelSr.sortingOrder = 25;
            if (_cancelFrames != null && _cancelFrames.Length > 0)
                _cancelSr.sprite = _cancelFrames[0];
            SetSpriteWorldSize(_cancelBtnObj, _cancelSr, CancelSize);

            _animFrame = 0;
            _animTimer = 0f;
            UpdateButtonPositions();
        }

        private void UpdateButtonPositions()
        {
            if (_previewObj == null) return;

            float right = _currentGridPos.x + _currentData.size.x + 0.1f;
            float top = _currentGridPos.y + _currentData.size.y;
            float bottom = _currentGridPos.y;

            if (_readyBtnObj != null)
                _readyBtnObj.transform.position = new Vector3(right + ReadySize * 0.5f, top - ReadySize * 0.5f + BtnOffsetY, 0f);
            if (_cancelBtnObj != null)
                _cancelBtnObj.transform.position = new Vector3(right + CancelSize * 0.5f, bottom + CancelSize * 0.5f - BtnOffsetY, 0f);
        }

        private void AnimateButtons()
        {
            _animTimer += Time.deltaTime;
            if (_animTimer < FrameInterval) return;
            _animTimer = 0f;

            _animFrame++;

            if (_readyFrames != null && _readyFrames.Length > 0 && _readySr != null)
                _readySr.sprite = _readyFrames[_animFrame % _readyFrames.Length];

            if (_cancelFrames != null && _cancelFrames.Length > 0 && _cancelSr != null)
                _cancelSr.sprite = _cancelFrames[_animFrame % _cancelFrames.Length];
        }

        private static void ScaleToFit(GameObject go, SpriteRenderer sr, HotelObjectData data)
        {
            if (sr.sprite == null) return;
            float spriteW = sr.sprite.bounds.size.x;
            float spriteH = sr.sprite.bounds.size.y;
            if (spriteW <= 0f || spriteH <= 0f) return;

            float targetW = data.size.x;
            float targetH = data.size.y;
            float scale = Mathf.Min(targetW / spriteW, targetH / spriteH) * data.visualScale;
            go.transform.localScale = new Vector3(scale, scale, 1f);
        }

        /// <summary>
        /// Find a floor cell near the center of the current camera view.
        /// </summary>
        private Vector2Int FindVisibleFloorCell()
        {
            var gridData = _gridRenderer.Data;
            Vector3 camCenter = _cam.transform.position;
            int cx = Mathf.FloorToInt(camCenter.x);
            int cy = Mathf.FloorToInt(camCenter.y);

            // Spiral search outward from camera center to find nearest walkable, non-door cell
            for (int radius = 0; radius < 30; radius++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        if (Mathf.Abs(dx) != radius && Mathf.Abs(dy) != radius) continue;
                        int x = cx + dx;
                        int y = cy + dy;
                        if (!gridData.InBounds(x, y)) continue;
                        if (!gridData.IsWalkable(x, y)) continue;
                        if (gridData.GetCell(x, y) == CellType.Door) continue;
                        return new Vector2Int(x, y);
                    }
                }
            }

            // Fallback
            var floorCells = _gridRenderer.CentralRoomFloorCells;
            return floorCells.Count > 0 ? floorCells[floorCells.Count / 2] : new Vector2Int(10, 7);
        }

        private static void SetSpriteWorldSize(GameObject go, SpriteRenderer sr, float worldSize)
        {
            if (sr.sprite == null) return;
            float spriteSize = Mathf.Max(sr.sprite.bounds.size.x, sr.sprite.bounds.size.y);
            if (spriteSize <= 0f) return;
            float scale = worldSize / spriteSize;
            go.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
