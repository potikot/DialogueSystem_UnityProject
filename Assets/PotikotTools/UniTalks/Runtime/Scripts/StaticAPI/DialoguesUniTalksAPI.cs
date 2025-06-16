using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace PotikotTools.UniTalks
{
    public static partial class UniTalksAPI
    {
        public static async Task<DialogueController> StartDialogueAsync(string name, IDialogueView view) => await StartDialogueAsync<DialogueController>(name, view);
        public static async Task<T> StartDialogueAsync<T>(string name, IDialogueView view, params object[] args) where T : DialogueController =>
            StartDialogue<T>(await GetDialogueAsync(name), view, args);

        public static DialogueController StartDialogue(string name, IDialogueView view) => StartDialogue<DialogueController>(name, view);
        public static T StartDialogue<T>(string name, IDialogueView view, params object[] args) where T : DialogueController =>
            StartDialogue<T>(GetDialogue(name), view, args);

        public static DialogueController StartDialogue(string name) => StartDialogue<DialogueController>(name);
        public static T StartDialogue<T>(string name, params object[] args) where T : DialogueController
        {
            var view = Object.FindObjectOfType<DialogueView>();
            return view == null ? null : StartDialogue<T>(GetDialogue(name), view, args);
        }

        public static DialogueController StartDialogue(DialogueData data, IDialogueView view) => StartDialogue<DialogueController>(data, view);
        public static T StartDialogue<T>(DialogueData data, IDialogueView view, params object[] args) where T : DialogueController
        {
            if (data == null)
                return null;
            
            var controller = (T)Activator.CreateInstance(typeof(T), args);
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