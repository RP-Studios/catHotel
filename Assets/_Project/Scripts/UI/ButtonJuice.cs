using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace CatHotel.UI
{
    /// <summary>
    /// Adds satisfying tap feedback to any UI element.
    /// Squash-and-stretch on press + subtle wobble on release.
    /// </summary>
    public class ButtonJuice : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float _punchScale = 0.15f;
        [SerializeField] private float _pressDuration = 0.1f;
        [SerializeField] private float _releaseDuration = 0.3f;

        private RectTransform _rt;
        private Tween _tween;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_rt == null) return;
            _tween?.Kill();
            _rt.localScale = Vector3.one;

            // Squash: slightly wider, shorter
            _tween = _rt.DOScale(new Vector3(1f + _punchScale, 1f - _punchScale, 1f), _pressDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_rt == null) return;
            _tween?.Kill();

            // Bounce back with overshoot + tiny rotation wobble
            var seq = DOTween.Sequence().SetUpdate(true);
            seq.Append(_rt.DOScale(Vector3.one, _releaseDuration).SetEase(Ease.OutBack, 3f));
            seq.Join(_rt.DORotate(new Vector3(0, 0, Random.Range(-3f, 3f)), _releaseDuration * 0.5f)
                .SetEase(Ease.OutQuad));
            seq.Append(_rt.DORotate(Vector3.zero, _releaseDuration * 0.5f).SetEase(Ease.OutQuad));
            _tween = seq;
        }

        private void OnDisable()
        {
            _tween?.Kill();
            if (_rt != null)
            {
                _rt.localScale = Vector3.one;
                _rt.rotation = Quaternion.identity;
            }
        }
    }
}
