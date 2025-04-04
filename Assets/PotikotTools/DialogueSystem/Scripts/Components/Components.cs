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
        public static DialogueResourceManager ResourceManager = new DialogueResourceManager();

        public static AudioController AudioController
        {
            get;
            set;
        }

        public static CommandController CommandController
        {
            get;
            set;
        }
    }
}