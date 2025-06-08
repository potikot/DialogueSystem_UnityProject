using UnityEditor;
using UnityEngine;

namespace PotikotTools.UniTalks.Editor
{
    [InitializeOnLoad]
    [DefaultExecutionOrder(-1000)]
    public static class EditorDialogueComponents
    {
        private static EditorDialogueDatabase _database;
        private static IAudioHandler _audioHandler;

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
        
        public static IAudioHandler AudioHandler
        {
            get => _audioHandler;
            set
            {
                if (value == null)
                    return;
                
                _audioHandler = value;
            }
        }
        
        public static DialogueEditorWindowsManager DialogueEditorWM => _dialogueEditorWM;
        public static DialoguePreviewWindowsManager DialoguePreviewWM => _dialoguePreviewWM;

        static EditorDialogueComponents()
        {
            IEditorDialoguePersistence editorPersistence = new JsonEditorDialoguePersistence();
            IDialoguePersistence runtimePersistence = new JsonDialoguePersistence();
            Database = new EditorDialogueDatabase(editorPersistence, runtimePersistence);

            EditorApplication.delayCall += () =>
            {
                _dialogueEditorWM = new DialogueEditorWindowsManager();
                _dialoguePreviewWM = new DialoguePreviewWindowsManager();
            };
        }
    }
}