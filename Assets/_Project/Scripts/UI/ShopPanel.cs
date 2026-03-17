using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CatHotel.Core;
using CatHotel.Hotel;
using CatHotel.Economy;

namespace CatHotel.UI
{
    /// <summary>
    /// Shop categories map to art folders, not to ObjectCategory.
    /// Multiple shop categories can map to the same ObjectCategory.
    /// </summary>
    public enum ShopCategory
    {
        Beds,        // art/objects/beds        → Sleep
        Pillows,     // art/objects/pillows     → Sleep
        Croquettes,  // art/objects/food         → Food (Hunger)
        Water,       // art/objects/water        → Food (Hunger)
        Balls,       // art/objects/toys          → Play
        Scratchers,  // art/objects/scratchers    → Play
        Litters      // art/objects/litters       → Clean
    }

    /// <summary>
    /// Main shop panel. Shows object categories, tapping a category opens
    /// the Category sub-view with MainShopItem prefabs populated from ScriptableObjects.
    /// </summary>
    public class ShopPanel : MonoBehaviour
    {
        private RectTransform _panel;
        private GameObject _panelObj;
        private float _panelWidth;
        private Tween _slideTween;
        private bool _isOpen;

        // Close button
        private RectTransform _closeRect;

        // Category action rects
        private RectTransform _bedsRect;
        private RectTransform _pillowsRect;
        private RectTransform _croquettesRect;
        private RectTransform _waterRect;
        private RectTransform _ballsRect;
        private RectTransform _scratchersRect;
        private RectTransform _littersRect;

        // Categories list (inside ShopPanel)
        private GameObject _categoriesObj;

        // Category detail view (inside ShopPanel)
        private GameObject _categoryObj;
        private RectTransform _backToCategoriesRect;
        private Transform _itemsListParent; // MainShopItemsList
        private TMP_Text _categoryNameText;
        private bool _isCategoryOpen;

        // Wired via ProtoSceneSetup
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private HotelObjectData[] _allShopObjects;
        [SerializeField] private EconomyManager _economy;
        [SerializeField] private ObjectPlacement _placement;

        // All shop items grouped by ShopCategory
        private readonly Dictionary<ShopCategory, List<HotelObjectData>> _shopItems = new();

        public bool IsOpen => _isOpen;

        private void Start()
        {
            // Load all HotelObjectData assets and group by shop category
            LoadShopItems();

            // --- Main shop panel ---
            _panelObj = FindInactiveByName("ShopPanel");
            if (_panelObj == null) return;

            _panel = _panelObj.GetComponent<RectTransform>();

            _panelObj.SetActive(true);
            EnsureRaycastBackground(_panelObj);
            Canvas.ForceUpdateCanvases();
            _panelWidth = _panel.rect.width;
            if (_panelWidth <= 0f) _panelWidth = 600f;

            var pos = _panel.anchoredPosition;
            pos.x = _panelWidth;
            _panel.anchoredPosition = pos;
            _panelObj.SetActive(false);

            // Find action rects and add juice
            _closeRect = FindRect(_panelObj, "CloseImage");
            _bedsRect = FindRect(_panelObj, "BedsAction");
            _pillowsRect = FindRect(_panelObj, "PillowsAction");
            _croquettesRect = FindRect(_panelObj, "CroquettesAction");
            _waterRect = FindRect(_panelObj, "WaterAction");
            _ballsRect = FindRect(_panelObj, "BallsAction");
            _scratchersRect = FindRect(_panelObj, "ScratchersAction");
            _littersRect = FindRect(_panelObj, "LittersAction");

            AddJuice(_closeRect);
            AddJuice(_bedsRect);
            AddJuice(_pillowsRect);
            AddJuice(_croquettesRect);
            AddJuice(_waterRect);
            AddJuice(_ballsRect);
            AddJuice(_scratchersRect);
            AddJuice(_littersRect);

            // --- Categories list + Category detail (children of ShopPanel) ---
            var categoriesT = FindInChildren(_panelObj.transform, "Categories");
            if (categoriesT != null)
                _categoriesObj = categoriesT.gameObject;

            var categoryT = FindInChildren(_panelObj.transform, "Category");
            if (categoryT != null)
            {
                _categoryObj = categoryT.gameObject;
                _categoryObj.SetActive(false); // hidden by default

                _backToCategoriesRect = FindRect(_categoryObj, "BackToCategoriesAction");
                AddJuice(_backToCategoriesRect);

                var nameT = FindInChildren(_categoryObj.transform, "CategoryName");
                if (nameT != null)
                    _categoryNameText = nameT.GetComponent<TMP_Text>();

                var listTransform = FindInChildren(_categoryObj.transform, "MainShopItemsList");
                if (listTransform != null)
                    _itemsListParent = listTransform;
            }

            // Wire ShopAction button
            var shopActionObj = GameObject.Find("ShopAction");
            if (shopActionObj != null)
            {
                var btn = shopActionObj.GetComponent<Button>();
                if (btn == null) btn = shopActionObj.AddComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(Toggle);

                var graphic = shopActionObj.GetComponent<Graphic>();
                if (graphic != null) graphic.raycastTarget = true;

                AddJuice(shopActionObj.GetComponent<RectTransform>());
            }
        }

        /// <summary>
        /// Loads all HotelObjectData from Assets/_Project/Data/Objects/
        /// and groups them by ShopCategory based on their asset name prefix.
        /// </summary>
        private void LoadShopItems()
        {
            _shopItems.Clear();
            foreach (ShopCategory cat in System.Enum.GetValues(typeof(ShopCategory)))
                _shopItems[cat] = new List<HotelObjectData>();

            if (_allShopObjects == null) return;

            foreach (var obj in _allShopObjects)
            {
                if (obj == null || obj.icon == null) continue;

                var cat = ClassifyObject(obj);
                if (cat.HasValue)
                    _shopItems[cat.Value].Add(obj);
            }

            foreach (var list in _shopItems.Values)
                list.Sort((a, b) => a.cost.CompareTo(b.cost));
        }

        /// <summary>
        /// Classify a HotelObjectData into a ShopCategory based on its asset name.
        /// </summary>
        private static ShopCategory? ClassifyObject(HotelObjectData obj)
        {
            string name = obj.name.ToLowerInvariant();

            if (name.Contains("bed") || name.Contains("luxury")) return ShopCategory.Beds;
            if (name.Contains("coussin") || name.Contains("pillow")) return ShopCategory.Pillows;
            if (name.Contains("food") || name.Contains("croquette")) return ShopCategory.Croquettes;
            if (name.Contains("water")) return ShopCategory.Water;
            if (name.Contains("woolball") || name.Contains("ball")) return ShopCategory.Balls;
            if (name.Contains("scratcher") || name.Contains("griffoir")) return ShopCategory.Scratchers;
            if (name.Contains("litter") || name.Contains("litiere")) return ShopCategory.Litters;

            // Fallback by ObjectCategory
            return obj.category switch
            {
                ObjectCategory.Sleep => ShopCategory.Beds,
                ObjectCategory.Food => ShopCategory.Croquettes,
                ObjectCategory.Play => ShopCategory.Balls,
                ObjectCategory.Clean => ShopCategory.Litters,
                _ => null
            };
        }

        private void Update()
        {
            if (!_isOpen) return;

            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;

            Vector2 screenPos = pointer.position.ReadValue();

            // Close (works from both views)
            if (_closeRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_closeRect, screenPos, null))
            {
                Close();
                return;
            }

            // If category is open, handle category-level interactions
            if (_isCategoryOpen)
            {
                if (_backToCategoriesRect != null &&
                    RectTransformUtility.RectangleContainsScreenPoint(_backToCategoriesRect, screenPos, null))
                {
                    CloseCategory();
                    return;
                }
                return;
            }

            // Category actions
            if (CheckCategoryTap(_bedsRect, ShopCategory.Beds, screenPos)) return;
            if (CheckCategoryTap(_pillowsRect, ShopCategory.Pillows, screenPos)) return;
            if (CheckCategoryTap(_croquettesRect, ShopCategory.Croquettes, screenPos)) return;
            if (CheckCategoryTap(_waterRect, ShopCategory.Water, screenPos)) return;
            if (CheckCategoryTap(_ballsRect, ShopCategory.Balls, screenPos)) return;
            if (CheckCategoryTap(_scratchersRect, ShopCategory.Scratchers, screenPos)) return;
            if (CheckCategoryTap(_littersRect, ShopCategory.Litters, screenPos)) return;
        }

        private bool CheckCategoryTap(RectTransform rect, ShopCategory category, Vector2 screenPos)
        {
            if (rect == null) return false;
            if (!RectTransformUtility.RectangleContainsScreenPoint(rect, screenPos, null)) return false;
            OpenCategory(category);
            return true;
        }

        public void Toggle()
        {
            if (_isOpen) Close();
            else Open();
        }

        public void Open()
        {
            if (_panel == null) return;
            _isOpen = true;
            _isCategoryOpen = false;

            // Reset to categories view
            if (_categoriesObj != null) _categoriesObj.SetActive(true);
            if (_categoryObj != null) _categoryObj.SetActive(false);

            _panelObj.SetActive(true);
            var pos = _panel.anchoredPosition;
            pos.x = _panelWidth;
            _panel.anchoredPosition = pos;
            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosX(0f, 0.35f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }

        public void Close()
        {
            if (_panel == null) return;

            if (_isCategoryOpen)
                CloseCategoryImmediate();

            _isOpen = false;
            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosX(_panelWidth, 0.25f)
                .SetEase(Ease.InCubic)
                .SetUpdate(true)
                .OnComplete(() => _panelObj.SetActive(false));
        }

        private static readonly Dictionary<ShopCategory, string> CategoryLabels = new()
        {
            { ShopCategory.Beds, "Lits" },
            { ShopCategory.Pillows, "Coussins" },
            { ShopCategory.Croquettes, "Croquettes" },
            { ShopCategory.Water, "Eau" },
            { ShopCategory.Balls, "Balles" },
            { ShopCategory.Scratchers, "Griffoirs" },
            { ShopCategory.Litters, "Litières" },
        };

        private void OpenCategory(ShopCategory category)
        {
            if (_categoryObj == null) return;

            _isCategoryOpen = true;
            ClearItemsList();
            PopulateItems(category);

            // Set category name
            if (_categoryNameText != null)
            {
                _categoryNameText.text = CategoryLabels.TryGetValue(category, out var label)
                    ? label : category.ToString();
            }

            // Swap views
            if (_categoriesObj != null) _categoriesObj.SetActive(false);
            _categoryObj.SetActive(true);
        }

        private void PopulateItems(ShopCategory category)
        {
            if (_itemsListParent == null || _itemPrefab == null) return;

            if (!_shopItems.TryGetValue(category, out var items)) return;

            foreach (var objData in items)
            {
                var go = Instantiate(_itemPrefab, _itemsListParent);
                go.name = $"ShopItem_{objData.displayName}";

                // ItemImage
                var itemImageT = FindInChildren(go.transform, "ItemImage");
                if (itemImageT != null)
                {
                    var img = itemImageT.GetComponent<Image>();
                    if (img != null && objData.icon != null)
                        img.sprite = objData.icon;
                }

                // ItemNameLabel
                var nameT = FindInChildren(go.transform, "ItemNameLabel");
                if (nameT != null)
                {
                    var tmp = nameT.GetComponent<TMP_Text>();
                    if (tmp != null) tmp.text = objData.displayName;
                }

                // ItemSpaceLabel
                var spaceT = FindInChildren(go.transform, "ItemSpaceLabel");
                if (spaceT != null)
                {
                    var tmp = spaceT.GetComponent<TMP_Text>();
                    if (tmp != null) tmp.text = $"{objData.size.x} x {objData.size.y}";
                }

                // ItemPriceValue
                var priceT = FindInChildren(go.transform, "ItemPriceValue");
                if (priceT != null)
                {
                    var tmp = priceT.GetComponent<TMP_Text>();
                    if (tmp != null) tmp.text = $"{objData.cost}";
                }

                // Make the whole item tappable → buy + place
                var btn = go.GetComponent<Button>();
                if (btn == null) btn = go.AddComponent<Button>();
                var captured = objData; // capture for lambda
                btn.onClick.AddListener(() => TryBuyAndPlace(captured));
                AddJuice(go.GetComponent<RectTransform>());
            }
        }

        private void TryBuyAndPlace(HotelObjectData data)
        {
            if (_economy == null || _placement == null) return;

            // Check if player can afford it
            if (_economy.Coins < data.cost)
            {
                Debug.LogWarning($"[Shop] Not enough coins for {data.displayName} (need {data.cost}, have {_economy.Coins})");
                return;
            }

            // Close shop and start placement (coins debited on confirm, not now)
            Close();
            _placement.BeginPlacement(data);
        }

        private void CloseCategory()
        {
            _isCategoryOpen = false;
            if (_categoryObj != null) _categoryObj.SetActive(false);
            if (_categoriesObj != null) _categoriesObj.SetActive(true);
        }

        private void CloseCategoryImmediate()
        {
            _isCategoryOpen = false;
            if (_categoryObj != null) _categoryObj.SetActive(false);
            if (_categoriesObj != null) _categoriesObj.SetActive(true);
        }

        private void ClearItemsList()
        {
            if (_itemsListParent == null) return;
            for (int i = _itemsListParent.childCount - 1; i >= 0; i--)
                Destroy(_itemsListParent.GetChild(i).gameObject);
        }

        // --- Helpers ---

        private static void EnsureRaycastBackground(GameObject obj)
        {
            var img = obj.GetComponent<Image>();
            if (img == null)
            {
                img = obj.AddComponent<Image>();
                img.color = Color.clear;
            }
            img.raycastTarget = true;
        }

        private static void AddJuice(RectTransform rt)
        {
            if (rt == null) return;
            if (rt.GetComponent<ButtonJuice>() == null)
                rt.gameObject.AddComponent<ButtonJuice>();
        }

        private static RectTransform FindRect(GameObject root, string childName)
        {
            var t = FindInChildren(root.transform, childName);
            return t != null ? t.GetComponent<RectTransform>() : null;
        }

        private static Transform FindInChildren(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name) return child;
                var found = FindInChildren(child, name);
                if (found != null) return found;
            }
            return null;
        }

        private static GameObject FindInactiveByName(string name)
        {
            var go = GameObject.Find(name);
            if (go != null) return go;

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                foreach (var root in scene.GetRootGameObjects())
                {
                    var found = FindInChildren(root.transform, name);
                    if (found != null) return found.gameObject;
                }
            }
            return null;
        }
    }
}
