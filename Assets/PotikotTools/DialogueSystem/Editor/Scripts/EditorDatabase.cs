using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;

namespace PotikotTools.DialogueSystem.Editor
{
    public class EditorDatabase
    {
        private IEditorDialogueSaverLoader _saverLoader;
        
        private Database database => Components.Database;

        public EditorDatabase(IEditorDialogueSaverLoader editorSaverLoader)
        {
            _saverLoader = editorSaverLoader;
        }
        
        // TODO: Possibility to Save and Load only required things
        
        public async Task<bool> SaveDialogueAsync(EditorDialogueData editorData)
        {
            bool editorDataSaved = await _saverLoader.SaveEditorDataAsync(database.RootPath, editorData);
            bool runtimeDataSaved = await _saverLoader.SaveDataAsync(database.RootPath, editorData.RuntimeData);
            
            return editorDataSaved && runtimeDataSaved;
        }

        public bool SaveDialogue(EditorDialogueData editorData)
        {
            bool editorDataSaved = _saverLoader.SaveEditorData(database.RootPath, editorData);
            bool runtimeDataSaved = _saverLoader.SaveData(database.RootPath, editorData.RuntimeData);
            
            return editorDataSaved && runtimeDataSaved;
        }

        public async Task<List<EditorDialogueData>> LoadAllDialoguesAsync()
        {
            string[] dialogueDirectories = Directory.GetDirectories(database.RootPath);
            List<EditorDialogueData> dialogues = new(dialogueDirectories.Length);
            
            foreach (string dialogueDirectory in dialogueDirectories)
                dialogues.Add(await LoadDialogueAsync(Path.GetFileName(dialogueDirectory)));
            
            return dialogues;
        }
        
        public async Task<EditorDialogueData> LoadDialogueAsync(string dialogueId)
        {
            EditorDialogueData editorData = await _saverLoader.LoadEditorDataAsync(database.RootPath, dialogueId);
            editorData.RuntimeData = await database.GetDialogueAsync(dialogueId);
            editorData.GenerateEditorNodeDatas();
            
            return editorData;
        }
        
        public EditorDialogueData LoadDialogue(string dialogueId)
        {
            EditorDialogueData editorData = _saverLoader.LoadEditorData(database.RootPath, dialogueId);
            editorData.RuntimeData = database.GetDialogue(dialogueId);
            editorData.GenerateEditorNodeDatas();

            return editorData;
        }

        public async Task<EditorDialogueData> CreateDialogue(string dialogueId)
        {
            string guid = AssetDatabase.CreateFolder(database.RelativeRootPath, dialogueId);

            if (string.IsNullOrEmpty(guid))
            {
                DL.LogError($"Cannot create folder for dialogue with id: {dialogueId}. Relative path: {database.RelativeRootPath}");
                return null;
            }

            string uniqueDialogueId = Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid));
            if (string.IsNullOrEmpty(uniqueDialogueId))
            {
                DL.LogError($"Cannot create folder for dialogue with id: \"{dialogueId}\". Relative path: \"{database.RelativeRootPath}\"");
                return null;
            }
            
            var runtimeData = new DialogueData(uniqueDialogueId);
            EditorDialogueData editorData = new EditorDialogueData(runtimeData);

            DL.LogWarning(editorData.Id);
            
            await SaveDialogueAsync(editorData);
            database.AddDialogue(runtimeData);
            
            return editorData;
        }

        public void DeleteDialogue(EditorDialogueData editorData) => DeleteDialogue(editorData.Id);
        
        public void DeleteDialogue(string dialogueId)
        {
            AssetDatabase.DeleteAsset(Path.Combine(database.RelativeRootPath, dialogueId));
        }
    }
}