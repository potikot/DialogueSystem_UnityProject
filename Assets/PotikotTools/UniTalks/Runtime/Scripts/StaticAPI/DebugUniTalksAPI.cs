using UnityEngine;

namespace PotikotTools.UniTalks
{
    public static partial class UniTalksAPI
    {
        [Command(nameof(Log), false)]
        public static void Log(object message)
        {
            Debug.Log($"[DialogueSystem] {message}");
        }

        [Command(nameof(LogWarning), false)]
        public static void LogWarning(object message)
        {
            Debug.LogWarning($"[DialogueSystem] {message}");
        }

        [Command(nameof(LogError), false)]
        public static void LogError(object message)
        {
            Debug.LogError($"[DialogueSystem] {message}");
        }
    }
}