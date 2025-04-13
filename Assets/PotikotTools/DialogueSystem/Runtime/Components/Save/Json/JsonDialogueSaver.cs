using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class JsonDialogueSaver : IDialogueSaver
    {
        private const string Extension = ".json";
        
        private static JsonSerializer _serializer;

        public JsonDialogueSaver()
        {
            _serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            _serializer.Converters.Add(new ConnectionDataConverter());
        }

        public void Save(string directoryPath, DialogueData dialogueData)
        {
            string fullPath = Path.Combine(directoryPath, dialogueData.Id + Extension);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            using (StreamWriter sw = new(fullPath))
                using (JsonWriter jw = new JsonTextWriter(sw))
                    _serializer.Serialize(jw, dialogueData);
        }

        public DialogueData Load(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId + Extension);

            if (!File.Exists(fullPath))
                return null;
            
            using (StreamReader sr = new(fullPath))
                using (JsonReader jr = new JsonTextReader(sr))
                    return _serializer.Deserialize<DialogueData>(jr);
        }
    }
}