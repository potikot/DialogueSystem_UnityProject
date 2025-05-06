using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public static class DialogueSystemPreferences
    {
        public static readonly Vector2 InitialDialogueEditorWindowSize = new(700f, 350f);
        public static readonly Vector2 InitialDialogueEditorWindowPosition = new
        (
            1920f / 2f - InitialDialogueEditorWindowSize.x / 2f,
            1080f / 2f - InitialDialogueEditorWindowSize.y / 2f
        );
        
        private static DialogueSystemPreferencesSO _preferencesSO;
        
        public static DialogueSystemPreferencesSO Data => _preferencesSO ??= ScriptableObject.CreateInstance<DialogueSystemPreferencesSO>();
    }
}