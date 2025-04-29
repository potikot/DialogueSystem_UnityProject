using System;
using System.Threading.Tasks;

namespace PotikotTools.DialogueSystem
{
    public static class DialogueSystemAPI
    {
        public static DialogueController StartDialogue(string id, IDialogueView view) => StartDialogue(GetDialogue(id), view);
        
        public static DialogueController StartDialogue(DialogueData data, IDialogueView view)
        {
            DialogueController controller = new();
            controller.Initialize(data, view);
            controller.StartDialogue();
            return controller;
        }
        
        public static Task<DialogueData> GetDialogueAsync(string id) => Components.Database.GetDialogueAsync(id);
        public static Task<bool> LoadDialogueAsync(string id) => Components.Database.LoadDialogueAsync(id);
        public static Task<bool> LoadDialogueGroupAsync(string tag) => Components.Database.LoadDialoguesByTagAsync(tag);
        
        public static DialogueData GetDialogue(string id) => Components.Database.GetDialogue(id);
        public static bool LoadDialogue(string id) => Components.Database.LoadDialogue(id);
        public static bool LoadDialogueGroup(string tag) => Components.Database.LoadDialoguesByTag(tag);
    }
}