using UnityEngine;
using UnityEngine.UI;

namespace CatHotel.UI
{
    /// <summary>
    /// Frame-by-frame sprite animator for UI Image (Canvas overlay).
    /// Mirrors Core.SpriteFrameAnimator but targets UnityEngine.UI.Image instead of SpriteRenderer.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UIFrameAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] _frames;
        [SerializeField] private float _fps = 10f;

        private Image _image;
        private int _currentFrame;
        private float _timer;
        private float _frameDuration;

        public Sprite[] Frames => _frames;

        public void Init(Sprite[] frames, float fps)
        {
            _frames = frames;
            _fps = fps;
            _frameDuration = _fps > 0f ? 1f / _fps : 0f;
            _currentFrame = 0;
            _timer = 0f;
            if (_image != null && _frames != null && _frames.Length > 0)
                _image.sprite = _frames[0];
        }

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            _currentFrame = 0;
            _timer = 0f;
            if (_image != null && _frames != null && _frames.Length > 0)
                _image.sprite = _frames[0];
        }

        private void Start()
        {
            if (_fps > 0f) _frameDuration = 1f / _fps;
        }

        private void Update()
        {
            if (_image == null || _frames == null || _frames.Length == 0) return;
            if (_frameDuration <= 0f) return;

            _timer += Time.unscaledDeltaTime;
            if (_timer >= _frameDuration)
            {
                _timer -= _frameDuration;
                _currentFrame = (_currentFrame + 1) % _frames.Length;
                _image.sprite = _frames[_currentFrame];
            }
        }
    }
}
