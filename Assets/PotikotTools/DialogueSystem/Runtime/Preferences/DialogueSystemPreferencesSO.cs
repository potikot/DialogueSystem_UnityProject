using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class DialogueSystemPreferencesSO : ScriptableObject
    {
        public string DatabaseDirectory;
        public string AudioDirectory;

        public string RuntimeDataFilename;
        public string EditorDataFilename; // TODO: exctract to editor preferences
        
        public SpeakerData[] Speakers;

        public DialogueSystemPreferencesSO()
        {
            DatabaseDirectory = "Resources/Dialogue System/Database";
            AudioDirectory = DatabaseDirectory + "/Audio";
            
            RuntimeDataFilename = "runtime.json";
            EditorDataFilename = "editor.json";
        }
    }
}