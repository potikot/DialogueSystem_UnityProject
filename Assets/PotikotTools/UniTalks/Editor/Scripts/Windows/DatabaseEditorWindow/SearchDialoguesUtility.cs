using System;
using System.Collections.Generic;

namespace PotikotTools.UniTalks.Editor
{
    public static class SearchDialoguesUtility
    {
        public static List<DialogueData> SearchDialoguesByName(string dialogueName)
        {
            var foundDialogues = new List<DialogueData>();
            
            foreach (var dialogue in DialoguesComponents.Database.Dialogues) // TODO: update dialogues when modified
            {
                if (dialogue.Key.StartsWith(dialogueName, StringComparison.OrdinalIgnoreCase))
                {
                    // DL.Log("Found Dialogue: " + dialogueName + " : " + dialogue.Value.Name);
                    foundDialogues.Add(dialogue.Value);
                }
            }
            
            return foundDialogues;
        }
        
        public static List<DialogueData> SearchDialoguesByTag(string tag)
        {
            // DL.Log("Searching Dialogue: " + tag);
            var foundDialogues = new List<DialogueData>();

            // foreach (var kvp in Components.Database.Dialogues)
            // {
            //     if (kvp.Value.Tags.Any(t => t.StartsWith(tag)))
            //         foundDialogues.Add(kvp.Value);
            // }
            
            foreach (var tagDialogues in DialoguesComponents.Database.Tags) // TODO: update tags when modified
            {
                if (tagDialogues.Key.StartsWith(tag))
                    foreach (string dialogueName in tagDialogues.Value)
                        foundDialogues.Add(DialoguesComponents.Database.GetDialogue(dialogueName));
            }
            
            return foundDialogues;
        }
    }
}