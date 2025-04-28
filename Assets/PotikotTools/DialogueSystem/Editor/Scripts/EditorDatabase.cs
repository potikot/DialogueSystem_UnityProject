using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;

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
            
            AssetDatabase.Refresh();
            
            return editorDataSaved && runtimeDataSaved;
        }

        public static bool SaveDialogue(EditorDialogueData editorData)
        {
            bool editorDataSaved = _saver.SaveEditorData(database.RootPath, editorData);
            bool runtimeDataSaved = _saver.Save(database.RootPath, editorData.RuntimeData);
            
            AssetDatabase.Refresh();

            return editorDataSaved && runtimeDataSaved;
        }

        public static async Task<List<EditorDialogueData>> LoadAllDialoguesAsync()
        {
            string[] dialogueDirectories = Directory.GetDirectories(database.RootPath);
            List<EditorDialogueData> dialogues = new(dialogueDirectories.Length);
            
            foreach (string dialogueDirectory in dialogueDirectories)
                dialogues.Add(await LoadDialogueAsync(Path.GetFileName(dialogueDirectory)));
            
            return dialogues;
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

        public static void CreateDialogue(string dialogueId)
        {
            string guid = AssetDatabase.CreateFolder(database.RelativeRootPath, dialogueId);
            
            if (string.IsNullOrEmpty(guid))
                return; // TODO: show message
            
            var runtimeData = new DialogueData(dialogueId);
            var editorData = new EditorDialogueData(runtimeData);

            if (!editorData.RuntimeData.TrySetId(Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid))))
            {
                // TODO: set unique id
            }
            
            SaveDialogue(editorData);

            AssetDatabase.Refresh();
        }
        
        public static void DeleteDialogue(EditorDialogueData editorData) => DeleteDialogue(editorData.Id);
        
        public static void DeleteDialogue(string dialogueId)
        {
            AssetDatabase.DeleteAsset(Path.Combine(database.RelativeRootPath, dialogueId));
        }
    }
}