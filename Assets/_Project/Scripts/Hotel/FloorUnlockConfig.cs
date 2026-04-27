using System;
using UnityEngine;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Defines the requirements and effects of unlocking each floor.
    /// Floor index 0 (RDC) is always unlocked at game start.
    /// </summary>
    [CreateAssetMenu(fileName = "FloorUnlockConfig", menuName = "Cat Hotel/Floor Unlock Config")]
    public class FloorUnlockConfig : ScriptableObject
    {
        [Serializable]
        public class FloorEntry
        {
            [Tooltip("Floor index (0 = RDC, 1 = first upper floor, ...).")]
            public int floorIndex;

            [Tooltip("Minimum reputation level required to unlock this floor (0-9).")]
            public int requiredReputationLevel;

            [Tooltip("Cat coins cost to unlock this floor.")]
            public int catCoinCost;

            [Tooltip("Total simultaneous cats allowed once this floor is unlocked.")]
            public int totalCapacity = 5;
        }

        [Tooltip("Ordered by floorIndex. Index 0 (RDC) is included for capacity, but cost/rep are ignored.")]
        public FloorEntry[] floors;

        public int FloorCount => floors != null ? floors.Length : 0;

        public FloorEntry GetEntry(int floorIndex)
        {
            if (floors == null) return null;
            for (int i = 0; i < floors.Length; i++)
                if (floors[i].floorIndex == floorIndex) return floors[i];
            return null;
        }
    }
}
