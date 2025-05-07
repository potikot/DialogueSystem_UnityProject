using UnityEditor;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    [InitializeOnLoad]
    [DefaultExecutionOrder(-1000)]
    public static class Components
    {
        private static Database _database;
        private static NodeLinker _nodeLinker;
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
        
        public static NodeLinker NodeLinker
        {
            get => _nodeLinker;
            set
            {
                if (value == null)
                    return;
                
                _nodeLinker = value;
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
            NodeLinker = new NodeLinker();
            CommandHandler = new CommandHandler();
        }
    }
}