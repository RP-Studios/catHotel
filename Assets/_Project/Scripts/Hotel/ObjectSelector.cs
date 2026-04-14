using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using CatHotel.Cats;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Handles tap-to-select on placed HotelObjects.
    /// Cats have priority: if a cat is under the pointer, no object is selected.
    /// Tapping empty space or another object deselects the current selection.
    /// </summary>
    public class ObjectSelector : MonoBehaviour
    {
        [SerializeField] private CatSpawner _catSpawner;
        [SerializeField] private ObjectPlacement _objectPlacement;
        [SerializeField] private ObjectMover _objectMover;
        [SerializeField] private CatHotel.Grid.GridRenderer _gridRenderer;

        private Camera _cam;
        private HotelObject _selected;

        public HotelObject Selected => _selected;

        private void Start()
        {
            _cam = Camera.main;
        }

        private void Update()
        {
            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;

            // Don't interfere with placement or move mode
            if (_objectPlacement != null && _objectPlacement.IsPlacing) return;
            if (_objectMover != null && _objectMover.IsMoving) return;

            // Don't select when tapping UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            Vector2 screenPos = pointer.position.ReadValue();
            if (float.IsNaN(screenPos.x) || float.IsNaN(screenPos.y)) return;
            Vector3 worldPos = _cam.ScreenToWorldPoint(screenPos);

            // Cat has priority — check if a cat is near the tap
            if (IsCatUnderPointer(worldPos))
            {
                DeselectCurrent();
                return;
            }

            // Find nearest object under tap (current floor only)
            int floor = _gridRenderer != null ? _gridRenderer.CurrentFloor : 0;
            HotelObject tapped = FindObjectAtWorld(worldPos, floor);

            if (tapped == null)
            {
                // Tapped empty space → deselect
                DeselectCurrent();
                return;
            }

            if (tapped == _selected)
            {
                // Tapped same object → deselect (toggle)
                DeselectCurrent();
                return;
            }

            // Select new object
            DeselectCurrent();
            _selected = tapped;
            _selected.Select();
        }

        public void DeselectCurrent()
        {
            if (_selected != null)
            {
                _selected.Deselect();
                _selected = null;
            }
        }

        private bool IsCatUnderPointer(Vector3 worldPos)
        {
            if (_catSpawner == null) return false;
            int visible = _gridRenderer != null ? _gridRenderer.CurrentFloor : 0;
            const float catTapRadius = 1.5f;
            foreach (var cat in _catSpawner.AllCats)
            {
                if (cat.FloorIndex != visible) continue;
                if (Vector2.Distance(worldPos, cat.transform.position) < catTapRadius)
                    return true;
            }
            return false;
        }

        private static HotelObject FindObjectAtWorld(Vector3 worldPos, int floor)
        {
            HotelObject best = null;
            float bestDist = float.MaxValue;

            foreach (var obj in ObjectRegistry.GetFloor(floor))
            {
                if (obj.Data.isStairs) continue;
                // Check if tap is within the object's grid footprint
                var rect = new RectInt(obj.GridPos, obj.Data.size);
                int px = Mathf.FloorToInt(worldPos.x);
                int py = Mathf.FloorToInt(worldPos.y);

                if (!rect.Contains(new Vector2Int(px, py))) continue;

                // Among overlapping objects, pick the nearest center
                float dist = Vector2.Distance(worldPos, obj.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = obj;
                }
            }

            return best;
        }
    }
}
