using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CatHotel.Core;
using CatHotel.Grid;
using CatHotel.Input;
using CatHotel.Economy;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Handles long-press to move placed objects and drag-to-sell.
    /// Flow: long press on selected object → enter move mode → drag to reposition →
    /// Ready to confirm, Cancel to restore, drag to ItemSellAction to sell.
    /// </summary>
    public class ObjectMover : MonoBehaviour
    {
        [SerializeField] private ObjectSelector _selector;
        [SerializeField] private ObjectPlacement _placement;
        [SerializeField] private GridRenderer _gridRenderer;
        [SerializeField] private EconomyManager _economy;
        [SerializeField] private CameraController _cameraController;

        [Header("Long Press")]
        [SerializeField] private float _longPressDuration = 0.5f;
        [SerializeField] private float _longPressScaleUp = 1.15f;

        [Header("Edge Scroll")]
        [SerializeField] private float _edgeScrollMargin = 0.1f; // % of screen
        [SerializeField] private float _edgeScrollSpeed = 5f;

        [Header("Button Sprites")]
        [SerializeField] private Sprite[] _readyFrames;
        [SerializeField] private Sprite[] _cancelFrames;

        private Camera _cam;
        private bool _isMoving;
        private HotelObject _movingObject;
        private Vector2Int _originalGridPos;
        private Vector3 _originalWorldPos;
        private Vector3 _originalScale;

        // Long press tracking
        private float _pressTimer;
        private bool _isPressing;
        private bool _longPressTriggered;
        private Tween _scaleUpTween;

        // Move mode
        private SpriteRenderer _movingSr;
        private Vector2Int _currentGridPos;
        private bool _isDragging;
        private bool _isValid;

        // Buttons
        private GameObject _readyBtnObj;
        private GameObject _cancelBtnObj;
        private SpriteRenderer _readySr;
        private SpriteRenderer _cancelSr;
        private float _animTimer;
        private int _animFrame;
        private const float FrameInterval = 0.1f;
        private const float ReadySize = 0.7f;
        private const float CancelSize = 0.5f;

        // Sell zone
        private RectTransform _sellActionRect;
        private TMP_Text _sellValueLabel;
        private GameObject _sellActionObj;
        private bool _isOverSell;
        private Tween _sellPulseTween;

        public bool IsMoving => _isMoving;

        private void Start()
        {
            _cam = Camera.main;

            // Find sell zone UI (search inactive objects too)
            _sellActionObj = FindInactiveByName("ItemSellAction");
            if (_sellActionObj != null)
            {
                _sellActionRect = _sellActionObj.GetComponent<RectTransform>();
                _sellActionObj.SetActive(false);

                var labelT = _sellActionObj.transform.Find("ItemSellValueLabel");
                if (labelT == null)
                    labelT = FindInChildren(_sellActionObj.transform, "ItemSellValueLabel");
                if (labelT != null)
                    _sellValueLabel = labelT.GetComponent<TMP_Text>();
            }
        }

        private void Update()
        {
            if (_placement != null && _placement.IsPlacing) return;

            if (_isMoving)
            {
                HandleMoveDrag();
                AnimateButtons();
                HandleEdgeScroll();
                return;
            }

            HandleLongPress();
        }

        // ======================== LONG PRESS ========================

        private void HandleLongPress()
        {
            var pointer = Pointer.current;
            if (pointer == null) return;

            if (pointer.press.wasPressedThisFrame)
            {
                // Only start long press on a selected object
                if (_selector == null || _selector.Selected == null) return;
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

                _isPressing = true;
                _pressTimer = 0f;
                _longPressTriggered = false;

                // Start scale-up feedback
                var sr = _selector.Selected.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    _scaleUpTween?.Kill();
                    _scaleUpTween = sr.transform.DOScale(
                        _originalScale != Vector3.zero ? _originalScale * _longPressScaleUp : sr.transform.localScale * _longPressScaleUp,
                        _longPressDuration).SetEase(Ease.OutQuad);
                    _originalScale = sr.transform.localScale;
                }
            }

            if (_isPressing && pointer.press.isPressed)
            {
                _pressTimer += Time.deltaTime;

                if (_pressTimer >= _longPressDuration && !_longPressTriggered)
                {
                    _longPressTriggered = true;
                    EnterMoveMode(_selector.Selected);
                }
            }

            if (_isPressing && pointer.press.wasReleasedThisFrame)
            {
                _isPressing = false;
                if (!_longPressTriggered)
                {
                    // Cancel scale-up if released before long press
                    _scaleUpTween?.Kill();
                    if (_selector.Selected != null)
                        _selector.Selected.transform.localScale = _originalScale;
                }
            }
        }

        // ======================== MOVE MODE ========================

        private void EnterMoveMode(HotelObject obj)
        {
            _isMoving = true;
            _movingObject = obj;
            _movingSr = obj.GetComponent<SpriteRenderer>();
            _originalGridPos = obj.GridPos;
            _originalWorldPos = obj.transform.position;
            _currentGridPos = _originalGridPos;
            _isDragging = true;

            // Deselect (remove SELEC sprite)
            _selector.DeselectCurrent();

            // Unregister from grid temporarily
            ObjectRegistry.Unregister(obj);

            // Semi-transparent while moving
            if (_movingSr != null)
                _movingSr.color = new Color(1f, 1f, 1f, 0.7f);

            // Lock camera
            if (_cameraController != null)
                _cameraController.PanLocked = true;

            // Show sell zone
            ShowSellZone(obj.Data);

            // Create buttons
            CreateButtons();
            UpdateValidity();
        }

        private void ExitMoveMode()
        {
            _isMoving = false;
            _isDragging = false;

            // Restore opacity
            if (_movingSr != null)
                _movingSr.color = Color.white;

            // Restore scale
            if (_movingObject != null)
                _movingObject.transform.localScale = _originalScale;

            // Unlock camera
            if (_cameraController != null)
                _cameraController.PanLocked = false;

            // Hide sell zone
            HideSellZone();

            // Destroy buttons
            if (_readyBtnObj != null) Destroy(_readyBtnObj);
            if (_cancelBtnObj != null) Destroy(_cancelBtnObj);

            // Clear preview
            _gridRenderer.ClearPreview();

            _movingObject = null;
            _movingSr = null;
        }

        private void ConfirmMove()
        {
            if (_movingObject == null) return;

            // Update grid position
            _movingObject.Init(_movingObject.Data, _currentGridPos);
            ObjectRegistry.Register(_movingObject);

            ExitMoveMode();
        }

        private void CancelMove()
        {
            if (_movingObject == null) return;

            // Restore to original position
            _movingObject.transform.position = _originalWorldPos;
            _movingObject.Init(_movingObject.Data, _originalGridPos);
            ObjectRegistry.Register(_movingObject);

            ExitMoveMode();
        }

        private void SellObject()
        {
            if (_movingObject == null) return;

            int sellPrice = _movingObject.Data.SellPrice;
            _economy.AddCoins(sellPrice);

            // Destroy the object
            var obj = _movingObject;
            ExitMoveMode();
            Destroy(obj.gameObject);
        }

        // ======================== DRAG ========================

        private Vector2 _pressStartScreen;
        private const float DragThreshold = 15f; // pixels before drag starts

        private void HandleMoveDrag()
        {
            var pointer = Pointer.current;
            if (pointer == null || _movingObject == null) return;

            Vector2 screenPos = pointer.position.ReadValue();
            if (float.IsNaN(screenPos.x) || float.IsNaN(screenPos.y)) return;

            // Track press start for drag threshold
            if (pointer.press.wasPressedThisFrame)
            {
                _pressStartScreen = screenPos;
                _isDragging = false;

                // Check buttons FIRST on press
                Vector3 worldTap = _cam.ScreenToWorldPoint(screenPos);

                if (_readyBtnObj != null &&
                    Vector2.Distance(worldTap, _readyBtnObj.transform.position) < ReadySize * 0.6f)
                {
                    if (_isValid) ConfirmMove();
                    return;
                }
                if (_cancelBtnObj != null &&
                    Vector2.Distance(worldTap, _cancelBtnObj.transform.position) < CancelSize * 0.6f)
                {
                    CancelMove();
                    return;
                }
            }

            // Release
            if (pointer.press.wasReleasedThisFrame)
            {
                if (_isDragging && _isOverSell)
                {
                    SellObject();
                    return;
                }
                _isDragging = false;
                return;
            }

            // Hold — start drag only after moving past threshold
            if (pointer.press.isPressed)
            {
                if (!_isDragging)
                {
                    if (Vector2.Distance(screenPos, _pressStartScreen) > DragThreshold)
                        _isDragging = true;
                    else
                        return;
                }

                // Dragging — move object
                CheckSellZoneHover(screenPos);

                Vector3 worldPos = _cam.ScreenToWorldPoint(screenPos);
                var data = _movingObject.Data;

                int gx = Mathf.FloorToInt(worldPos.x);
                int gy = data.wallMount ? GetTopWallY() : Mathf.FloorToInt(worldPos.y);
                var newPos = new Vector2Int(gx, gy);

                if (newPos != _currentGridPos)
                {
                    _currentGridPos = newPos;
                    UpdateObjectPosition();
                    UpdateValidity();
                }
            }
        }

        private void UpdateObjectPosition()
        {
            if (_movingObject == null) return;
            var data = _movingObject.Data;
            float cx = _currentGridPos.x + data.size.x * 0.5f;
            float cy = data.wallMount ? _currentGridPos.y + 0.45f : _currentGridPos.y + 0.25f;
            _movingObject.transform.position = new Vector3(cx, cy, 0f);
            UpdateButtonPositions();
        }

        private void UpdateValidity()
        {
            var gridData = _gridRenderer.Data;
            var data = _movingObject.Data;
            if (gridData == null) { _isValid = false; return; }

            var rect = new RectInt(_currentGridPos, data.size);

            bool cellsOk = true;
            if (data.wallMount)
            {
                for (int x = rect.xMin; x < rect.xMax; x++)
                {
                    if (!gridData.InBounds(x, _currentGridPos.y) ||
                        gridData.GetCell(x, _currentGridPos.y) != CellType.Wall)
                    {
                        cellsOk = false;
                        break;
                    }
                }
            }
            else
            {
                for (int y = rect.yMin; y < rect.yMax; y++)
                    for (int x = rect.xMin; x < rect.xMax; x++)
                    {
                        if (!gridData.InBounds(x, y) || !gridData.IsWalkable(x, y))
                        { cellsOk = false; break; }
                        if (gridData.GetCell(x, y) == CellType.Door)
                        { cellsOk = false; break; }
                    }
            }

            bool areaFree = ObjectRegistry.IsAreaFree(rect);
            _isValid = cellsOk && areaFree;

            _gridRenderer.ShowPreview(rect, _isValid);

            if (_movingSr != null)
                _movingSr.color = _isValid
                    ? new Color(1f, 1f, 1f, 0.8f)
                    : new Color(1f, 0.4f, 0.4f, 0.6f);

            if (_readySr != null)
                _readySr.color = _isValid ? Color.white : new Color(1f, 1f, 1f, 0.3f);
        }

        // ======================== SELL ZONE ========================

        private void ShowSellZone(HotelObjectData data)
        {
            if (_sellActionObj == null) return;
            _sellActionObj.SetActive(true);

            if (_sellValueLabel != null)
                _sellValueLabel.text = $"{data.SellPrice}";
        }

        private void HideSellZone()
        {
            _isOverSell = false;
            _sellPulseTween?.Kill();
            if (_sellActionObj != null)
            {
                if (_sellActionRect != null)
                {
                    _sellActionRect.localScale = Vector3.one;
                }
                _sellActionObj.SetActive(false);
            }
        }

        private void CheckSellZoneHover(Vector2 screenPos)
        {
            if (_sellActionRect == null) { _isOverSell = false; return; }

            bool wasOver = _isOverSell;
            _isOverSell = RectTransformUtility.RectangleContainsScreenPoint(_sellActionRect, screenPos, null);

            // Start pulse when entering sell zone
            if (_isOverSell && !wasOver)
            {
                _sellPulseTween?.Kill();
                _sellActionRect.localScale = Vector3.one;
                _sellPulseTween = _sellActionRect.DOScale(Vector3.one * 1.15f, 0.3f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
            // Stop pulse when leaving
            else if (!_isOverSell && wasOver)
            {
                _sellPulseTween?.Kill();
                _sellActionRect.localScale = Vector3.one;
            }
        }

        // ======================== EDGE SCROLL ========================

        private void HandleEdgeScroll()
        {
            if (!_isDragging || _cameraController == null) return;

            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.isPressed) return;

            Vector2 screenPos = pointer.position.ReadValue();
            if (float.IsNaN(screenPos.x) || float.IsNaN(screenPos.y)) return;

            float sw = Screen.width;
            float sh = Screen.height;
            float margin = _edgeScrollMargin;

            Vector3 scrollDir = Vector3.zero;
            if (screenPos.x < sw * margin) scrollDir.x = -1f;
            else if (screenPos.x > sw * (1f - margin)) scrollDir.x = 1f;
            if (screenPos.y < sh * margin) scrollDir.y = -1f;
            else if (screenPos.y > sh * (1f - margin)) scrollDir.y = 1f;

            if (scrollDir != Vector3.zero)
            {
                _cam.transform.position += scrollDir * _edgeScrollSpeed * Time.deltaTime;
            }
        }

        // ======================== BUTTONS ========================

        private void CreateButtons()
        {
            _readyBtnObj = new GameObject("MoveReadyBtn");
            _readySr = _readyBtnObj.AddComponent<SpriteRenderer>();
            _readySr.sortingLayerName = "Bubbles";
            _readySr.sortingOrder = 100;
            if (_readyFrames != null && _readyFrames.Length > 0)
                _readySr.sprite = _readyFrames[0];
            SetSpriteWorldSize(_readyBtnObj, _readySr, ReadySize);

            _cancelBtnObj = new GameObject("MoveCancelBtn");
            _cancelSr = _cancelBtnObj.AddComponent<SpriteRenderer>();
            _cancelSr.sortingLayerName = "Bubbles";
            _cancelSr.sortingOrder = 100;
            if (_cancelFrames != null && _cancelFrames.Length > 0)
                _cancelSr.sprite = _cancelFrames[0];
            SetSpriteWorldSize(_cancelBtnObj, _cancelSr, CancelSize);

            _animFrame = 0;
            _animTimer = 0f;
            UpdateButtonPositions();
        }

        private void UpdateButtonPositions()
        {
            if (_movingObject == null) return;
            var data = _movingObject.Data;
            float right = _currentGridPos.x + data.size.x + 0.1f;
            float top = _currentGridPos.y + data.size.y;
            float bottom = _currentGridPos.y;

            if (_readyBtnObj != null)
                _readyBtnObj.transform.position = new Vector3(right + ReadySize * 0.5f, top - ReadySize * 0.5f + 0.1f, 0f);
            if (_cancelBtnObj != null)
                _cancelBtnObj.transform.position = new Vector3(right + CancelSize * 0.5f, bottom + CancelSize * 0.5f - 0.1f, 0f);
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

        private int GetTopWallY()
        {
            var gridData = _gridRenderer.Data;
            if (gridData == null) return 29;
            for (int y = GridData.Height - 1; y >= 0; y--)
                for (int x = 0; x < GridData.Width; x++)
                    if (gridData.GetCell(x, y) == CellType.Wall &&
                        y > 0 && gridData.IsWalkable(x, y - 1))
                        return y;
            return 29;
        }

        private static void SetSpriteWorldSize(GameObject go, SpriteRenderer sr, float worldSize)
        {
            if (sr.sprite == null) return;
            float spriteSize = Mathf.Max(sr.sprite.bounds.size.x, sr.sprite.bounds.size.y);
            if (spriteSize <= 0f) return;
            float scale = worldSize / spriteSize;
            go.transform.localScale = new Vector3(scale, scale, 1f);
        }

        private static Transform FindInChildren(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name) return child;
                var found = FindInChildren(child, name);
                if (found != null) return found;
            }
            return null;
        }

        private static GameObject FindInactiveByName(string name)
        {
            var go = GameObject.Find(name);
            if (go != null) return go;
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                foreach (var root in scene.GetRootGameObjects())
                {
                    var found = FindInChildren(root.transform, name);
                    if (found != null) return found.gameObject;
                }
            }
            return null;
        }
    }
}
