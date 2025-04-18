using System.IO;
using System.Threading.Tasks;
using Extensions.Newtonsoft.Json;
using Newtonsoft.Json;

namespace PotikotTools.DialogueSystem
{
    public class EditorJsonDialogueSaverLoader : JsonDialogueSaverLoader, IEditorDialogueSaver
    {
        protected const string EditorGraphFilename = "editor.json";
        
        public async Task<bool> SaveEditorDataAsync(string directoryPath, EditorDialogueData editorDialogueData)
        {
            await editorDialogueData.SerializeJsonAsync(serializer);
            
            string fullPath = Path.Combine(directoryPath, editorDialogueData.Data.Id, EditorGraphFilename);

            using (StreamWriter sw = new(fullPath))
            using (JsonWriter jw = new JsonTextWriter(sw))
                serializer.Serialize(jw, editorDialogueData);
            
            return true;
        }

        public bool SaveEditorData(string directoryPath, EditorDialogueData editorDialogueData)
        {
            string fullPath = Path.Combine(directoryPath, editorDialogueData.Data.Id, EditorGraphFilename);
            
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            using (StreamWriter sw = new(fullPath))
            using (JsonWriter jw = new JsonTextWriter(sw))
                serializer.Serialize(jw, editorDialogueData);
            
            return true;
        }

        public async Task<EditorDialogueData> LoadEditorDataAsync(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, EditorGraphFilename);

            using StreamReader sr = new(fullPath);
            string json = await sr.ReadToEndAsync();
            
            return await json.DeserializeJsonAsync<EditorDialogueData>(serializer);
        }

        public EditorDialogueData LoadEditorData(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, EditorGraphFilename);

            if (!File.Exists(fullPath))
            {
                DL.LogError($"Can't find {EditorGraphFilename}");
                return null;
            }

            using (StreamReader sr = new(fullPath))
            using (JsonReader jr = new JsonTextReader(sr))
                return serializer.Deserialize<EditorDialogueData>(jr);
        }
    }
}