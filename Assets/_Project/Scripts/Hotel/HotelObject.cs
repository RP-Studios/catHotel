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
