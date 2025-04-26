using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public static class DialogueSystemPreferences
    {
        private static DialogueSystemPreferencesSO _preferencesSO;
        
        public static DialogueSystemPreferencesSO Data => _preferencesSO ??= ScriptableObject.CreateInstance<DialogueSystemPreferencesSO>();
    }
}