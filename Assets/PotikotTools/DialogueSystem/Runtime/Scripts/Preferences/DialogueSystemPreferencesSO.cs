using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class DialogueSystemPreferencesSO : ScriptableObject
    {
        public string DatabaseDirectory;
        public string AudioDirectory;

        public string RuntimeDataFilename;
        public string EditorDataFilename; // TODO: extract to editor preferences
        
        public SpeakerData[] Speakers;

        public DialogueSystemPreferencesSO()
        {
            DatabaseDirectory = "Resources/Dialogue System/Database";
            AudioDirectory = DatabaseDirectory + "/Audio";
            
            RuntimeDataFilename = "runtime.json";
            EditorDataFilename = "editor.json";
        }

        public void Reset()
        {
            DatabaseDirectory = "Resources/Dialogue System/Database";
            AudioDirectory = DatabaseDirectory + "/Audio";
            
            RuntimeDataFilename = "runtime.json";
            EditorDataFilename = "editor.json";
        }

        public void CopyFrom(DialogueSystemPreferencesSO source)
        {
            DatabaseDirectory = source.DatabaseDirectory;
            AudioDirectory = source.AudioDirectory;
            
            RuntimeDataFilename = source.RuntimeDataFilename;
            EditorDataFilename = source.EditorDataFilename;
        }

        public DialogueSystemPreferencesSO CreateCopy()
        {
            return Instantiate(this);
        }
    }
}