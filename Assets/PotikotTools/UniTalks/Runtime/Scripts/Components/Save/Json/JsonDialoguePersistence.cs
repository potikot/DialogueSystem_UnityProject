using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PotikotTools.UniTalks
{
    public class JsonDialoguePersistence : IDialoguePersistence
    {
        protected readonly JsonSerializerSettings serializerSettings;
        
        public JsonDialoguePersistence()
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

        public bool Save(string directoryPath, DialogueData dialogueData, bool refreshAsset = true)
        {
            string fullPath = Path.Combine(directoryPath, dialogueData.Name, UniTalksPreferences.Data.RuntimeDataFilename);
            
            string json = JsonConvert.SerializeObject(dialogueData, serializerSettings);
            return FileUtility.Write(fullPath, json, refreshAsset);
        }

        public async Task<bool> SaveAsync(string directoryPath, DialogueData dialogueData, bool refreshAsset = true)
        {
            string fullPath = Path.Combine(directoryPath, dialogueData.Name, UniTalksPreferences.Data.RuntimeDataFilename);
    
            string json = JsonConvert.SerializeObject(dialogueData, serializerSettings);
            return await FileUtility.WriteAsync(fullPath, json, refreshAsset);
        }
        
        public DialogueData Load(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, UniTalksPreferences.Data.RuntimeDataFilename);

            string json = FileUtility.Read(fullPath);
            
            if (json == null)
                return null;
            
            return JsonConvert.DeserializeObject<DialogueData>(json, serializerSettings);
        }

        public async Task<DialogueData> LoadAsync(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, UniTalksPreferences.Data.RuntimeDataFilename);
            
            string json = await FileUtility.ReadAsync(fullPath);
                        
            if (json == null)
                return null;

            return JsonConvert.DeserializeObject<DialogueData>(json, serializerSettings);
        }
        
        public List<string> LoadTags(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, UniTalksPreferences.Data.RuntimeDataFilename);

            string json = FileUtility.Read(fullPath);
            
            JObject jObject = JObject.Parse(json);
            return jObject["Tags"]?.ToObject<List<string>>();
        }
        
        public async Task<List<string>> LoadTagsAsync(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, UniTalksPreferences.Data.RuntimeDataFilename);

            string json = await FileUtility.ReadAsync(fullPath);
            
            JObject jObject = JObject.Parse(json);
            return jObject["Tags"]?.ToObject<List<string>>();
        }
    }
}