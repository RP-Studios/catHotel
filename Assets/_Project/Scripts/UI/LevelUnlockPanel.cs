using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CatHotel.Audio;
using CatHotel.Core;
using CatHotel.Hotel;

namespace CatHotel.UI
{
    /// <summary>
    /// "LevelUnlock" panel: shows current/next reputation level + conditions
    /// + new perks (capacity, floor access) + level-up action.
    ///
    /// Trigger: tap on the HUD's GlobalPex element. Close: tap outside the panel.
    /// </summary>
    public class LevelUnlockPanel : MonoBehaviour
    {
        [SerializeField] private HotelManager _hotel;

        // ---- Resolved scene refs (by name) ----
        private GameObject _panelGo;
        private RectTransform _panelRect;

        private TMP_Text _currentLevelLabel;
        private TMP_Text _currentLevelValueText;
        private TMP_Text _nextLevelLabel;
        private TMP_Text _nextLevelValueText;
        private TMP_Text _newStuffValue;

        // XPCondition
        private GameObject _xpUnchecked;
        private GameObject _xpChecked;
        private TMP_Text   _xpValue;

        // HappyCatsCondition
        private GameObject _happyUnchecked;
        private GameObject _happyChecked;
        private TMP_Text   _happyValue;

        // Newness
        private TMP_Text _newFloorAccessValue;
        private TMP_Text _capacityValue;
        private TMP_Text _currentCapacityValue;
        private TMP_Text _nextCapacityValue;

        // Action
        private TMP_Text _nextLevelPriceValue;
        private RectTransform _nextLevelAction;
        private TMP_Text _nextLevelActionLabel;
        private GameObject _lockerGo;

        // GlobalPex (HUD trigger)
        private RectTransform _globalPexRect;

        private bool _isOpen;
        private int _openedAtFrame = -1;
        private Camera _uiCamera;

        private void Start()
        {
            // Find the panel + its children by name (works even if inactive)
            _panelGo = FindInactiveByName("LevelUnlock");
            if (_panelGo == null)
            {
                Debug.LogWarning("[LevelUnlockPanel] 'LevelUnlock' not found in scene.");
                return;
            }

            _panelRect = _panelGo.GetComponent<RectTransform>();
            EnsureRaycastBackground(_panelGo);

            _currentLevelLabel     = FindTmp(_panelGo, "CurrentLevelLabel");
            _currentLevelValueText = FindTmp(_panelGo, "CurrentLevelValueText");
            _nextLevelLabel        = FindTmp(_panelGo, "NextLevelLabel");
            _nextLevelValueText    = FindTmp(_panelGo, "NextLevelValueText");
            _newStuffValue         = FindTmp(_panelGo, "NewStuffValue");

            // XP condition
            var xpCond = FindGo(_panelGo, "XPCondition");
            if (xpCond != null)
            {
                _xpUnchecked = FindGo(xpCond, "Unchecked");
                _xpChecked   = FindGo(xpCond, "Checked");
                _xpValue     = FindTmp(xpCond, "ConditionValue");
            }
            // Happy-cats condition
            var happyCond = FindGo(_panelGo, "HappyCatsCondition");
            if (happyCond != null)
            {
                _happyUnchecked = FindGo(happyCond, "Unchecked");
                _happyChecked   = FindGo(happyCond, "Checked");
                _happyValue     = FindTmp(happyCond, "ConditionValue");
            }

            _newFloorAccessValue   = FindTmp(_panelGo, "NewFloorAccessValue");
            _capacityValue         = FindTmp(_panelGo, "CapacityValue");
            _currentCapacityValue  = FindTmp(_panelGo, "CurrentCapacityValue");
            _nextCapacityValue     = FindTmp(_panelGo, "NextCapacityValue");

            _nextLevelPriceValue   = FindTmp(_panelGo, "NextLevelPriceValue");
            var actionGo = FindGo(_panelGo, "NextLevelAction");
            if (actionGo != null)
            {
                _nextLevelAction = actionGo.GetComponent<RectTransform>();
                _nextLevelActionLabel = FindTmp(actionGo, "NextLevelLabel");
                EnsureButton(actionGo).onClick.AddListener(OnNextLevelTapped);
            }
            _lockerGo = FindGo(_panelGo, "Locker");

            // Wire ALL GameObjects named "GlobalPex" (the scene has multiple — HUD bar + inner fill).
            int wired = 0;
            foreach (var go in FindAllByName("GlobalPex"))
            {
                _globalPexRect = go.GetComponent<RectTransform>();
                var btn = EnsureButton(go);
                btn.onClick.AddListener(Open);
                EnsureChildrenRaycastable(go);
                wired++;
            }
            Debug.Log($"[LevelUnlockPanel] Wired {wired} GlobalPex trigger(s)");

            // Listen for stats changes to refresh the panel content while open
            if (_hotel != null && _hotel.Reputation != null)
            {
                _hotel.Reputation.OnLevelChanged += _ => { if (_isOpen) Refresh(); };
                _hotel.Reputation.OnXpGained    += _ => { if (_isOpen) Refresh(); };
            }
            if (_hotel != null && _hotel.Economy != null)
            {
                _hotel.Economy.OnCoinsChanged += _ => { if (_isOpen) Refresh(); };
            }

            LocalizedStrings.OnLanguageChanged += () => { if (_isOpen) Refresh(); };

            _panelGo.SetActive(false);
            _isOpen = false;
        }

        private void Update()
        {
            if (!_isOpen) return;
            // Skip the frame the panel was opened on so the opening tap doesn't
            // immediately close it via the outside-tap logic below.
            if (Time.frameCount == _openedAtFrame) return;

            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;

            Vector2 screenPos = pointer.position.ReadValue();
            if (_panelRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_panelRect, screenPos, _uiCamera))
                return;

            Close();
        }

        public void Open()
        {
            if (_panelGo == null) return;
            _panelGo.SetActive(true);
            _isOpen = true;
            _openedAtFrame = Time.frameCount;
            Refresh();
            UISoundManager.Instance?.PlayOpenSection();
        }

        public void Close()
        {
            if (_panelGo == null) return;
            _panelGo.SetActive(false);
            _isOpen = false;
            UISoundManager.Instance?.PlayCloseSection();
        }

        public bool IsOpen => _isOpen;

        // ---- Refresh content ----

        private void Refresh()
        {
            if (_hotel == null || _hotel.Reputation == null) return;
            var rep = _hotel.Reputation;
            int curLevel = rep.Level;
            bool isMax = rep.IsMaxLevel;
            int nextLevel = isMax ? curLevel : curLevel + 1;

            // Headers
            if (_currentLevelLabel != null)
                _currentLevelLabel.text = string.Format(LocalizedStrings.Get("levelup.title.current"), curLevel);
            if (_currentLevelValueText != null)
                _currentLevelValueText.text = LocalizedStrings.Get(rep.LevelNameKey);
            if (_nextLevelLabel != null)
                _nextLevelLabel.text = string.Format(LocalizedStrings.Get("levelup.title.next"), nextLevel);
            if (_nextLevelValueText != null)
                _nextLevelValueText.text = LocalizedStrings.Get($"rep.{nextLevel}");

            if (_newStuffValue != null)
                _newStuffValue.text = LocalizedStrings.Get("levelup.newstuff");
            if (_capacityValue != null)
                _capacityValue.text = LocalizedStrings.Get("levelup.capacity.title");

            // Conditions
            int neededXp = isMax ? rep.Xp : rep.NextLevel.Value.XpRequired;
            int neededHappy = isMax ? 0 : rep.NextLevel.Value.HappyCatsRequired;
            int currentHappy = _hotel.CountHappyCats();
            int currentCoins = _hotel.Economy != null ? _hotel.Economy.Coins : 0;

            bool xpOk = rep.Xp >= neededXp;
            bool happyOk = currentHappy >= neededHappy;
            bool coinsOk = !isMax && currentCoins >= rep.NextLevel.Value.CoinCost;

            if (_xpUnchecked != null) _xpUnchecked.SetActive(!xpOk);
            if (_xpChecked   != null) _xpChecked.SetActive(xpOk);
            if (_xpValue     != null) _xpValue.text = string.Format(
                LocalizedStrings.Get("levelup.condition.xp"), rep.Xp, neededXp);

            if (_happyUnchecked != null) _happyUnchecked.SetActive(!happyOk);
            if (_happyChecked   != null) _happyChecked.SetActive(happyOk);
            if (_happyValue     != null) _happyValue.text = string.Format(
                LocalizedStrings.Get("levelup.condition.happycats"), currentHappy, neededHappy);

            // Newness
            if (_newFloorAccessValue != null)
                _newFloorAccessValue.text = string.Format(LocalizedStrings.Get("levelup.newfloor"), nextLevel);

            int curCap = rep.MaxCats;
            int nextCap = isMax ? curCap : rep.NextLevel.Value.TotalCapacity;
            if (_currentCapacityValue != null) _currentCapacityValue.text = curCap.ToString();
            if (_nextCapacityValue    != null) _nextCapacityValue.text    = nextCap.ToString();

            // Price + action availability
            int price = isMax ? 0 : rep.NextLevel.Value.CoinCost;
            if (_nextLevelPriceValue != null)
                _nextLevelPriceValue.text = price.ToString();

            bool canLevelUp = !isMax && xpOk && happyOk && coinsOk;
            if (_nextLevelActionLabel != null)
            {
                _nextLevelActionLabel.text = LocalizedStrings.Get("levelup.go.next");
                var c = _nextLevelActionLabel.color;
                c.a = canLevelUp ? 1f : 0.5f;
                _nextLevelActionLabel.color = c;
            }
            if (_lockerGo != null)
                _lockerGo.SetActive(!canLevelUp);
        }

        // ---- Actions ----

        private void OnNextLevelTapped()
        {
            if (_hotel == null || _hotel.Reputation == null || _hotel.Economy == null) return;

            int happy = _hotel.CountHappyCats();
            var result = _hotel.Reputation.TryLevelUp(happy, _hotel.Economy.TrySpend);

            if (result == ReputationManager.LevelUpResult.Success)
            {
                UISoundManager.Instance?.PlayTapPositive(); // already random among 6 variants
                Close();
                _hotel.SaveProgression();
            }
            else
            {
                UISoundManager.Instance?.PlayTapNegative();
                Refresh();
            }
        }

        // ---- Helpers ----

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
                    var found = FindDeep(root.transform, name);
                    if (found != null) return found.gameObject;
                }
            }
            return null;
        }

        private static System.Collections.Generic.List<GameObject> FindAllByName(string name)
        {
            var list = new System.Collections.Generic.List<GameObject>();
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                foreach (var root in scene.GetRootGameObjects())
                    CollectByName(root.transform, name, list);
            }
            return list;
        }

        private static void CollectByName(Transform t, string name, System.Collections.Generic.List<GameObject> outList)
        {
            if (t.name == name) outList.Add(t.gameObject);
            for (int i = 0; i < t.childCount; i++)
                CollectByName(t.GetChild(i), name, outList);
        }

        /// <summary>Force every Image / TMP_Text descendant of a Button to be raycast-receiving,
        /// so taps on labels/icons inside the button bubble up to the parent's onClick.</summary>
        private static void EnsureChildrenRaycastable(GameObject root)
        {
            foreach (var img in root.GetComponentsInChildren<Image>(true))
                img.raycastTarget = true;
            foreach (var txt in root.GetComponentsInChildren<TMP_Text>(true))
                txt.raycastTarget = true;
        }

        private static GameObject FindGo(GameObject root, string name)
        {
            if (root == null) return null;
            var t = FindDeep(root.transform, name);
            return t != null ? t.gameObject : null;
        }

        private static TMP_Text FindTmp(GameObject root, string name)
        {
            var go = FindGo(root, name);
            return go != null ? go.GetComponent<TMP_Text>() : null;
        }

        private static Transform FindDeep(Transform parent, string name)
        {
            if (parent.name == name) return parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                var found = FindDeep(parent.GetChild(i), name);
                if (found != null) return found;
            }
            return null;
        }

        private static void EnsureRaycastBackground(GameObject panel)
        {
            // Make sure the panel itself catches taps so they don't bleed through.
            var img = panel.GetComponent<Image>();
            if (img == null)
            {
                img = panel.AddComponent<Image>();
                img.color = new Color(0f, 0f, 0f, 0f); // invisible
            }
            img.raycastTarget = true;
        }

        private static Button EnsureButton(GameObject go)
        {
            var btn = go.GetComponent<Button>();
            if (btn == null) btn = go.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;
            var img = go.GetComponent<Image>();
            if (img == null)
            {
                img = go.AddComponent<Image>();
                img.color = new Color(0f, 0f, 0f, 0f);
            }
            img.raycastTarget = true;
            btn.targetGraphic = img;
            return btn;
        }
    }
}
