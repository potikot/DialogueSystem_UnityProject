using UnityEngine;

namespace PotikotTools.UniTalks
{
    public interface IAudioHandler
    {
        void Play(AudioClip clip);
        void Play(AudioClip clip, float volume);
    }

}
