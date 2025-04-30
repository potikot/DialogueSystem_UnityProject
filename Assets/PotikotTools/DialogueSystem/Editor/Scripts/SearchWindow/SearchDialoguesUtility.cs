using System.Collections.Generic;

namespace PotikotTools.DialogueSystem.Editor
{
    public static class SearchDialoguesUtility
    {
        public static List<DialogueData> SearchDialoguesByName(string dialogueName)
        {
            var foundDialogues = new List<DialogueData>();
            
            foreach (var dialogue in Components.Database.Dialogues)
            {
                if (dialogue.Key.StartsWith(dialogueName))
                {
                    DL.Log("Found Dialogue: " + dialogueName + " : " + dialogue.Value.Id);
                    foundDialogues.Add(dialogue.Value);
                }
            }
            
            return foundDialogues;
        }
        
        public static List<DialogueData> SearchDialoguesByTag(string tag)
        {
            DL.Log("Searching Dialogue: " + tag);
            var foundDialogues = new List<DialogueData>();
            
            foreach (var tagDialogues in Components.Database.Tags)
            {
                if (tagDialogues.Key.StartsWith(tag))
                    foreach (string dialogueName in tagDialogues.Value)
                        foundDialogues.Add(Components.Database.GetDialogue(dialogueName));
            }
            
            return foundDialogues;
        }
    }
}