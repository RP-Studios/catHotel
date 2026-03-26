using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using CatHotel.Cats;
using CatHotel.UI;

namespace CatHotel.Economy
{
    /// <summary>
    /// Visual layer for floating coins. One coin per cat, stacks grow the coin.
    /// Tap on cat: collect coin if present, else show info panel.
    /// </summary>
    public class FloatingCoinView : MonoBehaviour
    {
        [SerializeField] private EconomyManager _economy;
        [SerializeField] private Sprite _coinSprite;
        [SerializeField] private RuntimeAnimatorController _coinAnimController;
        [SerializeField] private CatSpawner _catSpawner;
        [SerializeField] private CatInfoPanel _catInfoPanel;

        [Header("Floating")]
        [SerializeField] private float _floatHeight = 1.0f;
        [SerializeField] private float _bobAmplitude = 0.15f;
        [SerializeField] private float _bobSpeed = 2f;
        [SerializeField] private float _catTapRadius = 1.0f;

        [Header("Collect Animation")]
        [SerializeField] private float _flyDuration = 0.5f;
        [SerializeField] private float _collectAllCoinCost = 5;

        [Header("SFX")]
        [SerializeField] private AudioClip[] _coinCollectClips;   // Coin-001..005
        [SerializeField] private AudioClip _fullPickUpClip;       // FullPickUp

        private readonly Dictionary<FloatingCoin, GameObject> _coinViews = new();
        private readonly HashSet<FloatingCoin> _collecting = new();
        private readonly List<FloatingCoin> _toRemove = new(); // reusable list
        private Camera _cam;
        private AudioSource _sfxSource;
        private Canvas _overlayCanvas;
        private RectTransform _coinTarget; // CatCoinsImage
        private Button _collectAllBtn;
        private RectTransform _collectAllRt;
        private Tween _collectAllPulse;
        private Image _pigIcon;

        private void Start()
        {
            _cam = Camera.main;

            // SFX source
            _sfxSource = GetComponent<AudioSource>();
            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
                _sfxSource.spatialBlend = 0f; // 2D
            }

            // Find overlay canvas
            foreach (var c in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                if (c.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    _overlayCanvas = c;
                    break;
                }
            }

            // Find CatCoinsImage for fly target
            var targetObj = GameObject.Find("CatCoinsImage");
            if (targetObj != null)
                _coinTarget = targetObj.GetComponent<RectTransform>();

            // Find CollectAllAction button and wire it
            var collectAllObj = GameObject.Find("CollectAllAction");
            if (collectAllObj != null)
            {
                _collectAllBtn = collectAllObj.GetComponent<Button>();
                if (_collectAllBtn == null)
                    _collectAllBtn = collectAllObj.AddComponent<Button>();
                _collectAllBtn.onClick.AddListener(OnCollectAllPressed);
                _collectAllRt = collectAllObj.GetComponent<RectTransform>();

                // Find "Pig" child icon
                var pigTransform = collectAllObj.transform.Find("Pig");
                if (pigTransform != null)
                    _pigIcon = pigTransform.GetComponent<Image>();

                // Tap juice
                if (collectAllObj.GetComponent<CatHotel.UI.ButtonJuice>() == null)
                    collectAllObj.AddComponent<CatHotel.UI.ButtonJuice>();
            }

            if (_economy == null) return;
            _economy.OnCoinSpawned += HandleCoinSpawned;
            _economy.OnCoinStacked += HandleCoinStacked;
            _economy.OnCoinCollected += HandleCoinCollected;
        }

        private void OnDestroy()
        {
            _collectAllPulse?.Kill();
            if (_economy == null) return;
            _economy.OnCoinSpawned -= HandleCoinSpawned;
            _economy.OnCoinStacked -= HandleCoinStacked;
            _economy.OnCoinCollected -= HandleCoinCollected;
        }

        private void HandleCoinSpawned(FloatingCoin coin)
        {
            if (_coinSprite == null) return;

            var go = new GameObject("CoinView");
            go.transform.position = coin.WorldPosition;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _coinSprite;
            sr.sortingOrder = 20;

            if (_coinAnimController != null)
            {
                var anim = go.AddComponent<Animator>();
                anim.runtimeAnimatorController = _coinAnimController;
                anim.Play("CoinSpawn");
                // Transition to idle spin after spawn anim (24 frames @ 24fps = 1s)
                StartCoroutine(TransitionToSpin(anim, 1f));
            }

            _coinViews[coin] = go;
        }

        private System.Collections.IEnumerator TransitionToSpin(Animator anim, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (anim != null) anim.Play("CoinSpin");
        }

        private void HandleCoinStacked(FloatingCoin coin)
        {
            // Punch the coin to show stacking feedback
            if (_coinViews.TryGetValue(coin, out var go) && go != null)
            {
                go.transform.DOKill();
                go.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 6, 0.5f);
            }
        }

        private void HandleCoinCollected(FloatingCoin coin)
        {
            _collecting.Add(coin);
        }

        private void Update()
        {
            UpdateFloatingCoins();
            UpdateCollectAllPulse();
            HandleInput();
        }

        private void UpdateFloatingCoins()
        {
            _toRemove.Clear();
            foreach (var kvp in _coinViews)
            {
                var coin = kvp.Key;
                var go = kvp.Value;
                if (go == null) { _toRemove.Add(coin); continue; }

                // Skip coins that are being collected (flying to UI)
                if (_collecting.Contains(coin)) continue;

                // Follow cat transform
                Vector3 basePos;
                if (coin.CatTransform != null)
                {
                    basePos = coin.CatTransform.position + Vector3.up * _floatHeight;
                    coin.WorldPosition = basePos;
                }
                else
                {
                    basePos = coin.WorldPosition;
                }

                // Bob: PingPong is cheaper than Sin
                float bobT = Mathf.PingPong(Time.time * _bobSpeed + coin.SpawnTime, 1f);
                float bob = (bobT * 2f - 1f) * _bobAmplitude;
                go.transform.position = basePos + Vector3.up * bob;

                // Scale based on stacks: 1.0 at 1 stack → 1.5 at 5 stacks
                float stackScale = 1f + (coin.Stacks - 1) * 0.125f;
                go.transform.localScale = Vector3.one * stackScale;
            }
            for (int i = 0; i < _toRemove.Count; i++)
                _coinViews.Remove(_toRemove[i]);
        }

        private void HandleInput()
        {
            if (_economy == null || _cam == null) return;

            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;

            // Don't process world taps when pointer is over UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            // If info panel is open, any world tap closes it
            if (_catInfoPanel != null && _catInfoPanel.IsOpen)
            {
                _catInfoPanel.Close();
                return;
            }

            Vector2 screenPos = pointer.position.ReadValue();
            Vector3 worldPos = _cam.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;

            // Find closest cat within tap radius
            CatEntity tappedCat = null;
            float bestDist = _catTapRadius;

            if (_catSpawner != null)
            {
                foreach (var cat in _catSpawner.AllCats)
                {
                    if (cat == null) continue;
                    float dist = Vector2.Distance(worldPos, cat.transform.position);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        tappedCat = cat;
                    }
                }
            }

            if (tappedCat == null) return;

            // Check if this cat has a floating coin
            FloatingCoin catCoin = FindCoinForCat(tappedCat.transform);
            if (catCoin != null)
            {
                _economy.StartCollect(catCoin);
                AnimateCollect(catCoin);
                PlayCoinCollectSfx();
            }
            else if (_catInfoPanel != null)
            {
                // No coin — show info panel
                var instance = _catInfoPanel.FindCatInstance(tappedCat);
                if (instance != null)
                    _catInfoPanel.Show(instance);
            }
        }

        private FloatingCoin FindCoinForCat(Transform catTransform)
        {
            foreach (var kvp in _coinViews)
            {
                var coin = kvp.Key;
                if (_collecting.Contains(coin)) continue;
                if (coin.CatTransform == catTransform) return coin;
            }
            return null;
        }

        /// <summary>Pulse the CollectAll button when floating coins exist.</summary>
        private void UpdateCollectAllPulse()
        {
            if (_collectAllRt == null) return;

            bool hasCoins = _coinViews.Count > _collecting.Count;

            if (hasCoins && _collectAllPulse == null)
            {
                _collectAllPulse = _collectAllRt
                    .DOScale(Vector3.one * 1.1f, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
            else if (!hasCoins && _collectAllPulse != null)
            {
                _collectAllPulse.Kill();
                _collectAllPulse = null;
                _collectAllRt.localScale = Vector3.one;
            }
        }

        private void OnCollectAllPressed()
        {
            if (_economy == null) return;

            var pendingCoins = _economy.StartCollectAll();
            if (pendingCoins.Count == 0) return;

            // Cost: 5 coins
            int cost = Mathf.RoundToInt(_collectAllCoinCost);
            if (cost > 0 && !_economy.TrySpend(cost))
                return;

            // Button burst effect: punch + pig spin
            if (_collectAllPulse != null)
            {
                _collectAllPulse.Kill();
                _collectAllPulse = null;
            }
            if (_collectAllRt != null)
            {
                _collectAllRt.localScale = Vector3.one;
                _collectAllRt.DOPunchScale(Vector3.one * 0.3f, 0.4f, 8, 0.5f);
            }
            if (_pigIcon != null)
            {
                _pigIcon.rectTransform.DORotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => _pigIcon.rectTransform.rotation = Quaternion.identity);
            }

            // Play full pickup SFX
            PlayFullPickUpSfx();

            // Stagger coin fly animations
            float delay = 0f;
            foreach (var coin in pendingCoins)
            {
                float d = delay;
                DOVirtual.DelayedCall(d, () => AnimateCollect(coin, true));
                delay += 0.08f;
            }
        }

        private void AnimateCollect(FloatingCoin coin, bool isCollectAll = false)
        {
            if (!_coinViews.TryGetValue(coin, out var worldGo))
            {
                // No view — just deposit
                _economy.DepositCoins(coin.Amount);
                return;
            }

            if (_overlayCanvas == null || _coinTarget == null)
            {
                // No canvas/target — instant collect
                _coinViews.Remove(coin);
                _collecting.Remove(coin);
                if (worldGo != null) Destroy(worldGo);
                _economy.DepositCoins(coin.Amount);
                return;
            }

            // Play collect animation on world coin, then fly
            var anim = worldGo != null ? worldGo.GetComponent<Animator>() : null;
            if (anim != null)
                anim.Play(isCollectAll ? "CoinCollectAll" : "CoinCollect");

            // Get screen pos of the world coin
            Vector3 startScreen = _cam.WorldToScreenPoint(worldGo.transform.position);

            // Destroy world-space sprite
            Destroy(worldGo);
            _coinViews.Remove(coin);

            // Create UI coin on overlay canvas
            var uiCoin = new GameObject("CoinFly");
            uiCoin.transform.SetParent(_overlayCanvas.transform, false);

            var img = uiCoin.AddComponent<Image>();
            img.sprite = _coinSprite;
            img.raycastTarget = false;

            // Match the CatCoinsImage icon size exactly
            var rt = uiCoin.GetComponent<RectTransform>();
            Vector2 targetSize = _coinTarget.sizeDelta;
            if (targetSize.x <= 0f || targetSize.y <= 0f) targetSize = new Vector2(32f, 32f);
            rt.sizeDelta = targetSize;
            var canvasRt = _overlayCanvas.GetComponent<RectTransform>();

            // Convert screen→canvas local coords
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRt, startScreen, null, out var startLocal);
            rt.anchoredPosition = startLocal;

            rt.localScale = Vector3.one;

            // Target position
            Vector3[] corners = new Vector3[4];
            _coinTarget.GetWorldCorners(corners);
            Vector3 targetWorldCenter = (corners[0] + corners[2]) / 2f;
            Vector2 targetScreen = RectTransformUtility.WorldToScreenPoint(null, targetWorldCenter);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRt, targetScreen, null, out var targetLocal);

            // Fly animation: scale up → arc → scale down at target
            var seq = DOTween.Sequence();

            // Pop up
            seq.Append(rt.DOScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutBack));

            // Fly to target + shrink
            seq.Append(rt.DOAnchorPos(targetLocal, _flyDuration).SetEase(Ease.InQuad));
            seq.Join(rt.DOScale(Vector3.one * 0.5f, _flyDuration).SetEase(Ease.InQuad));

            // On complete: destroy, deposit, punch the target icon
            seq.OnComplete(() =>
            {
                Destroy(uiCoin);
                _collecting.Remove(coin);
                _economy.DepositCoins(coin.Amount);

                // Punch the target icon (kill previous to avoid scale drift)
                if (_coinTarget != null)
                {
                    _coinTarget.DOKill();
                    _coinTarget.localScale = Vector3.one;
                    _coinTarget.DOPunchScale(Vector3.one * 0.2f, 0.2f, 6, 0.5f);
                }
            });
        }

        public int PendingCoinCount => _coinViews.Count;

        private void PlayCoinCollectSfx()
        {
            if (_sfxSource == null || _coinCollectClips == null || _coinCollectClips.Length == 0) return;
            var clip = _coinCollectClips[UnityEngine.Random.Range(0, _coinCollectClips.Length)];
            _sfxSource.PlayOneShot(clip, ParametersPanel.EffectsVolume);
        }

        private void PlayFullPickUpSfx()
        {
            if (_sfxSource == null || _fullPickUpClip == null) return;
            _sfxSource.PlayOneShot(_fullPickUpClip, ParametersPanel.EffectsVolume);
        }
    }
}
