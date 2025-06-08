using UnityEngine;

namespace PotikotTools.UniTalks
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioHandler : MonoBehaviour, IAudioHandler
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        public void Play(AudioClip clip)
        {
            Play(clip, 1f);
        }

        public void Play(AudioClip clip, float volumeScale)
        {
            if (clip == null)
                return;
            
            _audioSource.PlayOneShot(clip, volumeScale);
        }
    }
}