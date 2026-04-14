using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using CatHotel.Audio;
using CatHotel.Grid;
using CatHotel.Hotel;

namespace CatHotel.Cats
{
    public class BedSpot
    {
        public Vector2Int GridPos;
        public bool Occupied;

        public BedSpot(Vector2Int gridPos) { GridPos = gridPos; }
    }

    /// <summary>
    /// Lightweight service for cat interactions (petting, combat lookup).
    /// Spawning is now handled by HotelManager.
    /// </summary>
    public class CatSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridRenderer _gridRenderer;

        [Header("Combat")]
        [SerializeField] private RuntimeAnimatorController _fightCloudController;

        [Header("Petting")]
        [SerializeField] private RuntimeAnimatorController _handPetController;

        private readonly List<CatEntity> _cats = new();
        private bool _pettingMode;
        private Camera _mainCam;

        public RuntimeAnimatorController FightCloudController => _fightCloudController;
        public RuntimeAnimatorController HandPetController => _handPetController;
        public IReadOnlyList<CatEntity> AllCats => _cats;
        public bool PettingMode => _pettingMode;

        /// <summary>Called by HotelManager when a new cat is created.</summary>
        public void RegisterCat(CatEntity cat)
        {
            if (!_cats.Contains(cat))
                _cats.Add(cat);
        }

        /// <summary>Called by HotelManager when a cat leaves.</summary>
        public void UnregisterCat(CatEntity cat)
        {
            _cats.Remove(cat);
        }

        public void TogglePetting()
        {
            _pettingMode = !_pettingMode;
        }

        private void Update()
        {
            if (_pettingMode && Pointer.current != null
                && Pointer.current.press.wasPressedThisFrame)
            {
                TryPetAtPointer();
            }
        }

        private void TryPetAtPointer()
        {
            if (_mainCam == null) _mainCam = Camera.main;
            if (_mainCam == null) return;

            // Don't pet when tapping UI elements
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            Vector2 screenPos = Pointer.current.position.ReadValue();
            Vector3 worldPos = _mainCam.ScreenToWorldPoint(screenPos);

            int visibleFloor = _gridRenderer != null ? _gridRenderer.CurrentFloor : 0;
            CatEntity nearest = null;
            float bestDist = 1.5f;
            foreach (var cat in _cats)
            {
                if (cat.FloorIndex != visibleFloor) continue;
                float dist = Vector2.Distance(worldPos, cat.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest = cat;
                }
            }

            if (nearest != null)
            {
                nearest.PlayPetting(_handPetController);
                CatSoundManager.Instance?.PlayMeowForCat(nearest);

                // Also trigger happiness boost
                var happiness = nearest.GetComponent<CatHappiness>();
                happiness?.ApplyPetBonus();
            }
        }

        public CatEntity GetCatAt(Vector2Int pos, CatEntity exclude = null)
        {
            foreach (var cat in _cats)
                if (cat != exclude && cat.GridPos == pos)
                    return cat;
            return null;
        }

        // Legacy bed system — will migrate to ObjectRegistry/HotelObject
        private readonly List<BedSpot> _bedSpots = new();

        public BedSpot ClaimNearestBed(Vector2Int from)
        {
            BedSpot best = null;
            float bestDist = float.MaxValue;
            foreach (var bed in _bedSpots)
            {
                if (bed.Occupied) continue;
                float dist = Vector2Int.Distance(from, bed.GridPos);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = bed;
                }
            }
            if (best != null) best.Occupied = true;
            return best;
        }

        public void ReleaseBed(BedSpot bed)
        {
            if (bed != null) bed.Occupied = false;
        }
    }
}
