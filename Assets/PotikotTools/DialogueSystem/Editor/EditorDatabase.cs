using System.Threading.Tasks;

namespace PotikotTools.DialogueSystem
{
    public static class EditorDatabase
    {
        private static IDialogueSaver _saver;
        private static Database database => Components.Database;

        static EditorDatabase()
        {
            _saver = new JsonDialogueSaverLoader();
        }
        
        public static async Task<bool> SaveDialogueAsync(DialogueData dialogue)
        {
            return await _saver.SaveAsync(database.RootPath, dialogue);
        }
        
        public static bool SaveDialogue(DialogueData dialogue)
        {
            return _saver.Save(database.RootPath, dialogue);
        }
    }
}