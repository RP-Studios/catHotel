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

            // Don't interfere with placement mode
            if (_objectPlacement != null && _objectPlacement.IsPlacing) return;

            // Don't select when tapping UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            Vector2 screenPos = pointer.position.ReadValue();
            Vector3 worldPos = _cam.ScreenToWorldPoint(screenPos);

            // Cat has priority — check if a cat is near the tap
            if (IsCatUnderPointer(worldPos))
            {
                DeselectCurrent();
                return;
            }

            // Find nearest object under tap
            HotelObject tapped = FindObjectAtWorld(worldPos);

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
            // Same radius as CatSpawner.TryPetAtPointer
            const float catTapRadius = 1.5f;
            foreach (var cat in _catSpawner.AllCats)
            {
                if (Vector2.Distance(worldPos, cat.transform.position) < catTapRadius)
                    return true;
            }
            return false;
        }

        private static HotelObject FindObjectAtWorld(Vector3 worldPos)
        {
            HotelObject best = null;
            float bestDist = float.MaxValue;

            foreach (var obj in ObjectRegistry.Objects)
            {
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
