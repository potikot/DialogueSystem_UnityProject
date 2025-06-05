using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PotikotTools.UniTalks.Editor
{
    public class JsonEditorDialoguePersistence : IEditorDialoguePersistence
    {
        protected readonly JsonSerializerSettings serializerSettings;
        
        public JsonEditorDialoguePersistence()
        {
            // TODO: initialize with preferences

            serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            };
            
            serializerSettings.Converters.Add(new Vector2Converter());
            serializerSettings.Converters.Add(new Vector3Converter());
            serializerSettings.Converters.Add(new ConnectionDataConverter());
            // serializerSettings.Converters.Add(new NodeDataConverter());
        }
        
        public async Task<bool> SaveAsync(string directoryPath, EditorDialogueData editorDialogueData, bool refreshAsset = true)
        {
            string fullPath = Path.Combine(directoryPath, editorDialogueData.Name, UniTalksPreferences.Data.EditorDataFilename);
    
            string json = JsonConvert.SerializeObject(editorDialogueData, serializerSettings);
            return await FileUtility.WriteAsync(fullPath, json, refreshAsset);
        }
        
        public bool Save(string directoryPath, EditorDialogueData editorDialogueData, bool refreshAsset = true)
        {
            string fullPath = Path.Combine(directoryPath, editorDialogueData.Name, UniTalksPreferences.Data.EditorDataFilename);
            
            string json = JsonConvert.SerializeObject(editorDialogueData, serializerSettings);
            return FileUtility.Write(fullPath, json, refreshAsset);
        }

        public async Task<EditorDialogueData> LoadAsync(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, UniTalksPreferences.Data.EditorDataFilename);

            string json = await FileUtility.ReadAsync(fullPath);
                        
            if (json == null)
                return null;
            
            return JsonConvert.DeserializeObject<EditorDialogueData>(json, serializerSettings);
        }

        public EditorDialogueData Load(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, UniTalksPreferences.Data.EditorDataFilename);

            string json = FileUtility.Read(fullPath);
            
            if (json == null)
                return null;
            
            return JsonConvert.DeserializeObject<EditorDialogueData>(json, serializerSettings);
        }
    }
}