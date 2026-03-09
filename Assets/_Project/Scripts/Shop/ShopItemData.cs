using UnityEngine;

namespace CatHotel.Shop
{
    public enum ShopCategory
    {
        Beds,
        Food,
        Water,
        Toys,
        Litter,
        Carpets,
        Env
    }

    [CreateAssetMenu(fileName = "NewShopItem", menuName = "Cat Hotel/Shop Item")]
    public class ShopItemData : ScriptableObject
    {
        public string displayName;
        public Sprite icon;
        public int price;
        public ShopCategory category;

        [Header("Placement")]
        public string spritePath; // asset path used by ProtoSceneSetup to resolve the world sprite
    }
}
