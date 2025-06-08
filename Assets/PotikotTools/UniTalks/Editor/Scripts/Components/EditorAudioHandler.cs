using System;
using System.Reflection;
using UnityEngine;

namespace PotikotTools.UniTalks.Editor
{
    public class EditorAudioHandler : IAudioHandler
    {
        private static MethodInfo _playMethod;
        // private static MethodInfo _stopAllMethod;
        private static MethodInfo _setVolumeMethod;

        static EditorAudioHandler()
        {
            var bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var audioUtilType = Type.GetType("UnityEditor.AudioUtil,UnityEditor");
            
            _playMethod = audioUtilType.GetMethod(
                "PlayClip", bindingFlags,
                null, new[] { typeof(AudioClip), typeof(int), typeof(bool) }, null);
            // _stopAllMethod = audioUtilType.GetMethod("StopAllClips", bindingFlags);
            _setVolumeMethod = audioUtilType.GetMethod("SetClipVolume", bindingFlags);
        }
        
        public void Play(AudioClip clip)
        {
            Play(clip, 1f);
        }

        public void Play(AudioClip clip, float volume)
        {
            if (clip == null)
                return;

            // _stopAllMethod?.Invoke(null, null);
            _playMethod?.Invoke(null, new object[] { clip, 0, false });
            _setVolumeMethod?.Invoke(null, new object[] { clip, Mathf.Clamp01(volume) });
        }
    }
}