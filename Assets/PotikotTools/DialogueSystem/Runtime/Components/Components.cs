using UnityEditor;

namespace PotikotTools.DialogueSystem
{
    public class CommandController
    {
        
    }

    public class AudioController
    {
        
    }
    
    public static class Components
    {
        private static Database _database;

        public static Database Database
        {
            get => _database;
            set
            {
                if (value == null)
                    return;

                _database = value;
                _database.Initialize();
            }
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            Database = new Database();
        }
    }
}