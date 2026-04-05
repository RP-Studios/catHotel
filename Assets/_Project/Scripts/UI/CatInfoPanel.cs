using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CatHotel.Hotel;
using CatHotel.Cats;

namespace CatHotel.UI
{
    /// <summary>
    /// Manages the "CatInformationPannel" overlay.
    /// Slides in from the right, displays cat stats, closes on "CloseImage" tap.
    /// </summary>
    public class CatInfoPanel : MonoBehaviour
    {
        [SerializeField] private HotelManager _hotel;
        [SerializeField] private Sprite _pensionIcon;
        [SerializeField] private Sprite _shelterIcon;

        private RectTransform _panel;
        private GameObject _panelObj;
        private RectTransform _closeRect;
        private bool _isOpen;
        private CatInstance _currentCat;

        // Portrait
        private Image _catPortrait;

        // Stay type
        private Image _stayTypeImage;
        private TMP_Text _stayTypeLabel;

        // Pension time container (hidden for refuge)
        private GameObject _remainingTimePensionObj;

        // Text fields
        private TMP_Text _catName;
        private TMP_Text _catSpecies;
        private TMP_Text _timeRemaining;

        // Age, Description & Affinities
        private TMP_Text _catAge;
        private TMP_Text _catDesc;
        private TMP_Text _catSpeciesSpec;

        // Need bars (image Right offset: 150 = 0%, 0 = 100%)
        private RectTransform _happinessBar;
        private TMP_Text _happinessText;
        private RectTransform _angerBar;
        private TMP_Text _angerText;
        private RectTransform _thirstBar;
        private TMP_Text _thirstText;
        private RectTransform _sleepBar;
        private TMP_Text _sleepText;
        private RectTransform _playBar;
        private TMP_Text _playText;
        private RectTransform _cleanBar;
        private TMP_Text _cleanText;

        private const float BarMaxRight = 150f;
        private float _panelWidth;
        private Tween _slideTween;

        // Dirty-checking: only refresh UI when values change
        private const float RefreshInterval = 0.2f;
        private float _refreshTimer;
        private int _prevHappiness = -1;
        private int _prevHunger = -1;
        private int _prevThirst = -1;
        private int _prevSleep = -1;
        private int _prevPlay = -1;
        private int _prevClean = -1;
        private int _prevTimeSec = -1;

        private void Start()
        {
            // GameObject.Find doesn't find inactive objects — search all transforms
            _panelObj = FindInactiveByName("CatInformationPanel");
            if (_panelObj == null)
            {
                Debug.LogWarning("[CatInfoPanel] 'CatInformationPanel' not found in scene.");
                return;
            }

            _panel = _panelObj.GetComponent<RectTransform>();

            // Activate briefly to measure, then deactivate
            _panelObj.SetActive(true);

            var panelImg = _panelObj.GetComponent<Image>();
            if (panelImg == null)
            {
                panelImg = _panelObj.AddComponent<Image>();
                panelImg.color = Color.clear;
            }
            panelImg.raycastTarget = true;

            Canvas.ForceUpdateCanvases();
            _panelWidth = _panel.rect.width;
            if (_panelWidth <= 0f) _panelWidth = 800f;

            // Position off-screen and deactivate
            var pos = _panel.anchoredPosition;
            pos.x = _panelWidth;
            _panel.anchoredPosition = pos;
            _panelObj.SetActive(false);

            // Store CloseImage RectTransform for hit-testing in Update
            var closeTransform = FindInChildren(_panelObj.transform, "CloseImage");
            if (closeTransform != null)
            {
                _closeRect = closeTransform.GetComponent<RectTransform>();
                if (closeTransform.GetComponent<ButtonJuice>() == null)
                    closeTransform.gameObject.AddComponent<ButtonJuice>();
            }

            // Portrait
            var portraitT = FindInChildren(_panelObj.transform, "CatPortrait");
            if (portraitT != null)
                _catPortrait = portraitT.GetComponent<Image>();

            // Stay type
            var stayImgT = FindInChildren(_panelObj.transform, "StayTypeImage");
            if (stayImgT != null)
                _stayTypeImage = stayImgT.GetComponent<Image>();
            _stayTypeLabel = FindText(_panelObj, "StayTypeLabel");

            // Text fields
            _catName = FindText(_panelObj, "CatName");
            _catSpecies = FindText(_panelObj, "CatSpeciesValue");
            _timeRemaining = FindText(_panelObj, "TimeRemainingPensionValue");
            var remainingT = FindInChildren(_panelObj.transform, "RemaingTimePension");
            if (remainingT != null) _remainingTimePensionObj = remainingT.gameObject;

            // Age, Description & Affinities
            _catAge = FindText(_panelObj, "CatAgeValue");
            _catDesc = FindText(_panelObj, "CatDescValue");
            _catSpeciesSpec = FindText(_panelObj, "CatSpeciesSpecValue");

            // Need bars + values
            _happinessBar = FindBar(_panelObj, "HapinessImageValue");
            _happinessText = FindText(_panelObj, "HapinessValue");
            _angerBar = FindBar(_panelObj, "HungerImageValue");
            _angerText = FindText(_panelObj, "HungerValue");
            _thirstBar = FindBar(_panelObj, "StarvingImageValue");
            _thirstText = FindText(_panelObj, "StarvingValue");
            _sleepBar = FindBar(_panelObj, "SleepImageValue");
            _sleepText = FindText(_panelObj, "SleepValue");
            _playBar = FindBar(_panelObj, "PlayImageValue");
            _playText = FindText(_panelObj, "PlayValue");
            _cleanBar = FindBar(_panelObj, "CleanImageValue");
            _cleanText = FindText(_panelObj, "CleanValue");
        }

        private void Update()
        {
            if (!_isOpen) return;

            // Detect tap on CloseImage via pointer (bypass UI event system)
            if (_closeRect != null)
            {
                var pointer = Pointer.current;
                if (pointer != null && pointer.press.wasPressedThisFrame)
                {
                    Vector2 screenPos = pointer.position.ReadValue();
                    if (RectTransformUtility.RectangleContainsScreenPoint(_closeRect, screenPos, null))
                    {
                        Close();
                        return;
                    }
                }
            }

            if (_currentCat == null) return;

            _refreshTimer += Time.deltaTime;
            if (_refreshTimer < RefreshInterval) return;
            _refreshTimer = 0f;
            RefreshValues();
        }

        public void Show(CatInstance cat)
        {
            if (_panel == null || cat == null) return;

            _currentCat = cat;
            _isOpen = true;
            _refreshTimer = 0f;
            ResetDirtyState();

            // Fill static info
            if (_catName != null) _catName.text = cat.CatName;
            if (_catSpecies != null) _catSpecies.text = LocalizeBreedName(cat.Breed.breedName);
            if (_catAge != null) _catAge.text = cat.Breed.size < 1f
                ? Core.LocalizedStrings.Get("cat.kitten") : Core.LocalizedStrings.Get("cat.adult");
            if (_catDesc != null) _catDesc.text = cat.Description ?? "";
            if (_catSpeciesSpec != null)
            {
                var parts = new System.Collections.Generic.List<string>();
                if (cat.LikedBreed != null)
                {
                    string plural = !string.IsNullOrEmpty(cat.LikedBreed.breedNamePlural)
                        ? cat.LikedBreed.breedNamePlural : cat.LikedBreed.breedName;
                    parts.Add(Core.LocalizedStrings.Get("cat.likes", plural));
                }
                if (cat.DislikedBreed != null)
                {
                    string plural = !string.IsNullOrEmpty(cat.DislikedBreed.breedNamePlural)
                        ? cat.DislikedBreed.breedNamePlural : cat.DislikedBreed.breedName;
                    parts.Add(Core.LocalizedStrings.Get("cat.dislikes", plural));
                }
                _catSpeciesSpec.text = parts.Count > 0 ? string.Join("\n", parts) : "";
            }
            if (_catPortrait != null && cat.Breed.frontSprite != null)
            {
                var sprite = cat.IsSpecial && cat.Breed.specialFrontSprite != null
                    ? cat.Breed.specialFrontSprite : cat.Breed.frontSprite;
                _catPortrait.sprite = sprite;

                // Preserve native size and center within parent
                var rt = _catPortrait.rectTransform;
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                _catPortrait.SetNativeSize();
                rt.anchoredPosition = Vector2.zero;
            }

            // Stay type + pension time visibility
            bool isPension = cat.Mode == Core.CatMode.Pension;
            if (_remainingTimePensionObj != null)
                _remainingTimePensionObj.SetActive(isPension);
            if (_stayTypeImage != null)
            {
                var icon = isPension ? _pensionIcon : _shelterIcon;
                if (icon != null) _stayTypeImage.sprite = icon;
            }
            if (_stayTypeLabel != null)
                _stayTypeLabel.text = isPension
                    ? Core.LocalizedStrings.Get("cat.stay.pension") : Core.LocalizedStrings.Get("cat.stay.refuge");

            RefreshValues();

            // Activate + position offscreen before slide
            _panelObj.SetActive(true);
            var p = _panel.anchoredPosition;
            p.x = _panelWidth;
            _panel.anchoredPosition = p;

            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosX(0f, 0.7f).SetEase(Ease.OutCubic);
        }

        public void Close()
        {
            if (_panel == null) return;

            _isOpen = false;
            _currentCat = null;

            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosX(_panelWidth, 0.5f)
                .SetEase(Ease.InCubic)
                .OnComplete(() => _panelObj.SetActive(false));
        }

        public bool IsOpen => _isOpen;

        /// <summary>Find CatInstance from a CatEntity.</summary>
        public CatInstance FindCatInstance(CatEntity entity)
        {
            if (_hotel == null || entity == null) return null;
            foreach (var cat in _hotel.Cats)
                if (cat.Entity == entity)
                    return cat;
            return null;
        }

        private void RefreshValues()
        {
            var cat = _currentCat;
            if (cat == null) return;

            // Time remaining pension
            if (_timeRemaining != null)
            {
                if (cat.Mode == Core.CatMode.Pension && cat.PensionTimeRemaining > 0f)
                {
                    int sec = Mathf.CeilToInt(cat.PensionTimeRemaining);
                    if (sec != _prevTimeSec)
                    {
                        _prevTimeSec = sec;
                        _timeRemaining.text = FormatTime(cat.PensionTimeRemaining);
                    }
                }
                else if (_prevTimeSec != 0)
                {
                    _prevTimeSec = 0;
                    _timeRemaining.text = Core.LocalizedStrings.Get("cat.time.none");
                }
            }

            // Happiness
            if (cat.Happiness != null)
                SetBarIfChanged(_happinessBar, _happinessText, cat.Happiness.Value, ref _prevHappiness);

            // Needs (Anger = Hunger)
            if (cat.Needs != null)
            {
                SetBarIfChanged(_angerBar, _angerText, cat.Needs.Hunger, ref _prevHunger);
                SetBarIfChanged(_thirstBar, _thirstText, cat.Needs.Thirst, ref _prevThirst);
                SetBarIfChanged(_sleepBar, _sleepText, cat.Needs.Sleep, ref _prevSleep);
                SetBarIfChanged(_playBar, _playText, cat.Needs.Play, ref _prevPlay);
                SetBarIfChanged(_cleanBar, _cleanText, cat.Needs.Clean, ref _prevClean);
            }
        }

        private void ResetDirtyState()
        {
            _prevHappiness = _prevHunger = _prevThirst = _prevSleep = _prevPlay = _prevClean = _prevTimeSec = -1;
        }

        // Bar color thresholds
        private static readonly Color BarColorHigh   = new(0.71f, 0.72f, 0.27f, 1f); // #B5B946
        private static readonly Color BarColorMid    = new(0.98f, 0.91f, 0.70f, 1f); // #F9E7B2
        private static readonly Color BarColorLow    = new(0.34f, 0.22f, 0.12f, 1f); // #56381E
        private static readonly Color BarColorCrit   = new(0.83f, 0.31f, 0.31f, 1f); // #D34E4E

        private static void SetBarIfChanged(RectTransform bar, TMP_Text text, float value01to100, ref int prevPct)
        {
            int pct = Mathf.RoundToInt(value01to100);
            if (pct == prevPct) return;
            prevPct = pct;

            if (text != null) text.text = $"{pct}%";
            if (bar != null)
            {
                float right = BarMaxRight * (1f - value01to100 / 100f);
                var offset = bar.offsetMax;
                offset.x = -right;
                bar.offsetMax = offset;

                var img = bar.GetComponent<Image>();
                if (img != null)
                {
                    img.color = pct > 75 ? BarColorHigh
                              : pct > 50 ? BarColorMid
                              : pct > 25 ? BarColorLow
                              : BarColorCrit;
                }
            }
        }

        private static string FormatTime(float seconds)
        {
            int total = Mathf.CeilToInt(seconds);
            int h = total / 3600;
            int m = (total % 3600) / 60;
            int s = total % 60;
            return $"{h:D2}:{m:D2}:{s:D2}";
        }

        private static readonly System.Collections.Generic.Dictionary<string, string> BreedNameToKey = new()
        {
            { "Européen", "breed.europeen" },
            { "Siamois", "breed.siamois" },
            { "Ragdoll", "breed.ragdoll" },
            { "Sibérien", "breed.siberien" },
            { "Chartreux", "breed.chartreux" },
        };

        private static string LocalizeBreedName(string frenchName)
        {
            if (BreedNameToKey.TryGetValue(frenchName, out var key))
                return Core.LocalizedStrings.Get(key);
            return frenchName;
        }

        private static TMP_Text FindText(GameObject root, string childName)
        {
            var t = FindInChildren(root.transform, childName);
            if (t == null) return null;
            return t.GetComponent<TMP_Text>();
        }

        private static RectTransform FindBar(GameObject root, string childName)
        {
            var t = FindInChildren(root.transform, childName);
            if (t == null) return null;
            return t.GetComponent<RectTransform>();
        }

        private static Transform FindInChildren(Transform parent, string name)
        {
            // Recursive search through all children
            foreach (Transform child in parent)
            {
                if (child.name == name) return child;
                var found = FindInChildren(child, name);
                if (found != null) return found;
            }
            return null;
        }

        /// <summary>Find a GameObject by name even if it's inactive.</summary>
        private static GameObject FindInactiveByName(string name)
        {
            // First try the fast path (active objects)
            var go = GameObject.Find(name);
            if (go != null) return go;

            // Search all root objects including inactive
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
