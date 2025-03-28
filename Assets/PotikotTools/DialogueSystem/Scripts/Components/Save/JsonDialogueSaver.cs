using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class JsonDialogueSaver : IDialogueSaver
    {
        private const string Extension = ".json";
        
        private static readonly string DirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "PotikotTools/DialogueSystem/Database");

        static JsonDialogueSaver()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new AssetReferenceConverter()
                }
            };
        }
        
        public void Save(DialogueData dialogueData)
        {
            string fullPath = Path.Combine(DirectoryPath, dialogueData.Id + Extension);
            
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
            
            string json = JsonConvert.SerializeObject(dialogueData, Formatting.Indented);
            File.WriteAllText(fullPath, json);
        }

        public DialogueData Load(string dialogueId)
        {
            string fullPath = Path.Combine(DirectoryPath, dialogueId + Extension);

            if (!File.Exists(fullPath))
                return null;
            
            string json = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<DialogueData>(json);
        }
    }
}