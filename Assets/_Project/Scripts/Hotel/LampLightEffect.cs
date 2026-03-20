using UnityEngine;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Simple sprite animation for lamp light effects.
    /// Cycles through sprites at a given frame rate.
    /// </summary>
    public class LampLightEffect : MonoBehaviour
    {
        [SerializeField] private Sprite[] _frames;
        [SerializeField] private float _fps = 10f;

        private SpriteRenderer _sr;
        private float _timer;
        private int _currentFrame;

        public void Init(Sprite[] frames, float fps)
        {
            _frames = frames;
            _fps = fps;
        }

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_frames == null || _frames.Length == 0) return;

            _timer += Time.deltaTime;
            float frameDuration = 1f / _fps;
            if (_timer >= frameDuration)
            {
                _timer -= frameDuration;
                _currentFrame = (_currentFrame + 1) % _frames.Length;
                _sr.sprite = _frames[_currentFrame];
            }
        }
    }
}
