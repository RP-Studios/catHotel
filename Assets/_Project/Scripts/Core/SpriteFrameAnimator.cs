using UnityEngine;

namespace CatHotel.Core
{
    /// <summary>
    /// Simple frame-by-frame sprite animator that doesn't rely on Animator/Controller.
    /// Loads all sub-sprites from a spritesheet and cycles through them.
    /// </summary>
    public class SpriteFrameAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] _frames;
        [SerializeField] private float _fps = 12f;

        private SpriteRenderer _sr;
        private int _currentFrame;
        private float _timer;
        private float _frameDuration;

        public void Init(Sprite[] frames, float fps)
        {
            _frames = frames;
            _fps = fps;
            _frameDuration = 1f / _fps;
            _currentFrame = 0;
            _timer = 0f;
        }

        private void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (_fps > 0f) _frameDuration = 1f / _fps;
        }

        private void Update()
        {
            if (_sr == null || _frames == null || _frames.Length == 0) return;

            _timer += Time.deltaTime;
            if (_timer >= _frameDuration)
            {
                _timer -= _frameDuration;
                _currentFrame = (_currentFrame + 1) % _frames.Length;
                _sr.sprite = _frames[_currentFrame];
            }
        }
    }
}
