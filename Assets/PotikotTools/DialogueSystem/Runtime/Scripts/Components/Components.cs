using UnityEditor;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    [InitializeOnLoad]
    [DefaultExecutionOrder(-1000)]
    public static class Components
    {
        private static Database _database;
        private static NodeBinder _nodeBinder;
        private static CommandHandler _commandHandler;
        
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
        
        public static NodeBinder NodeBinder
        {
            get => _nodeBinder;
            set
            {
                if (value == null)
                    return;
                
                _nodeBinder = value;
            }
        }
        
        public static CommandHandler CommandHandler
        {
            get => _commandHandler;
            set
            {
                if (value == null)
                    return;
                
                _commandHandler = value;
            }
        }
        
        static Components()
        {
            Database = new Database();
            NodeBinder = new NodeBinder();
            CommandHandler = new CommandHandler();
        }
    }
}