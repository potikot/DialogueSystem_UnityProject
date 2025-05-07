using UnityEditor;
using UnityEngine;

namespace PotikotTools.DialogueSystem.Editor
{
    [InitializeOnLoad]
    [DefaultExecutionOrder(-1000)]
    public static class EditorComponents
    {
        private static EditorDatabase _database;

        public static EditorDatabase Database
        {
            get => _database;
            set
            {
                if (value == null)
                    return;

                _database = value;
            }
        }
        
        static EditorComponents()
        {
            IEditorDialogueSaverLoader saverLoader = new EditorJsonDialogueLoader();
            Database = new EditorDatabase(saverLoader);
        }
    }
}