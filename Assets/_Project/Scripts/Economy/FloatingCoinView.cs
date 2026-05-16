using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CatHotel.Audio;
using CatHotel.Cats;
using CatHotel.Core;
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
        [SerializeField] private AudioClip[] _coinCollectClips;
        [SerializeField] private AudioClip _fullPickUpClip;

        private readonly Dictionary<FloatingCoin, GameObject> _coinViews = new();
        private readonly List<FloatingCoin> _toRemove = new();
        private Camera _cam;
        private AudioSource _sfxSource;
        private Canvas _overlayCanvas;
        private RectTransform _coinTarget;
        private Button _collectAllBtn;
        private RectTransform _collectAllRt;
        private Tween _collectAllPulse;
        private Image _pigIcon;

        private void Start()
        {
            _cam = Camera.main;

            _sfxSource = GetComponent<AudioSource>();
            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
                _sfxSource.spatialBlend = 0f;
            }

            foreach (var c in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                if (c.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    _overlayCanvas = c;
                    break;
                }
            }

            var targetObj = GameObject.Find("CatCoinsImage");
            if (targetObj != null)
                _coinTarget = targetObj.GetComponent<RectTransform>();

            var collectAllObj = GameObject.Find("CollectAllAction");
            if (collectAllObj != null)
            {
                _collectAllBtn = collectAllObj.GetComponent<Button>();
                if (_collectAllBtn == null)
                    _collectAllBtn = collectAllObj.AddComponent<Button>();
                _collectAllBtn.onClick.AddListener(OnCollectAllPressed);
                _collectAllRt = collectAllObj.GetComponent<RectTransform>();

                var pigTransform = collectAllObj.transform.Find("Pig");
                if (pigTransform != null)
                    _pigIcon = pigTransform.GetComponent<Image>();

                // Localized label "Tout collecter" (inactive-aware: include hidden children)
                foreach (var tmp in collectAllObj.GetComponentsInChildren<TMP_Text>(true))
                {
                    if (tmp.name == "CollectCoinsLabel")
                    {
                        tmp.text = LocalizedStrings.Get("hud.collect.label");
                        break;
                    }
                }

                if (collectAllObj.GetComponent<ButtonJuice>() == null)
                    collectAllObj.AddComponent<ButtonJuice>();
            }

            if (_economy == null) return;
            _economy.OnCoinSpawned += HandleCoinSpawned;
            _economy.OnCoinStacked += HandleCoinStacked;
        }

        private void OnDestroy()
        {
            _collectAllPulse?.Kill();
            if (_economy == null) return;
            _economy.OnCoinSpawned -= HandleCoinSpawned;
            _economy.OnCoinStacked -= HandleCoinStacked;
        }

        private void HandleCoinSpawned(FloatingCoin coin)
        {
            if (_coinSprite == null) return;

            var go = new GameObject("CoinView");
            go.transform.position = coin.WorldPosition;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _coinSprite;
            sr.sortingLayerName = "Bubbles";
            sr.sortingOrder = 0;

            if (_coinAnimController != null)
            {
                var anim = go.AddComponent<Animator>();
                anim.runtimeAnimatorController = _coinAnimController;
                anim.Play("CoinSpawn");
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
            if (_coinViews.TryGetValue(coin, out var go) && go != null)
            {
                go.transform.DOKill();
                go.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 6, 0.5f);
            }
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

                // Orphan cleanup: cat was destroyed but coin view remains
                if (coin.CatTransform == null)
                {
                    Destroy(go);
                    _economy.DepositCoins(coin.Amount);
                    _toRemove.Add(coin);
                    continue;
                }

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

                float bobT = Mathf.PingPong(Time.time * _bobSpeed + coin.SpawnTime, 1f);
                float bob = (bobT * 2f - 1f) * _bobAmplitude;
                go.transform.position = basePos + Vector3.up * bob;

                float stackScale = 1f + (coin.Stacks - 1) * 0.125f;
                go.transform.localScale = Vector3.one * stackScale;

                // Hide coin if its cat is on a hidden floor (matches the cat's renderer state).
                var catSr = coin.CatTransform != null ? coin.CatTransform.GetComponent<SpriteRenderer>() : null;
                var coinSr = go.GetComponent<SpriteRenderer>();
                if (coinSr != null)
                    coinSr.enabled = catSr == null || catSr.enabled;
            }
            for (int i = 0; i < _toRemove.Count; i++)
                _coinViews.Remove(_toRemove[i]);
        }

        private void HandleInput()
        {
            if (_economy == null || _cam == null) return;

            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            Vector2 screenPos = pointer.position.ReadValue();
            if (float.IsNaN(screenPos.x) || float.IsNaN(screenPos.y)) return;
            Vector3 worldPos = _cam.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;

            CatEntity tappedCat = null;
            float bestDist = _catTapRadius;

            if (_catSpawner != null)
            {
                foreach (var cat in _catSpawner.AllCats)
                {
                    if (cat == null) continue;
                    // Skip cats on hidden floors (their renderer is disabled).
                    if (cat.SpriteRenderer == null || !cat.SpriteRenderer.enabled) continue;
                    float dist = Vector2.Distance(worldPos, cat.transform.position);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        tappedCat = cat;
                    }
                }
            }

            // No cat under pointer: empty-space tap closes the info panel if open.
            if (tappedCat == null)
            {
                if (_catInfoPanel != null && _catInfoPanel.IsOpen)
                    _catInfoPanel.Close();
                return;
            }

            // Tap on a cat: try coin collection first, otherwise open/switch the info panel.
            FloatingCoin catCoin = FindCoinForCat(tappedCat.transform);
            if (catCoin != null)
            {
                CollectCoin(catCoin);
                PlayCoinCollectSfx();
                UISoundManager.Instance?.PlayTapPositive();
            }
            else if (_catInfoPanel != null)
            {
                CatSoundManager.Instance?.PlayMeowForCat(tappedCat);
                var instance = _catInfoPanel.FindCatInstance(tappedCat);
                if (instance != null)
                    _catInfoPanel.Show(instance); // handles both first-open and cat-switch
            }
        }

        private FloatingCoin FindCoinForCat(Transform catTransform)
        {
            foreach (var kvp in _coinViews)
            {
                if (kvp.Value == null) continue;
                if (kvp.Key.CatTransform == catTransform) return kvp.Key;
            }
            return null;
        }

        /// <summary>Collect a single coin: remove from economy, animate fly, deposit on complete.</summary>
        private void CollectCoin(FloatingCoin coin)
        {
            // Remove from economy tracking immediately
            _economy.StartCollect(coin);
            // Animate and deposit on completion
            AnimateCollect(coin);
            // Tutorial: notify coin collected
            CatHotel.Tutorial.TutorialManager.Instance?.NotifyEvent(
                CatHotel.Tutorial.TutorialTrigger.WaitForCoinCollected);
        }

        /// <summary>Pulse the CollectAll button when floating coins exist.</summary>
        private void UpdateCollectAllPulse()
        {
            if (_collectAllRt == null) return;

            bool hasCoins = _coinViews.Count > 0;

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

            // Snapshot all current coins
            var coins = new List<FloatingCoin>(_coinViews.Keys);
            if (coins.Count == 0) return;

            int cost = Mathf.RoundToInt(_collectAllCoinCost);
            if (cost > 0 && !_economy.TrySpend(cost))
                return;

            // Button burst effect
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

            PlayFullPickUpSfx();

            // Remove all from economy first, then animate
            foreach (var coin in coins)
                _economy.StartCollect(coin);

            float delay = 0f;
            foreach (var coin in coins)
            {
                float d = delay;
                var c = coin; // capture for lambda
                DOVirtual.DelayedCall(d, () => AnimateCollect(c, true));
                delay += 0.08f;
            }
        }

        private void AnimateCollect(FloatingCoin coin, bool isCollectAll = false)
        {
            // Get and remove the world view
            if (!_coinViews.TryGetValue(coin, out var worldGo))
            {
                // No view — just deposit directly
                _economy.DepositCoins(coin.Amount);
                return;
            }
            _coinViews.Remove(coin);

            if (worldGo == null)
            {
                _economy.DepositCoins(coin.Amount);
                return;
            }

            if (_overlayCanvas == null || _coinTarget == null || _cam == null)
            {
                Destroy(worldGo);
                _economy.DepositCoins(coin.Amount);
                return;
            }

            // Play collect animation on world sprite
            var anim = worldGo.GetComponent<Animator>();
            if (anim != null)
                anim.Play(isCollectAll ? "CoinCollectAll" : "CoinCollect");

            // Capture screen position, then destroy world GO and create UI fly
            Vector3 startScreen = _cam.WorldToScreenPoint(worldGo.transform.position);

            // Delay destruction slightly so collect anim is visible (0.15s)
            float collectAnimDelay = 0.15f;
            var capturedGo = worldGo; // capture for lambda
            var capturedAmount = coin.Amount;

            DOVirtual.DelayedCall(collectAnimDelay, () =>
            {
                // Re-capture screen pos (coin may have moved with cat)
                if (capturedGo != null)
                {
                    startScreen = _cam.WorldToScreenPoint(capturedGo.transform.position);
                    Destroy(capturedGo);
                }

                FlyToTarget(startScreen, capturedAmount, isCollectAll);
            });
        }

        private void FlyToTarget(Vector3 startScreen, int amount, bool isCollectAll)
        {
            var canvasRt = _overlayCanvas.GetComponent<RectTransform>();

            var uiCoin = new GameObject("CoinFly");
            uiCoin.transform.SetParent(_overlayCanvas.transform, false);

            var img = uiCoin.AddComponent<Image>();
            img.sprite = _coinSprite;
            img.raycastTarget = false;

            var rt = uiCoin.GetComponent<RectTransform>();
            Vector2 targetSize = _coinTarget.sizeDelta;
            if (targetSize.x <= 0f || targetSize.y <= 0f) targetSize = new Vector2(32f, 32f);
            rt.sizeDelta = targetSize;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRt, startScreen, null, out var startLocal);
            rt.anchoredPosition = startLocal;
            rt.localScale = Vector3.one;

            Vector3[] corners = new Vector3[4];
            _coinTarget.GetWorldCorners(corners);
            Vector3 targetWorldCenter = (corners[0] + corners[2]) / 2f;
            Vector2 targetScreen = RectTransformUtility.WorldToScreenPoint(null, targetWorldCenter);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRt, targetScreen, null, out var targetLocal);

            var seq = DOTween.Sequence();
            seq.Append(rt.DOScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutBack));
            seq.Append(rt.DOAnchorPos(targetLocal, _flyDuration).SetEase(Ease.InQuad));
            seq.Join(rt.DOScale(Vector3.one * 0.5f, _flyDuration).SetEase(Ease.InQuad));
            seq.OnComplete(() =>
            {
                Destroy(uiCoin);
                _economy.DepositCoins(amount);

                if (_coinTarget != null)
                {
                    _coinTarget.DOKill();
                    _coinTarget.localScale = Vector3.one;
                    _coinTarget.DOPunchScale(Vector3.one * 0.2f, 0.2f, 6, 0.5f);
                }
            });
        }

        /// <summary>
        /// Auto-collect coin for a specific cat (pension departure).
        /// Animates if visible, otherwise deposits instantly.
        /// </summary>
        public void CollectCoinForCat(Transform catTransform)
        {
            var coin = FindCoinForCat(catTransform);
            if (coin == null) return;
            _economy.StartCollect(coin);
            AnimateCollect(coin);
        }

        /// <summary>
        /// Instantly remove and deposit coin for a cat without animation (cleanup).
        /// </summary>
        public void ForceCollectCoinForCat(Transform catTransform)
        {
            var coin = FindCoinForCat(catTransform);
            if (coin == null) return;
            _economy.StartCollect(coin);

            if (_coinViews.TryGetValue(coin, out var go))
            {
                if (go != null) Destroy(go);
                _coinViews.Remove(coin);
            }
            _economy.DepositCoins(coin.Amount);
        }

        public int PendingCoinCount => _coinViews.Count;

        private void PlayCoinCollectSfx()
        {
            if (_sfxSource == null || _coinCollectClips == null || _coinCollectClips.Length == 0) return;
            var clip = _coinCollectClips[Random.Range(0, _coinCollectClips.Length)];
            _sfxSource.PlayOneShot(clip, ParametersPanel.EffectsVolume);
        }

        private void PlayFullPickUpSfx()
        {
            if (_sfxSource == null || _fullPickUpClip == null) return;
            _sfxSource.PlayOneShot(_fullPickUpClip, ParametersPanel.EffectsVolume);
        }
    }
}
