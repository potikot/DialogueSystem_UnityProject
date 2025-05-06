using UnityEditor;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class CommandController
    {
        
    }

    public class AudioController
    {
        
    }
    
    [InitializeOnLoad]
    [DefaultExecutionOrder(-1000)]
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

        private static NodeLinker _nodeLinker;

        public static NodeLinker NodeLinker => _nodeLinker ??= new NodeLinker();

        static Components()
        {
            Database = new Database();
        }
    }
}