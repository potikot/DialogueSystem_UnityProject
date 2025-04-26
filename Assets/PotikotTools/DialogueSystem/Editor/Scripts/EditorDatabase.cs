using System.Threading.Tasks;

namespace PotikotTools.DialogueSystem.Editor
{
    public static class EditorDatabase
    {
        private static IEditorDialogueSaver _saver;
        
        private static Database database => Components.Database;

        static EditorDatabase()
        {
            _saver = new EditorJsonDialogueSaverLoader();
        }
        
        public static async Task<bool> SaveDialogueAsync(EditorDialogueData editorData)
        {
            bool editorDataSaved = await _saver.SaveEditorDataAsync(database.RootPath, editorData);
            bool runtimeDataSaved = await _saver.SaveAsync(database.RootPath, editorData.RuntimeData);
            
            return editorDataSaved && runtimeDataSaved;
        }

        public static bool SaveDialogue(EditorDialogueData editorData)
        {
            bool editorDataSaved = _saver.SaveEditorData(database.RootPath, editorData);
            bool runtimeDataSaved = _saver.Save(database.RootPath, editorData.RuntimeData);
            
            return editorDataSaved && runtimeDataSaved;
        }

        public static async Task<EditorDialogueData> LoadDialogueAsync(string dialogueId)
        {
            EditorDialogueData editorData = await _saver.LoadEditorDataAsync(database.RootPath, dialogueId);
            editorData.RuntimeData = await database.GetDialogueAsync(dialogueId);
            editorData.GenerateEditorNodeDatas();
            
            return editorData;
        }
        
        public static EditorDialogueData LoadDialogue(string dialogueId)
        {
            EditorDialogueData editorData = _saver.LoadEditorData(database.RootPath, dialogueId);
            editorData.RuntimeData = database.GetDialogue(dialogueId);
            editorData.GenerateEditorNodeDatas();
            
            return editorData;
        }
    }
}