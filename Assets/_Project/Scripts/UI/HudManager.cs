using UnityEngine;
using TMPro;
using CatHotel.Hotel;
using CatHotel.Economy;

namespace CatHotel.UI
{
    /// <summary>
    /// Finds named UI objects in the scene and keeps them updated.
    /// Coins counter animates 1-by-1 toward the real value.
    /// </summary>
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private HotelManager _hotel;
        [SerializeField] private EconomyManager _economy;
        [SerializeField] private ReputationManager _reputation;

        private TMP_Text _coinsText;
        private TMP_Text _purrlsText;
        private TMP_Text _capacityText;
        private TMP_Text _comfortText;
        private TMP_Text _floorText;
        private TMP_Text _timerText;
        private RectTransform _timerImage;

        // Level / XP UI
        private TMP_Text _nextLevelObjective;
        private TMP_Text _currentLvlValue;
        private TMP_Text _currentLvlDesc;
        private TMP_Text _nextLvlValue;
        private TMP_Text _nextLvlDesc;
        private RectTransform _pexImage;

        // PexImageValue right offset: 900 = empty, 97 = full
        private const float PexRightEmpty = 900f;
        private const float PexRightFull = 97f;

        // Animated coin counter
        private int _displayedCoins;
        private int _targetCoins;
        private float _coinTickTimer;
        private bool _coinsInitialized;
        private const float CoinTickInterval = 0.04f; // 25 ticks/sec

        // Timer image: sliced, right padding goes from 160 (full) to 5 (empty)
        private const float TimerImageRightFull = 160f;
        private const float TimerImageRightEmpty = 5f;

        // Dirty-checking: cache previous values to avoid redundant UI updates
        private int _prevCapacityPct = -1;
        private int _prevComfort = -1;
        private int _prevTimerSec = -1;
        private int _prevLevel = -1;
        private int _prevXp = -1;

        private void Start()
        {
            _coinsText = FindText("CoinsCounter");
            _purrlsText = FindText("PurrlsCounter");
            _capacityText = FindText("CapacityPct");
            _comfortText = FindText("ComfortLevel");
            _floorText = FindText("CurrentFloorIndex");
            _timerText = FindText("NexCatTimerSec");

            // Level / XP UI
            _nextLevelObjective = FindText("NextLevelObjectiveCurrentValue");
            _currentLvlValue = FindText("CurrentLvlValue");
            _currentLvlDesc = FindText("CurrentLvlDesc");
            _nextLvlValue = FindText("NextLvlValue");
            _nextLvlDesc = FindText("NextLvlDesc");

            var pexObj = GameObject.Find("PexImageValue");
            if (pexObj != null)
                _pexImage = pexObj.GetComponent<RectTransform>();

            var timerImgObj = GameObject.Find("NextCatTimerImage");
            if (timerImgObj != null)
                _timerImage = timerImgObj.GetComponent<RectTransform>();

            if (_economy != null)
            {
                _economy.OnCoinsChanged += OnCoinsChanged;
                _economy.OnGemsChanged += g => UpdatePurrls(g);
            }

            // Initial values (no animation)
            if (_economy != null)
            {
                _displayedCoins = _economy.Coins;
                _targetCoins = _economy.Coins;
                RefreshCoinsDisplay();
                UpdatePurrls(_economy.Gems);
            }
        }

        private void Update()
        {
            if (_hotel == null) return;

            AnimateCoinCounter();
            UpdateCapacity();
            UpdateComfort();
            UpdateFloor();
            UpdateTimer();
            UpdateLevel();
        }

        private void OnCoinsChanged(int newTotal)
        {
            if (!_coinsInitialized)
            {
                // First event (after Init): snap display instantly, no animation
                _coinsInitialized = true;
                _displayedCoins = newTotal;
                RefreshCoinsDisplay();
            }
            _targetCoins = newTotal;
        }

        private void AnimateCoinCounter()
        {
            if (_displayedCoins == _targetCoins) return;

            _coinTickTimer += Time.deltaTime;
            if (_coinTickTimer < CoinTickInterval) return;
            _coinTickTimer = 0f;

            int diff = _targetCoins - _displayedCoins;
            // Step scales with distance: always finishes in ~0.5s max
            int step = Mathf.Max(1, Mathf.Abs(diff) / 12);
            _displayedCoins += diff > 0 ? step : -step;

            // Clamp to avoid overshooting
            if ((diff > 0 && _displayedCoins > _targetCoins) ||
                (diff < 0 && _displayedCoins < _targetCoins))
                _displayedCoins = _targetCoins;

            RefreshCoinsDisplay();
        }

        private void RefreshCoinsDisplay()
        {
            if (_coinsText != null)
                _coinsText.text = FormatNumber(_displayedCoins);
        }

        private void UpdatePurrls(int gems)
        {
            if (_purrlsText != null)
                _purrlsText.text = FormatNumber(gems);
        }

        private void UpdateCapacity()
        {
            if (_capacityText == null) return;

            int current = _hotel.CatCount;
            int max = _hotel.Config != null ? _hotel.Config.maxCats : 20;
            int pct = max > 0 ? Mathf.RoundToInt((float)current / max * 100f) : 0;
            if (pct == _prevCapacityPct) return;
            _prevCapacityPct = pct;
            _capacityText.text = $"{pct}%";
        }

        private void UpdateComfort()
        {
            if (_comfortText == null) return;

            int max = _hotel.Config != null ? _hotel.Config.maxCats : 20;
            float comfort = ObjectRegistry.CalculateComfort(_hotel.CatCount, max);
            int rounded = Mathf.RoundToInt(comfort);
            if (rounded == _prevComfort) return;
            _prevComfort = rounded;
            _comfortText.text = $"{rounded}";
        }

        private void UpdateFloor()
        {
            // Static value — set once then skip
            if (_floorText == null || _floorText.text == "RDC") return;
            _floorText.text = "RDC";
        }

        private void UpdateTimer()
        {
            if (_hotel.Config == null) return;

            float interval = _hotel.Config.arrivalInterval;
            float remaining = interval - _hotel.ArrivalTimer;
            int sec = Mathf.CeilToInt(remaining);

            // Only update text when the displayed second changes
            if (sec != _prevTimerSec)
            {
                _prevTimerSec = sec;

                if (_timerText != null)
                {
                    if (remaining >= 60f)
                    {
                        int min = sec / 60;
                        int s = sec % 60;
                        _timerText.text = $"{min}min {s:00}s";
                    }
                    else
                    {
                        _timerText.text = $"{sec}s";
                    }
                }
            }

            if (_timerImage != null)
            {
                float t = Mathf.Clamp01(remaining / interval);
                float right = Mathf.Lerp(TimerImageRightEmpty, TimerImageRightFull, t);
                var offset = _timerImage.offsetMax;
                offset.x = -right;
                _timerImage.offsetMax = offset;
            }
        }

        private void UpdateLevel()
        {
            if (_reputation == null) return;

            int level = _reputation.Level;
            int xp = _reputation.Xp;

            // Dirty check
            if (level == _prevLevel && xp == _prevXp) return;
            _prevLevel = level;
            _prevXp = xp;

            var current = _reputation.CurrentLevel;
            var next = _reputation.NextLevel;

            // Current level
            if (_currentLvlValue != null)
                _currentLvlValue.text = $"Niveau {level}";
            if (_currentLvlDesc != null)
                _currentLvlDesc.text = current.Name;

            if (next.HasValue)
            {
                // Count cats meeting happiness threshold
                int qualifiedCats = 0;
                foreach (var cat in _hotel.Cats)
                {
                    if (cat.Happiness != null && cat.Happiness.Value >= next.Value.MinHappiness)
                        qualifiedCats++;
                }

                if (_nextLvlValue != null)
                    _nextLvlValue.text = $"Niveau {next.Value.Index}";
                if (_nextLvlDesc != null)
                    _nextLvlDesc.text = next.Value.Name;
                if (_nextLevelObjective != null)
                    _nextLevelObjective.text = $"{qualifiedCats}/{next.Value.CatsRequired} chats à +{next.Value.MinHappiness:0}% de bonheur";

                // XP bar based on accumulated XP progress
                if (_pexImage != null)
                {
                    float progress = _reputation.XpProgress;
                    float right = Mathf.Lerp(PexRightEmpty, PexRightFull, progress);
                    var offset = _pexImage.offsetMax;
                    offset.x = -right;
                    _pexImage.offsetMax = offset;
                }
            }
            else
            {
                if (_nextLvlValue != null)
                    _nextLvlValue.text = "MAX";
                if (_nextLvlDesc != null)
                    _nextLvlDesc.text = "";
                if (_nextLevelObjective != null)
                    _nextLevelObjective.text = "Niveau maximum atteint !";
                if (_pexImage != null)
                {
                    var offset = _pexImage.offsetMax;
                    offset.x = -PexRightFull;
                    _pexImage.offsetMax = offset;
                }
            }
        }

        private static TMP_Text FindText(string name)
        {
            var go = GameObject.Find(name);
            if (go == null)
            {
                Debug.LogWarning($"[HUD] UI object '{name}' not found in scene.");
                return null;
            }
            var tmp = go.GetComponent<TMP_Text>();
            if (tmp == null)
                Debug.LogWarning($"[HUD] '{name}' has no TMP_Text component.");
            return tmp;
        }

        private static string FormatNumber(int value)
        {
            if (value >= 1_000_000) return $"{value / 1_000_000f:F1}M";
            if (value >= 1_000) return $"{value / 1_000f:F1}K";
            return value.ToString();
        }
    }
}
