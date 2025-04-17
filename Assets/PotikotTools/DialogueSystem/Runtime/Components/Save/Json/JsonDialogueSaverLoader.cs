using System.Collections.Generic;
using System.IO;
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
        private const string GraphFilename = "runtime.json";
        
        private static JsonSerializer _serializer;

        public JsonDialogueSaverLoader()
        {
            _serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            };

            _serializer.Converters.Add(new ConnectionDataConverter());
        }

        public bool Save(string directoryPath, DialogueData dialogueData)
        {
            string fullPath = Path.Combine(directoryPath, dialogueData.Id, GraphFilename);
            
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            using (StreamWriter sw = new(fullPath))
                using (JsonWriter jw = new JsonTextWriter(sw))
                    _serializer.Serialize(jw, dialogueData);
            
            return true;
        }

        public async Task<bool> SaveAsync(string directoryPath, DialogueData dialogueData)
        {
            await dialogueData.SerializeJsonAsync(_serializer);
            
            string fullPath = Path.Combine(directoryPath, dialogueData.Id, GraphFilename);

            using (StreamWriter sw = new(fullPath))
                using (JsonWriter jw = new JsonTextWriter(sw))
                    _serializer.Serialize(jw, dialogueData);
            
            return true;
        }

        public DialogueData Load(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, GraphFilename);

            if (!File.Exists(fullPath))
            {
                DL.LogError("Can't find runtime.json");
                return null;
            }

            using (StreamReader sr = new(fullPath))
                using (JsonReader jr = new JsonTextReader(sr))
                    return _serializer.Deserialize<DialogueData>(jr);
        }

        public async Task<DialogueData> LoadAsync(string directory, string dialogueId)
        {
            string fullPath = Path.Combine(directory, dialogueId, GraphFilename);

            using StreamReader sr = new(fullPath);
            string json = await sr.ReadToEndAsync();
            
            return await json.DeserializeJsonAsync<DialogueData>(_serializer);
        }
    }
}