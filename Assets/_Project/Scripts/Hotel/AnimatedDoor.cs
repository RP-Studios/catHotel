using System.Collections;
using UnityEngine;

namespace CatHotel.Hotel
{
    /// <summary>
    /// Animated door that opens when a cat passes through and closes after.
    /// Displays frame 0 (closed) by default.
    /// When triggered: plays open animation (frames 0→8), waits, plays close (frames 8→0).
    /// </summary>
    public class AnimatedDoor : MonoBehaviour
    {
        [SerializeField] private float _openSpeed = 18f;   // frames per second for opening
        [SerializeField] private float _closeSpeed = 18f;   // frames per second for closing
        [SerializeField] private float _holdDuration = 1f;  // seconds to stay open

        private SpriteRenderer _sr;
        private Sprite[] _frames;
        private bool _isAnimating;

        public void Init(Sprite[] frames)
        {
            _frames = frames;
            _sr = GetComponent<SpriteRenderer>();
            if (_sr == null)
                _sr = gameObject.AddComponent<SpriteRenderer>();

            _sr.sortingLayerName = "Objects";
            _sr.sortingOrder = 0;

            // Show first frame (closed)
            if (_frames != null && _frames.Length > 0)
                _sr.sprite = _frames[0];
        }

        /// <summary>Play open → hold → close sequence. Can be called while already animating (resets).</summary>
        public void PlayOpenClose()
        {
            if (_frames == null || _frames.Length < 2) return;
            if (_isAnimating) return;
            StartCoroutine(OpenCloseSequence());
        }

        private IEnumerator OpenCloseSequence()
        {
            _isAnimating = true;
            int lastFrame = _frames.Length - 1;

            // Open: frame 0 → last frame
            float frameDuration = 1f / _openSpeed;
            for (int i = 0; i <= lastFrame; i++)
            {
                _sr.sprite = _frames[i];
                yield return new WaitForSeconds(frameDuration);
            }

            // Hold open
            yield return new WaitForSeconds(_holdDuration);

            // Close: last frame → frame 0
            frameDuration = 1f / _closeSpeed;
            for (int i = lastFrame; i >= 0; i--)
            {
                _sr.sprite = _frames[i];
                yield return new WaitForSeconds(frameDuration);
            }

            _isAnimating = false;
        }
    }
}
