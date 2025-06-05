using UnityEngine;

namespace PotikotTools.UniTalks
{
    public class UniTalksPreferencesSO : ScriptableObject
    {
        public string DatabaseDirectory = "Resources/UniTalks/Database";

        public string RuntimeDataFilename = "runtime.json";
        public string EditorDataFilename = "editor.json"; // TODO: extract to editor preferences
        
        public SpeakerData[] Speakers;

        public void Reset()
        {
            DatabaseDirectory = "Resources/UniTalks/Database";
            
            RuntimeDataFilename = "runtime.json";
            EditorDataFilename = "editor.json";
        }

        public void CopyFrom(UniTalksPreferencesSO source)
        {
            DatabaseDirectory = source.DatabaseDirectory;
            
            RuntimeDataFilename = source.RuntimeDataFilename;
            EditorDataFilename = source.EditorDataFilename;
        }

        public UniTalksPreferencesSO CreateCopy()
        {
            return Instantiate(this);
        }
    }
}