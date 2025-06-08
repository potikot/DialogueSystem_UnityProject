using UnityEngine;

namespace PotikotTools.UniTalks
{
    public static class DialoguesComponents
    {
        private static DialogueDatabase _database;
        private static NodeBinder _nodeBinder;
        private static CommandHandler _commandHandler;
        private static IAudioHandler _audioHandler;
        
        private static bool _isInitialized;
        
        public static DialogueDatabase Database
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

        public static IAudioHandler AudioHandler
        {
            get => _audioHandler;
            set
            {
                if (value == null)
                    return;
                
                _audioHandler = value;
            }
        }
        
        static DialoguesComponents() => Initialize();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_isInitialized)
                return;
            
            _isInitialized = true;
            Database = new DialogueDatabase();
            NodeBinder = new NodeBinder();
            CommandHandler = new CommandHandler();
        }
    }
}