using UnityEngine;

namespace CatHotel.Core
{
    /// <summary>
    /// Copies the sortingOrder of a target SpriteRenderer + an offset.
    /// Used for objects placed on top of other objects (e.g. aquarium on table).
    /// </summary>
    public class SortOrderFollower : MonoBehaviour
    {
        private SpriteRenderer _target;
        private SpriteRenderer _sr;
        private int _offset;

        public void Init(SpriteRenderer target, int offset)
        {
            _target = target;
            _offset = offset;
        }

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            if (_sr == null || _target == null) return;
            _sr.sortingOrder = _target.sortingOrder + _offset;
        }
    }
}
