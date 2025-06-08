using System.Threading.Tasks;
using UnityEngine;

namespace PotikotTools.UniTalks
{
    public static class UniTalksAPI
    {
        public static async Task<DialogueController> StartDialogueAsync(string name, IDialogueView view) =>
            StartDialogue(await GetDialogueAsync(name), view);

        public static DialogueController StartDialogue(string name, IDialogueView view) =>
            StartDialogue(GetDialogue(name), view);

        [Command]
        public static DialogueController StartDialogue(string name)
        {
            var view = Object.FindObjectOfType<DialogueView>();
            return view == null ? null : StartDialogue(GetDialogue("name"), view);
        }

        
        public static DialogueController StartDialogue(DialogueData data, IDialogueView view)
        {
            DialogueController controller = new();
            controller.Initialize(data, view);
            controller.StartDialogue();
            return controller;
        }
        
        public static Task<DialogueData> GetDialogueAsync(string id) => DialoguesComponents.Database.GetDialogueAsync(id);
        public static Task<bool> LoadDialogueAsync(string id) => DialoguesComponents.Database.LoadDialogueAsync(id);
        public static Task<bool> LoadDialogueGroupAsync(string tag) => DialoguesComponents.Database.LoadDialoguesByTagAsync(tag);
        
        public static DialogueData GetDialogue(string id) => DialoguesComponents.Database.GetDialogue(id);
        public static bool LoadDialogue(string id) => DialoguesComponents.Database.LoadDialogue(id);
        public static bool LoadDialogueGroup(string tag) => DialoguesComponents.Database.LoadDialoguesByTag(tag);
    }
}