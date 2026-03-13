using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace CatHotel.UI
{
    public struct PensionEndData
    {
        public Sprite CatSprite;
        public string CatName;
        public float AvgHappiness;
        public int BaseCoins;
        public int TipCoins;
        public int TotalCoins;
    }

    /// <summary>
    /// End-of-pension recap panel. Pauses the game, shows cat stats and earnings,
    /// animates numbers counting up, then collects coins with fly animation.
    /// Panel stays INACTIVE until Show() is called.
    /// </summary>
    public class EndPensionPanel : MonoBehaviour
    {
        private GameObject _panelObj;
        private RectTransform _panel;
        private float _panelWidth;
        private Tween _slideTween;
        private bool _isOpen;
        private bool _initialized;

        // UI elements
        private Image _catImage;
        private TMP_Text _catName;
        private TMP_Text _happinessRecap;
        private TMP_Text _baseValue;
        private TMP_Text _tipValue;
        private TMP_Text _totalValue;
        private RectTransform _collectRect;
        private RectTransform _doubleRect;

        // Coin fly
        private RectTransform _coinTarget;
        private Sprite _coinSprite;
        private Canvas _overlayCanvas;

        private Action _onCollect;
        private PensionEndData _data;
        private Coroutine _animCoroutine;

        private void Start()
        {
            CacheReferences();
        }

        private void CacheReferences()
        {
            if (_initialized) return;

            _panelObj = FindInactiveByName("EndPensionPanel");
            if (_panelObj == null) return;

            _panel = _panelObj.GetComponent<RectTransform>();

            // Ensure raycastable background
            var panelImg = _panelObj.GetComponent<Image>();
            if (panelImg == null)
            {
                panelImg = _panelObj.AddComponent<Image>();
                panelImg.color = Color.clear;
            }
            panelImg.raycastTarget = true;

            // Find UI elements while panel might still be inactive
            _catImage = FindImage(_panelObj, "CatImage");
            _catName = FindTMP(_panelObj, "CatName");
            _happinessRecap = FindTMP(_panelObj, "HapinessRecapValue");
            _baseValue = FindTMP(_panelObj, "BaseValue");
            _tipValue = FindTMP(_panelObj, "TipValue");
            _totalValue = FindTMP(_panelObj, "TotalValue");
            _collectRect = FindRect(_panelObj, "CollectAction");
            _doubleRect = FindRect(_panelObj, "DoubleGainsAction");

            AddJuice(_collectRect);
            AddJuice(_doubleRect);

            // Coin fly target + sprite
            var coinTargetObj = GameObject.Find("CatCoinsImage");
            if (coinTargetObj != null)
            {
                _coinTarget = coinTargetObj.GetComponent<RectTransform>();
                var coinImg = coinTargetObj.GetComponent<Image>();
                if (coinImg != null) _coinSprite = coinImg.sprite;
            }

            // Overlay canvas for coin fly
            foreach (var c in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                if (c.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    _overlayCanvas = c;
                    break;
                }
            }

            // Keep panel inactive until Show()
            _panelObj.SetActive(false);
            _initialized = true;
        }

        private void Update()
        {
            if (!_isOpen) return;

            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;
            Vector2 screenPos = pointer.position.ReadValue();

            if (_collectRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_collectRect, screenPos, null))
            {
                Collect();
                return;
            }

            // Double gains — rewarded ad placeholder
            if (_doubleRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_doubleRect, screenPos, null))
            {
                // TODO: trigger rewarded ad, then collect double
            }
        }

        public void Show(PensionEndData data, Action onCollect)
        {
            CacheReferences();
            if (_panel == null) return;

            _data = data;
            _onCollect = onCollect;

            // Activate and position offscreen
            _panelObj.SetActive(true);
            Canvas.ForceUpdateCanvases();
            _panelWidth = _panel.rect.width;
            if (_panelWidth <= 0f) _panelWidth = 800f;

            var pos = _panel.anchoredPosition;
            pos.x = _panelWidth;
            _panel.anchoredPosition = pos;

            // Set cat image
            if (_catImage != null && data.CatSprite != null)
            {
                _catImage.sprite = data.CatSprite;
                _catImage.SetNativeSize();
            }

            if (_catName != null)
                _catName.text = data.CatName;

            // Reset values to 0
            if (_happinessRecap != null) _happinessRecap.text = "0%";
            if (_baseValue != null) _baseValue.text = "0";
            if (_tipValue != null) _tipValue.text = "0";
            if (_totalValue != null) _totalValue.text = "0";

            // Slide in
            _isOpen = true;
            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosX(0f, 0.35f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    if (_animCoroutine != null) StopCoroutine(_animCoroutine);
                    _animCoroutine = StartCoroutine(AnimateNumbers());
                });
        }

        private void Close()
        {
            _isOpen = false;
            _slideTween?.Kill();
            if (_animCoroutine != null)
            {
                StopCoroutine(_animCoroutine);
                _animCoroutine = null;
            }

            _slideTween = _panel.DOAnchorPosX(_panelWidth, 0.25f)
                .SetEase(Ease.InCubic)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _panelObj.SetActive(false);
                    var cb = _onCollect;
                    _onCollect = null; // prevent double-invoke
                    cb?.Invoke();
                });
        }

        private IEnumerator AnimateNumbers()
        {
            yield return StartCoroutine(AnimateValue(
                _happinessRecap, 0f, _data.AvgHappiness, 0.6f, v => $"{v:0}%"));

            yield return WaitRealtime(0.15f);

            yield return StartCoroutine(AnimateValue(
                _baseValue, 0f, _data.BaseCoins, 0.5f, v => $"{v:0}"));

            if (_data.TipCoins > 0)
            {
                yield return WaitRealtime(0.1f);
                yield return StartCoroutine(AnimateValue(
                    _tipValue, 0f, _data.TipCoins, 0.4f, v => $"+{v:0}"));
            }
            else
            {
                if (_tipValue != null) _tipValue.text = "-";
            }

            yield return WaitRealtime(0.15f);
            yield return StartCoroutine(AnimateValue(
                _totalValue, 0f, _data.TotalCoins, 0.6f, v => $"{v:0}"));

            if (_totalValue != null)
            {
                var rt = _totalValue.GetComponent<RectTransform>();
                if (rt != null)
                {
                    DOTween.Sequence().SetUpdate(true)
                        .Append(rt.DOPunchScale(Vector3.one * 0.3f, 0.3f, 8, 0.5f));
                }
            }

            _animCoroutine = null;
        }

        private IEnumerator AnimateValue(TMP_Text text, float from, float to, float duration,
            Func<float, string> format)
        {
            if (text == null) yield break;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                float current = Mathf.Lerp(from, to, eased);
                text.text = format(current);
                yield return null;
            }
            text.text = format(to);
        }

        private void Collect()
        {
            if (!_isOpen) return;
            _isOpen = false; // prevent double-tap

            int coinCount = Mathf.Clamp(_data.TotalCoins / 10, 3, 8);
            StartCoroutine(CoinFlySequence(coinCount, Close));
        }

        private IEnumerator CoinFlySequence(int count, Action onDone)
        {
            if (_overlayCanvas == null || _coinTarget == null || _collectRect == null)
            {
                onDone?.Invoke();
                yield break;
            }

            Vector3[] srcCorners = new Vector3[4];
            _collectRect.GetWorldCorners(srcCorners);
            Vector3 srcCenter = (srcCorners[0] + srcCorners[2]) / 2f;

            Vector3[] dstCorners = new Vector3[4];
            _coinTarget.GetWorldCorners(dstCorners);
            Vector3 dstCenter = (dstCorners[0] + dstCorners[2]) / 2f;

            var canvasRt = _overlayCanvas.GetComponent<RectTransform>();
            int completed = 0;

            for (int i = 0; i < count; i++)
            {
                var coinGo = new GameObject($"FlyingCoin_{i}");
                coinGo.transform.SetParent(_overlayCanvas.transform, false);

                var coinImg = coinGo.AddComponent<Image>();
                if (_coinSprite != null)
                    coinImg.sprite = _coinSprite;

                var coinRt = coinGo.GetComponent<RectTransform>();
                coinRt.sizeDelta = new Vector2(32f, 32f);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRt,
                    RectTransformUtility.WorldToScreenPoint(null, srcCenter),
                    null, out Vector2 srcLocal);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRt,
                    RectTransformUtility.WorldToScreenPoint(null, dstCenter),
                    null, out Vector2 dstLocal);

                srcLocal += new Vector2(
                    UnityEngine.Random.Range(-30f, 30f),
                    UnityEngine.Random.Range(-20f, 20f));
                coinRt.anchoredPosition = srcLocal;

                float delay = i * 0.08f;
                float flyDuration = 0.4f;

                var seq = DOTween.Sequence().SetUpdate(true);
                seq.AppendInterval(delay);
                seq.Append(coinRt.DOScale(Vector3.one * 1.3f, 0.08f).SetEase(Ease.OutBack));
                seq.Append(coinRt.DOAnchorPos(dstLocal, flyDuration).SetEase(Ease.InQuad));
                seq.Join(coinRt.DOScale(Vector3.one * 0.4f, flyDuration).SetEase(Ease.InQuad));
                seq.OnComplete(() =>
                {
                    Destroy(coinGo);
                    completed++;

                    if (_coinTarget != null)
                    {
                        _coinTarget.DOKill();
                        _coinTarget.localScale = Vector3.one;
                        _coinTarget.DOPunchScale(Vector3.one * 0.2f, 0.15f, 6, 0.5f)
                            .SetUpdate(true);
                    }
                });
            }

            float maxWait = count * 0.08f + 0.5f + 0.2f;
            float waited = 0f;
            while (waited < maxWait && completed < count)
            {
                waited += Time.unscaledDeltaTime;
                yield return null;
            }

            onDone?.Invoke();
        }

        private static IEnumerator WaitRealtime(float seconds)
        {
            float elapsed = 0f;
            while (elapsed < seconds)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
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

        private static TMP_Text FindTMP(GameObject root, string childName)
        {
            var t = FindInChildren(root.transform, childName);
            return t != null ? t.GetComponent<TMP_Text>() : null;
        }

        private static Image FindImage(GameObject root, string childName)
        {
            var t = FindInChildren(root.transform, childName);
            return t != null ? t.GetComponent<Image>() : null;
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
