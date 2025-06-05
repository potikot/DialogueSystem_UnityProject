using UnityEditor;
using UnityEngine;

namespace PotikotTools.DialogueSystem.Editor
{
    [InitializeOnLoad]
    [DefaultExecutionOrder(-1000)]
    public static class EditorComponents
    {
        private static EditorDatabase _database;

        private static NodeEditorWindowsManager _nodeEditorWM;
        private static DialogueTestWindowsManager _dialogueTestWM;
        
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
        
        public static NodeEditorWindowsManager NodeEditorWM => _nodeEditorWM;
        public static DialogueTestWindowsManager DialogueTestWM => _dialogueTestWM;

        static EditorComponents()
        {
            IEditorDialoguePersistence editorPersistence = new JsonEditorDialoguePersistence();
            IDialoguePersistence runtimePersistence = new JsonDialoguePersistence();
            Database = new EditorDatabase(editorPersistence, runtimePersistence);

            EditorApplication.delayCall += () =>
            {
                _nodeEditorWM = new NodeEditorWindowsManager();
                _dialogueTestWM = new DialogueTestWindowsManager();
            };
        }
    }
}