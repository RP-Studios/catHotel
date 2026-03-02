using UnityEngine;
using UnityEngine.UI;
using CatHotel.Cats;
using CatHotel.Input;

namespace CatHotel.UI
{
    public class ProtoUI : MonoBehaviour
    {
        [SerializeField] private RoomBuilderInput _roomBuilder;
        [SerializeField] private CatSpawner _catSpawner;
        [SerializeField] private CameraController _cameraController;

        private Button _buildButton;
        private Button _spawnButton;
        private Button _petButton;
        private Slider _zoomSlider;
        private Text _buildLabel;
        private Text _petLabel;
        private bool _updatingSlider;

        private void Start()
        {
            CreateUI();

            if (_cameraController != null)
            {
                _cameraController.OnZoomChanged += SyncSliderFromCamera;
                SyncSliderFromCamera();
            }
        }

        private void OnDestroy()
        {
            if (_cameraController != null)
                _cameraController.OnZoomChanged -= SyncSliderFromCamera;
        }

        private void CreateUI()
        {
            // --- Canvas ---
            var canvasObj = new GameObject("ProtoCanvas");
            canvasObj.transform.SetParent(transform, false);

            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            // --- EventSystem (if missing) ---
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                var esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }

            // --- Bottom bar ---
            var barObj = CreatePanel(canvasObj.transform, "BottomBar",
                new Color(0.1f, 0.1f, 0.12f, 0.85f));

            var barRect = barObj.GetComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0, 0);
            barRect.anchorMax = new Vector2(1, 0);
            barRect.pivot = new Vector2(0.5f, 0);
            barRect.sizeDelta = new Vector2(0, 140);

            var layout = barObj.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 20;
            layout.padding = new RectOffset(30, 30, 15, 15);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            // --- Build button ---
            _buildButton = CreateButton(barObj.transform, "BuildBtn", "Build", 200);
            _buildLabel = _buildButton.GetComponentInChildren<Text>();
            _buildButton.onClick.AddListener(OnBuildPressed);

            // --- Spawn button ---
            _spawnButton = CreateButton(barObj.transform, "SpawnBtn", "Spawn", 200);
            _spawnButton.onClick.AddListener(OnSpawnPressed);

            // --- Happy button ---
            var happyBtn = CreateButton(barObj.transform, "HappyBtn", "Happy", 160);
            happyBtn.GetComponent<Image>().color = new Color(0.3f, 0.7f, 0.3f, 1f);
            happyBtn.onClick.AddListener(OnHappyPressed);

            // --- Unhappy button ---
            var unhappyBtn = CreateButton(barObj.transform, "UnhappyBtn", "Sad", 160);
            unhappyBtn.GetComponent<Image>().color = new Color(0.75f, 0.3f, 0.3f, 1f);
            unhappyBtn.onClick.AddListener(OnUnhappyPressed);

            // --- Pet button ---
            _petButton = CreateButton(barObj.transform, "PetBtn", "Pet", 160);
            _petButton.GetComponent<Image>().color = new Color(0.85f, 0.6f, 0.25f, 1f);
            _petLabel = _petButton.GetComponentInChildren<Text>();
            _petButton.onClick.AddListener(OnPetPressed);

            // --- Zoom slider ---
            _zoomSlider = CreateZoomSlider(barObj.transform);
            _zoomSlider.onValueChanged.AddListener(OnZoomSliderChanged);
        }

        private void OnBuildPressed()
        {
            if (_roomBuilder == null) return;
            _roomBuilder.SetBuildMode(!_roomBuilder.BuildMode);
            _buildLabel.text = _roomBuilder.BuildMode ? "Build ON" : "Build";
        }

        private void OnSpawnPressed()
        {
            if (_catSpawner == null) return;
            _catSpawner.SpawnInitialCats();
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

        private void OnPetPressed()
        {
            if (_catSpawner == null) return;
            _catSpawner.TogglePetting();
            _petLabel.text = _catSpawner.PettingMode ? "Pet ON" : "Pet";
        }

        private void OnZoomSliderChanged(float value)
        {
            if (_updatingSlider || _cameraController == null) return;
            _cameraController.SetZoomNormalized(value);
        }

        private void SyncSliderFromCamera()
        {
            if (_zoomSlider == null || _cameraController == null) return;
            _updatingSlider = true;
            _zoomSlider.value = _cameraController.GetZoomNormalized();
            _updatingSlider = false;
        }

        // --- UI factory helpers ---

        private static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);

            var img = obj.AddComponent<Image>();
            img.color = color;

            return obj;
        }

        private static Button CreateButton(Transform parent, string name, string label, float width)
        {
            var obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);

            var img = obj.AddComponent<Image>();
            img.color = new Color(0.25f, 0.55f, 0.85f, 1f);

            var btn = obj.AddComponent<Button>();
            btn.targetGraphic = img;

            var le = obj.AddComponent<LayoutElement>();
            le.preferredWidth = width;
            le.minWidth = 140;

            // Label
            var txtObj = new GameObject("Label", typeof(RectTransform));
            txtObj.transform.SetParent(obj.transform, false);

            var txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.sizeDelta = Vector2.zero;

            var txt = txtObj.AddComponent<Text>();
            txt.text = label;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 36;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;

            return btn;
        }

        private static Slider CreateZoomSlider(Transform parent)
        {
            var obj = new GameObject("ZoomSlider", typeof(RectTransform));
            obj.transform.SetParent(parent, false);

            var le = obj.AddComponent<LayoutElement>();
            le.flexibleWidth = 1;
            le.minWidth = 200;

            var slider = obj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0f;

            // Background
            var bgObj = new GameObject("Background", typeof(RectTransform));
            bgObj.transform.SetParent(obj.transform, false);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0.35f);
            bgRect.anchorMax = new Vector2(1, 0.65f);
            bgRect.sizeDelta = Vector2.zero;
            var bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.3f, 0.3f, 0.35f, 1f);

            // Fill area
            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(obj.transform, false);
            var fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.35f);
            fillAreaRect.anchorMax = new Vector2(1, 0.65f);
            fillAreaRect.sizeDelta = Vector2.zero;

            var fill = new GameObject("Fill", typeof(RectTransform));
            fill.transform.SetParent(fillArea.transform, false);
            var fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            var fillImg = fill.AddComponent<Image>();
            fillImg.color = new Color(0.25f, 0.55f, 0.85f, 1f);

            slider.fillRect = fillRect;

            // Handle slide area
            var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(obj.transform, false);
            var handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.sizeDelta = Vector2.zero;

            var handle = new GameObject("Handle", typeof(RectTransform));
            handle.transform.SetParent(handleArea.transform, false);
            var handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(40, 0);
            var handleImg = handle.AddComponent<Image>();
            handleImg.color = Color.white;

            slider.handleRect = handleRect;

            return slider;
        }
    }
}
