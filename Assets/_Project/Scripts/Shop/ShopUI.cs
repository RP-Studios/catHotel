using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CatHotel.Shop
{
    public class ShopUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform _panel;
        [SerializeField] private RectTransform _content; // ScrollRect content
        [SerializeField] private Button _closeBtn;
        [SerializeField] private GameObject _itemPrefab;

        [Header("Data")]
        [SerializeField] private ShopItemData[] _items;

        [Header("Colors")]
        [SerializeField] private Color _cardColor = new Color(0.96f, 0.90f, 0.78f, 1f); // Crème #F5E6C8
        [SerializeField] private Color _priceColor = new Color(0.78f, 0.47f, 0.47f, 1f); // Rose #C87878

        private readonly List<ShopItemUI> _cards = new();
        private bool _isOpen;

        public bool IsOpen => _isOpen;

        public event System.Action<ShopItemData> OnItemSelected;

        private void Awake()
        {
            if (_closeBtn != null)
                _closeBtn.onClick.AddListener(Close);

            if (_panel != null)
                _panel.gameObject.SetActive(false);
        }

        public void Open()
        {
            if (_panel == null) return;
            _panel.gameObject.SetActive(true);
            _isOpen = true;

            if (_cards.Count == 0)
                PopulateItems();
        }

        public void Close()
        {
            if (_panel == null) return;
            _panel.gameObject.SetActive(false);
            _isOpen = false;
        }

        public void Toggle()
        {
            if (_isOpen) Close();
            else Open();
        }

        private void PopulateItems()
        {
            if (_items == null || _content == null || _itemPrefab == null) return;

            foreach (var item in _items)
            {
                var go = Instantiate(_itemPrefab, _content);
                var card = go.GetComponent<ShopItemUI>();
                if (card == null) continue;

                card.Setup(item, _cardColor, _priceColor);
                card.OnClicked += HandleItemClicked;
                _cards.Add(card);
            }
        }

        private void HandleItemClicked(ShopItemData item)
        {
            OnItemSelected?.Invoke(item);
#if UNITY_EDITOR
            Debug.Log($"[ShopUI] Selected: {item.displayName} ({item.price} coins)");
#endif
        }

        private void OnDestroy()
        {
            foreach (var card in _cards)
                if (card != null)
                    card.OnClicked -= HandleItemClicked;
        }
    }
}
