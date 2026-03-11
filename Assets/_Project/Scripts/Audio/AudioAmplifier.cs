using UnityEngine;

namespace CatHotel.Audio
{
    /// <summary>
    /// Amplifies audio beyond Unity's AudioSource.volume cap of 1.0.
    /// Attach to the same GameObject as an AudioSource.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioAmplifier : MonoBehaviour
    {
        public float gain = 1f;

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (gain <= 1f) return;

            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= gain;
            }
        }
    }
}
