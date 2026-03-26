using System.Collections.Generic;
using UnityEngine;
using CatHotel.Core;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Placed object in the hotel (bed, food bowl, litter, toy, decoration...).
    /// Attached to the GameObject in the scene. Registered in ObjectRegistry.
    /// </summary>
    public class HotelObject : MonoBehaviour
    {
        [SerializeField] private HotelObjectData _data;
        [SerializeField] private Vector2Int _gridPos;
        private readonly HashSet<int> _currentUsers = new(); // instanceIDs of cats using this

        public HotelObjectData Data => _data;
        public Vector2Int GridPos => _gridPos;
        public bool IsFull => _currentUsers.Count >= _data.maxUsers;
        public int UserCount => _currentUsers.Count;
        public bool IsSelected { get; private set; }

        public void Init(HotelObjectData data, Vector2Int gridPos)
        {
            _data = data;
            _gridPos = gridPos;
        }

        public bool TryReserve(int catInstanceId)
        {
            if (IsFull) return false;
            return _currentUsers.Add(catInstanceId);
        }

        public void Release(int catInstanceId)
        {
            _currentUsers.Remove(catInstanceId);
        }

        /// <summary>Satisfaction per second = (100 / useDuration) × efficiency.</summary>
        public float SatisfactionRate => (100f / _data.useDuration) * _data.efficiency;

        /// <summary>Switch to selected sprite (preserving visual size).</summary>
        public void Select()
        {
            if (IsSelected || _data.selectedSprite == null) return;
            IsSelected = true;
            SwapSprite(_data.selectedSprite);
        }

        /// <summary>Switch back to normal sprite.</summary>
        public void Deselect()
        {
            if (!IsSelected) return;
            IsSelected = false;
            var normalSprite = _data.worldSprite != null ? _data.worldSprite : _data.icon;
            SwapSprite(normalSprite);
        }

        private void SwapSprite(Sprite newSprite)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr == null || newSprite == null) return;
            sr.sprite = newSprite;

            // Recalculate scale so the object keeps the same world footprint
            float spriteW = newSprite.bounds.size.x;
            float spriteH = newSprite.bounds.size.y;
            if (spriteW > 0f && spriteH > 0f)
            {
                float targetW = _data.size.x;
                float targetH = _data.size.y;
                float scale = Mathf.Min(targetW / spriteW, targetH / spriteH) * _data.visualScale;
                transform.localScale = new Vector3(scale, scale, 1f);
            }
        }

        private void OnEnable()
        {
            ObjectRegistry.Register(this);
        }

        private void OnDisable()
        {
            ObjectRegistry.Unregister(this);
        }
    }
}
