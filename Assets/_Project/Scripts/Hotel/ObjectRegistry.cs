using System.Collections.Generic;
using UnityEngine;
using CatHotel.Core;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Global registry of all placed HotelObjects.
    /// Provides queries for cats to find objects that satisfy their needs.
    /// </summary>
    public static class ObjectRegistry
    {
        private static readonly List<HotelObject> All = new();

        public static IReadOnlyList<HotelObject> Objects => All;

        public static void Register(HotelObject obj)
        {
            if (!All.Contains(obj))
                All.Add(obj);
        }

        public static void Unregister(HotelObject obj)
        {
            All.Remove(obj);
        }

        /// <summary>
        /// Find the nearest available object that satisfies the given need.
        /// Returns null if none found.
        /// </summary>
        public static HotelObject FindNearest(NeedType need, Vector2Int fromPos)
        {
            HotelObject best = null;
            float bestDist = float.MaxValue;

            var targetCategory = NeedToCategory(need);

            foreach (var obj in All)
            {
                if (obj.Data.category != targetCategory) continue;
                if (obj.IsFull) continue;

                float dist = Vector2Int.Distance(fromPos, obj.GridPos);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = obj;
                }
            }

            return best;
        }

        /// <summary>Calculate total comfort bonus from all decorations.</summary>
        public static float CalculateComfort(int totalCats, int roomCapacity)
        {
            float comfort = 50f; // neutral baseline

            foreach (var obj in All)
            {
                if (obj.Data.category == ObjectCategory.Decoration)
                    comfort += obj.Data.comfortBonus;
            }

            // Overcrowding penalty
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

        public static void Clear() => All.Clear();

        private static ObjectCategory NeedToCategory(NeedType need)
        {
            return need switch
            {
                NeedType.Hunger => ObjectCategory.Food,
                NeedType.Sleep => ObjectCategory.Sleep,
                NeedType.Play => ObjectCategory.Play,
                NeedType.Clean => ObjectCategory.Clean,
                _ => ObjectCategory.Food
            };
        }
    }
}
