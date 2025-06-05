using UnityEngine;

namespace PotikotTools.UniTalks
{
    public static class DL
    {
        [Command]
        public static void Log(object message)
        {
            Debug.Log($"[DialogueSystem] {message}");
        }
        
        [Command]
        public static void LogWarning(object message)
        {
            Debug.LogWarning($"[DialogueSystem] {message}");
        }

        [Command]
        public static void LogError(object message)
        {
            Debug.LogError($"[DialogueSystem] {message}");
        }
    }
}