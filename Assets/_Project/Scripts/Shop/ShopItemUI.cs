using UnityEngine;
using UnityEngine.UI;

namespace CatHotel.Shop
{
    public class ShopItemUI : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Text _nameLabel;
        [SerializeField] private Text _priceLabel;
        [SerializeField] private Image _priceTag;
        [SerializeField] private Button _button;

        private ShopItemData _data;

        public event System.Action<ShopItemData> OnClicked;

        public void Setup(ShopItemData data, Color cardColor, Color priceColor)
        {
            _data = data;

            if (_iconImage != null && data.icon != null)
            {
                _iconImage.sprite = data.icon;
                _iconImage.preserveAspect = true;
            }

            if (_nameLabel != null)
                _nameLabel.text = data.displayName;

            if (_priceLabel != null)
                _priceLabel.text = $"{data.price}";

            if (_background != null)
                _background.color = cardColor;

            if (_priceTag != null)
                _priceTag.color = priceColor;

            if (_button != null)
                _button.onClick.AddListener(() => OnClicked?.Invoke(_data));
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveAllListeners();
        }
    }
}
