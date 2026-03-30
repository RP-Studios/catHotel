using UnityEngine;
using CatHotel.UI;

namespace CatHotel.Audio
{
    /// <summary>
    /// Plays background music and responds to the music volume slider.
    /// Attach to a GameObject with an AudioSource (loop enabled, clip assigned).
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.loop = true;
            _source.playOnAwake = true;
            _source.volume = ParametersPanel.MusicVolume;
        }

        private void OnEnable()
        {
            ParametersPanel.OnMusicVolumeChanged += SetVolume;
        }

        private void OnDisable()
        {
            ParametersPanel.OnMusicVolumeChanged -= SetVolume;
        }

        private void SetVolume(float vol)
        {
            if (_source != null)
                _source.volume = vol;
        }
    }
}
