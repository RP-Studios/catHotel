using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using CatHotel.Audio;
using CatHotel.Cats;
using CatHotel.Grid;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Orchestrates floor switching: camera slight offset, fade overlay, tilemap redraw,
    /// object/cat visibility sync, and input gating.
    /// </summary>
    public class FloorManager : MonoBehaviour
    {
        [SerializeField] private GridRenderer _gridRenderer;
        [SerializeField] private CatSpawner _catSpawner;
        [SerializeField] private Camera _camera;
        [SerializeField] private Image _fadeOverlay;

        [Header("Transition")]
        [SerializeField] private float _fadeDuration = 0.18f;
        [SerializeField] private float _cameraYOffset = 0.5f; // subtle "height" cue

        private int _currentFloor;
        private bool _isTransitioning;
        private float _camBaseY;

        public int CurrentFloor => _currentFloor;
        public bool IsTransitioning => _isTransitioning;

        public event Action<int> OnFloorChanged;

        private static FloorManager _instance;
        public static FloorManager Instance => _instance;

        private void Awake()
        {
            _instance = this;
            _currentFloor = 0;
            if (_gridRenderer == null) _gridRenderer = FindAnyObjectByType<GridRenderer>();
            if (_catSpawner == null) _catSpawner = FindAnyObjectByType<CatSpawner>();
            if (_camera == null) _camera = Camera.main;
            if (_camera != null) _camBaseY = _camera.transform.position.y;
            EnsureFadeOverlay();
            if (_fadeOverlay != null)
            {
                var c = _fadeOverlay.color;
                _fadeOverlay.color = new Color(c.r, c.g, c.b, 0f);
                _fadeOverlay.gameObject.SetActive(true);
                _fadeOverlay.raycastTarget = false;
            }
        }

        private void EnsureFadeOverlay()
        {
            if (_fadeOverlay != null) return;
            // Auto-create a black full-screen Image on the first screen-space Canvas found
            var canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null) return;
            // Find an overlay canvas with highest sorting to draw on top
            foreach (var c in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                if (c.renderMode == RenderMode.ScreenSpaceOverlay &&
                    (canvas == null || c.sortingOrder >= canvas.sortingOrder))
                    canvas = c;
            }

            var existing = canvas.transform.Find("FloorFadeOverlay");
            GameObject go;
            if (existing != null) { go = existing.gameObject; }
            else
            {
                go = new GameObject("FloorFadeOverlay");
                go.transform.SetParent(canvas.transform, false);
            }
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.SetAsLastSibling();

            _fadeOverlay = go.GetComponent<Image>();
            if (_fadeOverlay == null) _fadeOverlay = go.AddComponent<Image>();
            _fadeOverlay.color = new Color(0f, 0f, 0f, 0f);
            _fadeOverlay.raycastTarget = false;
        }

        private void Start()
        {
            // Initial visibility sync (in case restored objects landed on f1 before manager spawned)
            SyncVisibility();
        }

        public bool CanGoUp
        {
            get
            {
                if (_isTransitioning) return false;
                if (_currentFloor >= GridRenderer.FloorCount - 1) return false;
                int next = _currentFloor + 1;
                var prog = FloorProgression.Instance;
                return prog == null || prog.IsUnlocked(next);
            }
        }
        public bool CanGoDown => !_isTransitioning && _currentFloor > 0;

        public void GoUp()
        {
            if (!CanGoUp) return;
            SwitchTo(_currentFloor + 1);
        }

        public void GoDown()
        {
            if (!CanGoDown) return;
            SwitchTo(_currentFloor - 1);
        }

        public void SwitchTo(int targetFloor)
        {
            if (_isTransitioning) return;
            if (targetFloor < 0 || targetFloor >= GridRenderer.FloorCount) return;
            if (targetFloor == _currentFloor) return;

            _isTransitioning = true;
            bool up = targetFloor > _currentFloor;
            UISoundManager.Instance?.PlayTapPositive();

            // Camera "height" cue: post-swap we start offset, and the offset resorbs
            // during the fade-in. Going up → camera starts slightly below baseline and rises;
            // going down → camera starts slightly above and settles.
            float postSwapOffset = up ? -_cameraYOffset : _cameraYOffset;

            // Always refresh the live camera baseline so pan/zoom don't break things.
            if (_camera != null) _camBaseY = _camera.transform.position.y;

            var seq = DOTween.Sequence();

            // Fade to black (camera untouched).
            if (_fadeOverlay != null)
            {
                _fadeOverlay.raycastTarget = true;
                seq.Append(_fadeOverlay.DOFade(1f, _fadeDuration).SetEase(Ease.InOutSine));
            }
            else
            {
                seq.AppendInterval(_fadeDuration);
            }

            // Under black: swap floor, sync visibility, snap camera to offset.
            seq.AppendCallback(() =>
            {
                _currentFloor = targetFloor;
                if (_gridRenderer != null) _gridRenderer.SetCurrentFloor(targetFloor);
                SyncVisibility();
                if (_camera != null)
                {
                    var p = _camera.transform.position;
                    _camera.transform.position = new Vector3(p.x, _camBaseY + postSwapOffset, p.z);
                }
            });

            // Fade back in + camera resorbs to baseline.
            if (_fadeOverlay != null)
            {
                seq.Append(_fadeOverlay.DOFade(0f, _fadeDuration).SetEase(Ease.InOutSine));
            }
            else
            {
                seq.AppendInterval(_fadeDuration);
            }
            if (_camera != null)
            {
                seq.Join(_camera.transform
                    .DOMoveY(_camBaseY, _fadeDuration)
                    .SetEase(Ease.OutSine));
            }

            seq.OnComplete(() =>
            {
                _isTransitioning = false;
                if (_fadeOverlay != null) _fadeOverlay.raycastTarget = false;
                OnFloorChanged?.Invoke(_currentFloor);
            });
        }

        /// <summary>Show objects/cats on current floor, hide the rest.</summary>
        public void SyncVisibility()
        {
            // Objects
            foreach (var obj in ObjectRegistry.Objects)
            {
                var sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null) sr.enabled = obj.FloorIndex == _currentFloor;
            }

            // Cats
            if (_catSpawner != null)
            {
                foreach (var cat in _catSpawner.AllCats)
                    if (cat != null) cat.SyncFloorVisibility();
            }

            // Ground-floor-only decorations (pension/refuge entrance logos, doors, ...)
            if (_gridRenderer != null)
                _gridRenderer.SetGroundFloorPropsVisible(_currentFloor == 0);
        }
    }
}
