using System.IO;

namespace PotikotTools.DialogueSystem
{
    public static class Database
    {
        private static IDialogueSaver Saver = new JsonDialogueSaver();
        private static readonly string DirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", DialogueSystemPreferences.Preferences.DialoguesDirectory);

        public static bool SaveDialogue(DialogueData dialogue)
        {
            Saver.Save(DirectoryPath, dialogue);
            return true;
        }
        
        public static DialogueData LoadDialogue(string dialogueId)
        {
            return Saver.Load(DirectoryPath, dialogueId);
        }
    }
}