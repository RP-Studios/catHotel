using UnityEngine;

namespace CatHotel.Core
{
    [CreateAssetMenu(fileName = "NewObject", menuName = "Cat Hotel/Hotel Object")]
    public class HotelObjectData : ScriptableObject
    {
        [Header("Identity")]
        public string displayName;
        public Sprite icon;
        public Sprite worldSprite;

        [Header("Category")]
        public ObjectCategory category;

        [Header("Economy")]
        public int cost;
        [Tooltip("Sell price = cost * sellRatio")]
        public float sellRatio = 0.5f;

        [Header("Gameplay")]
        [Tooltip("How fast this object satisfies the need (1.0 = base)")]
        public float efficiency = 1f;

        [Tooltip("Comfort bonus (decorations only)")]
        public int comfortBonus;

        [Header("Placement")]
        public Vector2Int size = Vector2Int.one;
        [Tooltip("Can be placed on walls (shelves, paintings)")]
        public bool wallMount;

        [Header("Carpet bonus (carpet only)")]
        [Tooltip("Which need gets the zone bonus")]
        public NeedType carpetBonusNeed;
        [Tooltip("Zone bonus percentage (e.g. 0.2 = +20%)")]
        public float carpetBonusPercent;

        [Header("Usage")]
        [Tooltip("How many cats can use this object at once")]
        public int maxUsers = 1;
        [Tooltip("Time in seconds to fully satisfy need from 0%")]
        public float useDuration = 5f;

        public int SellPrice => Mathf.RoundToInt(cost * sellRatio);

        /// <summary>Maps object category to the need it satisfies. Returns null for non-need categories.</summary>
        public NeedType? SatisfiedNeed => category switch
        {
            ObjectCategory.Food => NeedType.Hunger,
            ObjectCategory.Sleep => NeedType.Sleep,
            ObjectCategory.Play => NeedType.Play,
            ObjectCategory.Clean => NeedType.Clean,
            _ => null
        };
    }
}
