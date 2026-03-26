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
        /// <summary>Extra bias added to sortingOrder (e.g. +1 for on-table objects).</summary>
        public int OrderBias;

        /// <summary>Override Y used for sorting (e.g. use table's Y for on-table objects).</summary>
        public float? SortYOverride;

        private SpriteRenderer _sr;
        private float _bottomOffset;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null && _sr.sprite != null)
            {
                _bottomOffset = _sr.sprite.bounds.min.y * transform.localScale.y;
            }
        }

        private void LateUpdate()
        {
            if (_sr == null) return;
            float sortY = SortYOverride ?? (transform.position.y + _bottomOffset);
            _sr.sortingOrder = Mathf.RoundToInt(-sortY * 100f) + 10000 + OrderBias;
        }
    }
}
