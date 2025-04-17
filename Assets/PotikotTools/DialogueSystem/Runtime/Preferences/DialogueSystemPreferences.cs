using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class DialogueSystemPreferencesSO : ScriptableObject
    {
        public string DatabaseDirectory;
        public string AudioDirectory;
        
        public SpeakerData[] Speakers;

        public DialogueSystemPreferencesSO()
        {
            DatabaseDirectory = "Resources/Dialogue System/Database";
            AudioDirectory = DatabaseDirectory + "/Audio";
        }
    }
    
    public static class DialogueSystemPreferences
    {
        private static DialogueSystemPreferencesSO _preferencesSO;
        
        public static DialogueSystemPreferencesSO Preferences => _preferencesSO ??= ScriptableObject.CreateInstance<DialogueSystemPreferencesSO>();
    }
}