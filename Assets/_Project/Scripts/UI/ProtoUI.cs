using UnityEngine;
using UnityEngine.UI;
using CatHotel.Cats;
using CatHotel.Input;
using CatHotel.Shop;

namespace CatHotel.UI
{
    public class ProtoUI : MonoBehaviour
    {
        [SerializeField] private RoomBuilderInput _roomBuilder;
        [SerializeField] private CatSpawner _catSpawner;
        [SerializeField] private CameraController _cameraController;

        [Header("UI Zones (set by ProtoSceneSetup)")]
        [SerializeField] private RectTransform _zoneTop;
        [SerializeField] private RectTransform _zoneLeft;
        [SerializeField] private RectTransform _zoneRight;
        [SerializeField] private RectTransform _zoneCenter;

        [Header("Shop")]
        [SerializeField] private ShopUI _shopUI;

        public RectTransform ZoneTop    => _zoneTop;
        public RectTransform ZoneLeft   => _zoneLeft;
        public RectTransform ZoneRight  => _zoneRight;
        public RectTransform ZoneCenter => _zoneCenter;

        private void Start()
        {
            WireToolbarButtons();
        }

        private void WireToolbarButtons()
        {
            if (_zoneRight == null) return;

            // 5 children in ZoneRight: Spawn, (rien), (rien), Boutique, Build
            System.Action[] actions =
            {
                OnSpawnPressed,
                null,
                null,
                OnShopPressed,
                OnBuildPressed,
            };

            int count = Mathf.Min(_zoneRight.childCount, actions.Length);
            for (int i = 0; i < count; i++)
            {
                if (actions[i] == null) continue;

                var child = _zoneRight.GetChild(i).gameObject;

                var btn = child.GetComponent<Button>();
                if (btn == null)
                    btn = child.AddComponent<Button>();

                var img = child.GetComponent<Image>();
                if (img != null)
                    btn.targetGraphic = img;

                int idx = i;
                btn.onClick.AddListener(() => actions[idx]());
            }
        }

        private void OnBuildPressed()
        {
            if (_roomBuilder == null) return;
            _roomBuilder.SetBuildMode(!_roomBuilder.BuildMode);
        }

        private void OnSpawnPressed()
        {
            if (_catSpawner == null) return;
            _catSpawner.SpawnInitialCats();
        }

        private void OnPetPressed()
        {
            if (_catSpawner == null) return;
            _catSpawner.TogglePetting();
        }

        private void OnHappyPressed()
        {
            var cat = _catSpawner != null ? _catSpawner.GetRandomCat() : null;
            if (cat != null) cat.PlayHappy();
        }

        private void OnUnhappyPressed()
        {
            var cat = _catSpawner != null ? _catSpawner.GetRandomCat() : null;
            if (cat != null) cat.PlayUnhappy();
        }

        private void OnShopPressed()
        {
            if (_shopUI == null) return;
            _shopUI.Toggle();
        }
    }
}
