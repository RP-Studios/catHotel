using System.Collections.Generic;
using UnityEngine;
using CatHotel.Core;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Global registry of all placed HotelObjects, bucketed by floor.
    /// Queries take a floorIndex (or a set of floors) so cat AI can scope
    /// its searches to the floors it knows about.
    /// </summary>
    public static class ObjectRegistry
    {
        private static readonly Dictionary<int, List<HotelObject>> _byFloor = new();
        private static readonly List<HotelObject> _all = new();

        public static IReadOnlyList<HotelObject> Objects => _all;

        public static IReadOnlyList<HotelObject> GetFloor(int floorIndex)
        {
            if (_byFloor.TryGetValue(floorIndex, out var list)) return list;
            return System.Array.Empty<HotelObject>();
        }

        public static void Register(HotelObject obj)
        {
            if (obj == null) return;
            if (!_all.Contains(obj)) _all.Add(obj);

            if (!_byFloor.TryGetValue(obj.FloorIndex, out var list))
            {
                list = new List<HotelObject>();
                _byFloor[obj.FloorIndex] = list;
            }
            if (!list.Contains(obj)) list.Add(obj);
        }

        public static void Unregister(HotelObject obj)
        {
            if (obj == null) return;
            _all.Remove(obj);
            foreach (var list in _byFloor.Values)
                list.Remove(obj);
        }

        /// <summary>Find the nearest available object that satisfies the given need,
        /// searching only the specified floor.</summary>
        public static HotelObject FindNearest(NeedType need, Vector2Int fromPos, int floorIndex)
        {
            HotelObject best = null;
            float bestDist = float.MaxValue;
            var targetCategory = NeedToCategory(need);

            foreach (var obj in GetFloor(floorIndex))
            {
                if (obj.Data.category != targetCategory) continue;
                if (obj.IsFull) continue;
                float dist = Vector2Int.Distance(fromPos, obj.GridPos);
                if (dist < bestDist) { bestDist = dist; best = obj; }
            }
            return best;
        }

        /// <summary>Find the nearest available object across multiple floors (the floors
        /// the cat has visited). Returns the object and its floor.</summary>
        public static bool TryFindNearestAcross(NeedType need, Vector2Int fromPos,
            IEnumerable<int> floors, out HotelObject best, out int bestFloor)
        {
            best = null;
            bestFloor = -1;
            float bestDist = float.MaxValue;
            var targetCategory = NeedToCategory(need);

            foreach (var floor in floors)
            {
                foreach (var obj in GetFloor(floor))
                {
                    if (obj.Data.category != targetCategory) continue;
                    if (obj.IsFull) continue;
                    float dist = Vector2Int.Distance(fromPos, obj.GridPos);
                    // Heavy penalty for objects on another floor so local options win ties
                    float score = dist + 1000f * System.Math.Abs(floor - bestFloor);
                    if (best == null || dist < bestDist)
                    {
                        bestDist = dist;
                        best = obj;
                        bestFloor = floor;
                    }
                }
            }
            return best != null;
        }

        /// <summary>Total comfort bonus from decorations on a given floor.</summary>
        public static float CalculateComfort(int totalCats, int roomCapacity, int floorIndex)
        {
            float comfort = 50f; // neutral baseline

            foreach (var obj in GetFloor(floorIndex))
            {
                if (obj.Data.category == ObjectCategory.Decoration)
                    comfort += obj.Data.comfortBonus;
            }

            if (roomCapacity > 0)
            {
                float occupancy = (float)totalCats / roomCapacity;
                if (occupancy > 0.8f)
                    comfort -= 20f * ((occupancy - 0.8f) / 0.2f);
                else if (occupancy < 0.2f)
                    comfort += 5f;
            }

            return Mathf.Clamp(comfort, 0f, 100f);
        }

        /// <summary>Check if a rectangular area is free of placed objects on a given floor.</summary>
        public static bool IsAreaFree(RectInt area, int floorIndex)
        {
            foreach (var obj in GetFloor(floorIndex))
            {
                var objRect = new RectInt(obj.GridPos, obj.Data.size);
                if (area.Overlaps(objRect))
                    return false;
            }
            return true;
        }

        /// <summary>Check if a table-category object covers the given area on a given floor.</summary>
        public static bool HasTableAt(RectInt area, int floorIndex)
        {
            foreach (var obj in GetFloor(floorIndex))
            {
                if (!obj.Data.isTable) continue;
                var objRect = new RectInt(obj.GridPos, obj.Data.size);
                if (area.Overlaps(objRect)) return true;
            }
            return false;
        }

        public static void Clear()
        {
            _all.Clear();
            _byFloor.Clear();
        }

        private static ObjectCategory NeedToCategory(NeedType need)
        {
            return need switch
            {
                NeedType.Hunger => ObjectCategory.Food,
                NeedType.Thirst => ObjectCategory.Water,
                NeedType.Sleep => ObjectCategory.Sleep,
                NeedType.Play => ObjectCategory.Play,
                NeedType.Clean => ObjectCategory.Clean,
                _ => ObjectCategory.Food
            };
        }
    }
}
