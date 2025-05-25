using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;

namespace PotikotTools.DialogueSystem.Editor
{
    public class EditorDatabase
    {
        protected IEditorDialogueSaverLoader saverLoader;
        protected Dictionary<string, EditorDialogueData> dialogues;

        protected Database database => Components.Database;
        
        public EditorDatabase(IEditorDialogueSaverLoader editorSaverLoader)
        {
            saverLoader = editorSaverLoader;
            dialogues = new Dictionary<string, EditorDialogueData>();
        }
        
        // TODO: Possibility to Save and Load only required things
        
        public async Task<bool> SaveDialogueAsync(EditorDialogueData editorData)
        {
            if (editorData == null)
                return false;
            
            bool editorDataSaved = await saverLoader.SaveEditorDataAsync(database.DialoguesRootPath, editorData);
            bool runtimeDataSaved = await saverLoader.SaveDataAsync(database.DialoguesRootPath, editorData.RuntimeData);
            
            return editorDataSaved && runtimeDataSaved;
        }

        public bool SaveDialogue(EditorDialogueData editorData)
        {
            if (editorData == null)
                return false;

            bool editorDataSaved = saverLoader.SaveEditorData(database.DialoguesRootPath, editorData);
            bool runtimeDataSaved = saverLoader.SaveData(database.DialoguesRootPath, editorData.RuntimeData);
            
            return editorDataSaved && runtimeDataSaved;
        }

        public async Task<List<EditorDialogueData>> LoadAllDialoguesAsync()
        {
            string[] dialogueDirectories = Directory.GetDirectories(database.DialoguesRootPath);
            List<EditorDialogueData> loadedDialogues = new(dialogueDirectories.Length);

            foreach (string dialogueDirectory in dialogueDirectories)
            {
                var editorData = await LoadDialogueAsync(Path.GetFileName(dialogueDirectory));
                if (editorData != null)
                    loadedDialogues.Add(editorData);
            }
            
            return loadedDialogues;
        }
        
        public List<EditorDialogueData> LoadAllDialogues()
        {
            string[] dialogueDirectories = Directory.GetDirectories(database.DialoguesRootPath);
            List<EditorDialogueData> loadedDialogues = new(dialogueDirectories.Length);

            foreach (string dialogueDirectory in dialogueDirectories)
            {
                var editorData = LoadDialogue(Path.GetFileName(dialogueDirectory));
                if (editorData != null)
                    loadedDialogues.Add(editorData);
            }
            
            return loadedDialogues;
        }
        
        public async Task<EditorDialogueData> LoadDialogueAsync(string dialogueName)
        {
            if (string.IsNullOrEmpty(dialogueName))
                return null;

            if (dialogues.TryGetValue(dialogueName, out var editorData))
                return editorData;
            
            editorData = await saverLoader.LoadEditorDataAsync(database.DialoguesRootPath, dialogueName);
            
            if (editorData == null)
                return null;
            
            editorData.RuntimeData = await database.GetDialogueAsync(dialogueName);
            editorData.GenerateEditorNodeDatas();

            if (dialogues.TryGetValue(dialogueName, out var tmp))
                return tmp;

            dialogues.Add(editorData.Name, editorData);

            return editorData;
        }

        public EditorDialogueData LoadDialogue(string dialogueName)
        {
            if (string.IsNullOrEmpty(dialogueName))
                return null;
            
            if (dialogues.TryGetValue(dialogueName, out var editorData))
                return editorData;
            
            editorData = saverLoader.LoadEditorData(database.DialoguesRootPath, dialogueName);

            if (editorData == null)
                return null;
            
            editorData.RuntimeData = database.GetDialogue(dialogueName);
            editorData.GenerateEditorNodeDatas();
            
            dialogues.Add(editorData.Name, editorData);

            return editorData;
        }

        public async Task<EditorDialogueData> CreateDialogue(string dialogueName)
        {
            if (string.IsNullOrEmpty(dialogueName))
                return null;

            string guid = AssetDatabase.CreateFolder(database.DialoguesRelativeRootPath, dialogueName);

            if (string.IsNullOrEmpty(guid))
            {
                DL.LogError($"Cannot create folder for dialogue with id: {dialogueName}. Relative path: {database.DialoguesRelativeRootPath}");
                return null;
            }

            string uniqueDialogueName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid));
            if (string.IsNullOrEmpty(uniqueDialogueName))
            {
                DL.LogError($"Cannot create folder for dialogue with id: \"{dialogueName}\". Relative path: \"{database.DialoguesRelativeRootPath}\"");
                return null;
            }
            
            var runtimeData = new DialogueData(uniqueDialogueName);
            EditorDialogueData editorData = new EditorDialogueData(runtimeData);
            dialogues.Add(editorData.Name, editorData);
            
            await SaveDialogueAsync(editorData);
            database.AddDialogue(runtimeData);

            return editorData;
        }

        public void DeleteDialogue(EditorDialogueData editorData)
        {
            if (editorData == null)
                return;
            
            DeleteDialogue(editorData.Name);
        }
        
        public void DeleteDialogue(string dialogueName)
        {
            if (string.IsNullOrEmpty(dialogueName))
                return;
            
            AssetDatabase.DeleteAsset(Path.Combine(database.DialoguesRelativeRootPath, dialogueName));
            
            if (dialogues.Remove(dialogueName, out var editorData))
                editorData.OnDelete();
        }
    }
}