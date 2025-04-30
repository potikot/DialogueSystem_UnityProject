using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Extensions.Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class JsonDialogueSaverLoader : IDialogueSaver, IDialogueLoader
    {
        protected JsonSerializer serializer;

        public JsonDialogueSaverLoader()
        {
            // TODO: initialize with preferences
            
            serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            };

            serializer.Converters.Add(new ConnectionDataConverter());
            serializer.Converters.Add(new Vector2Converter());
        }

        public bool Save(string directoryPath, DialogueData dialogueData)
        {
            string fullPath = Path.Combine(directoryPath, dialogueData.Id, DialogueSystemPreferences.Data.RuntimeDataFilename);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            using (StreamWriter sw = new(fullPath))
                using (JsonWriter jw = new JsonTextWriter(sw))
                    serializer.Serialize(jw, dialogueData);
            
            return true;
        }

        public async Task<bool> SaveAsync(string directoryPath, DialogueData dialogueData)
        {
            string fullPath = Path.Combine(directoryPath, dialogueData.Id, DialogueSystemPreferences.Data.RuntimeDataFilename);

            using (StreamWriter sw = new(fullPath))
            {
                string json = await dialogueData.SerializeJsonAsync(serializer);
                await sw.WriteAsync(json);
            }
            
            return true;
        }

        public DialogueData Load(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, DialogueSystemPreferences.Data.RuntimeDataFilename);

            if (!File.Exists(fullPath))
            {
                DL.LogError($"Can't find {DialogueSystemPreferences.Data.RuntimeDataFilename}");
                return null;
            }

            using (StreamReader sr = new(fullPath))
                using (JsonReader jr = new JsonTextReader(sr))
                    return serializer.Deserialize<DialogueData>(jr);
        }

        public async Task<DialogueData> LoadAsync(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, DialogueSystemPreferences.Data.RuntimeDataFilename);

            using (StreamReader sr = new(fullPath))
            {
                string json = await sr.ReadToEndAsync();
                return await json.DeserializeJsonAsync<DialogueData>(serializer);
            }
        }
        
        public List<string> LoadTags(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, DialogueSystemPreferences.Data.RuntimeDataFilename);

            using (StreamReader sr = new(fullPath))
            {
                string json = sr.ReadToEnd();
            
                JObject jObject = JObject.Parse(json);
                return jObject["Tags"].ToObject<List<string>>();
            }
        }
        
        public async Task<List<string>> LoadTagsAsync(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, DialogueSystemPreferences.Data.RuntimeDataFilename);

            using (StreamReader sr = new(fullPath))
            {
                string json = await sr.ReadToEndAsync();
                
                JObject jObject = JObject.Parse(json);
                return jObject["Tags"].ToObject<List<string>>();
            }
        }
    }
}