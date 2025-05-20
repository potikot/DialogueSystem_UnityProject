using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PotikotTools.DialogueSystem.Editor
{
    public class EditorJsonDialogueLoader : JsonDialogueLoader, IEditorDialogueSaverLoader, IDialogueSaver
    {
        public bool SaveData(string directoryPath, DialogueData dialogueData, bool refreshAsset = true)
        {
            string fullPath = Path.Combine(directoryPath, dialogueData.Name, DialogueSystemPreferences.Data.RuntimeDataFilename);
            
            string json = JsonConvert.SerializeObject(dialogueData, serializerSettings);
            return FileUtility.Write(fullPath, json, refreshAsset);
        }

        public async Task<bool> SaveDataAsync(string directoryPath, DialogueData dialogueData, bool refreshAsset = true)
        {
            string fullPath = Path.Combine(directoryPath, dialogueData.Name, DialogueSystemPreferences.Data.RuntimeDataFilename);
    
            string json = JsonConvert.SerializeObject(dialogueData, serializerSettings);
            return await FileUtility.WriteAsync(fullPath, json, refreshAsset);
        }
        
        public async Task<bool> SaveEditorDataAsync(string directoryPath, EditorDialogueData editorDialogueData, bool refreshAsset = true)
        {
            string fullPath = Path.Combine(directoryPath, editorDialogueData.Name, DialogueSystemPreferences.Data.EditorDataFilename);
    
            string json = JsonConvert.SerializeObject(editorDialogueData, serializerSettings);
            return await FileUtility.WriteAsync(fullPath, json, refreshAsset);
        }
        
        public bool SaveEditorData(string directoryPath, EditorDialogueData editorDialogueData, bool refreshAsset = true)
        {
            string fullPath = Path.Combine(directoryPath, editorDialogueData.Name, DialogueSystemPreferences.Data.EditorDataFilename);
            
            string json = JsonConvert.SerializeObject(editorDialogueData, serializerSettings);
            return FileUtility.Write(fullPath, json, refreshAsset);
        }

        public async Task<EditorDialogueData> LoadEditorDataAsync(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, DialogueSystemPreferences.Data.EditorDataFilename);

            string json = await FileUtility.ReadAsync(fullPath);
                        
            if (json == null)
                return null;
            
            return JsonConvert.DeserializeObject<EditorDialogueData>(json, serializerSettings);
        }

        public EditorDialogueData LoadEditorData(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, DialogueSystemPreferences.Data.EditorDataFilename);

            string json = FileUtility.Read(fullPath);
            
            if (json == null)
                return null;
            
            return JsonConvert.DeserializeObject<EditorDialogueData>(json, serializerSettings);
        }
    }
}