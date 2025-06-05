using UnityEditor;
using UnityEngine;

namespace PotikotTools.UniTalks.Editor
{
    [InitializeOnLoad]
    [DefaultExecutionOrder(-1000)]
    public static class EditorUniTalksComponents
    {
        private static EditorDialogueDatabase _database;

        private static DialogueEditorWindowsManager _dialogueEditorWM;
        private static DialoguePreviewWindowsManager _dialoguePreviewWM;
        
        public static EditorDialogueDatabase Database
        {
            get => _database;
            set
            {
                if (value == null)
                    return;

                _database = value;
            }
        }
        
        public static DialogueEditorWindowsManager DialogueEditorWM => _dialogueEditorWM;
        public static DialoguePreviewWindowsManager DialoguePreviewWM => _dialoguePreviewWM;

        static EditorUniTalksComponents()
        {
            IEditorDialoguePersistence editorPersistence = new MessagePackEditorDialoguePersistence();
            IDialoguePersistence runtimePersistence = new MessagePackDialoguePersistence();
            Database = new EditorDialogueDatabase(editorPersistence, runtimePersistence);

            EditorApplication.delayCall += () =>
            {
                _dialogueEditorWM = new DialogueEditorWindowsManager();
                _dialoguePreviewWM = new DialoguePreviewWindowsManager();
            };
        }
    }
}