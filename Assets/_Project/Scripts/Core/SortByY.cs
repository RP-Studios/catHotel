using UnityEngine;

namespace CatHotel.Core
{
    /// <summary>
    /// Dynamically sets SpriteRenderer.sortingOrder based on Y position.
    /// Lower Y (closer to camera) = higher sortingOrder.
    /// Uses the bottom edge of the sprite for correct overlap.
    /// </summary>
    public class SortByY : MonoBehaviour
    {
        private SpriteRenderer _sr;
        private float _bottomOffset;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null && _sr.sprite != null)
            {
                // Use the bottom of the sprite bounds for sorting
                _bottomOffset = _sr.sprite.bounds.min.y * transform.localScale.y;
            }
        }

        private void LateUpdate()
        {
            if (_sr == null) return;
            float bottomY = transform.position.y + _bottomOffset;
            // Same scale as CatEntity: *100 + 10000
            _sr.sortingOrder = Mathf.RoundToInt(-bottomY * 100f) + 10000;
        }
    }
}
