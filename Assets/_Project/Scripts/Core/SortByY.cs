using UnityEngine;

namespace CatHotel.Core
{
    /// <summary>
    /// Sets SpriteRenderer.sortingOrder based on Y position within its sorting layer.
    /// Lower Y = higher sortingOrder = rendered in front.
    /// X position is used as tie-breaker to prevent z-fighting.
    ///
    /// Sorting layers (back to front):
    ///   Default  → tilemap decor (floor, walls)
    ///   Carpets  → carpet objects
    ///   Objects  → furniture, bowls, beds, etc.
    ///   Cats     → all cats
    ///   Bubbles  → mood/need bubbles, coins, fight clouds
    /// </summary>
    public class SortByY : MonoBehaviour
    {
        private SpriteRenderer _sr;

        private void LateUpdate()
        {
            if (_sr == null)
            {
                _sr = GetComponent<SpriteRenderer>();
                if (_sr == null) return;
            }

            // Lower Y = higher order = rendered in front
            int yOrder = Mathf.RoundToInt(-transform.position.y * 100f);

            // X tie-breaker to prevent z-fighting at same Y
            int xTieBreaker = Mathf.RoundToInt(transform.position.x);

            _sr.sortingOrder = yOrder + xTieBreaker;
        }
    }
}
