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

        public void Save(DialogueData dialogueData)
        {
            string fullPath = Path.Combine(DirectoryPath, dialogueData.Id + Extension);
            
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
            
            string json = JsonConvert.SerializeObject(dialogueData, Formatting.Indented);
            DL.Log("Save:\n" + json);
            File.WriteAllText(fullPath, json);
        }

        public DialogueData Load(string dialogueId)
        {
            string fullPath = Path.Combine(DirectoryPath, dialogueId + Extension);

            if (!File.Exists(fullPath))
                return null;
            
            string json = File.ReadAllText(fullPath);
            DL.Log("Load:\n" + json);

            return JsonConvert.DeserializeObject<DialogueData>(json);
        }
    }
}