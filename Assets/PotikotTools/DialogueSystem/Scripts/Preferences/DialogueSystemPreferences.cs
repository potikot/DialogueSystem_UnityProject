using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class DialogueSystemPreferencesSO : ScriptableObject
    {
        public string DatabaseDirectory;
        public string DialoguesDirectory;
        public string AudioDirectory;
        
        public SpeakerData[] Speakers;

        public DialogueSystemPreferencesSO()
        {
            DatabaseDirectory = "Dialogue System/Database";
            DialoguesDirectory = DatabaseDirectory + "/Dialogues";
            AudioDirectory = DatabaseDirectory + "/Audio";
        }
    }
    
    public static class DialogueSystemPreferences
    {
        private static DialogueSystemPreferencesSO _preferencesSO;
        
        public static DialogueSystemPreferencesSO Preferences => _preferencesSO ??= ScriptableObject.CreateInstance<DialogueSystemPreferencesSO>();
    }
}