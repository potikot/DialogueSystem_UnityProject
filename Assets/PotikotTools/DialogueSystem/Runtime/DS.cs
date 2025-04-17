using System;

namespace PotikotTools.DialogueSystem
{
    public static class DS
    {
        public static DialogueController StartDialogue(DialogueData dialogueData) => throw new NotImplementedException();
        
        public static DialogueData GetDialogue(string dialogueId) => Components.Database.GetDialogue(dialogueId);
        public static bool LoadDialogue(string dialogueId) => Components.Database.LoadDialogue(dialogueId);
        public static bool LoadDialogueGroup(string tag) => Components.Database.LoadDialogueGroup(tag);
    }
}