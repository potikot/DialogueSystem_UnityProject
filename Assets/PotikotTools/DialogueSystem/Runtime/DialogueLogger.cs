using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public static class DL
    {
        public static void Log(object message)
        {
            Debug.Log($"[DialogueSystem] {message}");
        }

        public static void LogWarning(object message)
        {
            Debug.LogWarning($"[DialogueSystem] {message}");
        }

        public static void LogError(object message)
        {
            Debug.LogError($"[DialogueSystem] {message}");
        }
    }
}