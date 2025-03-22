namespace PotikotTools.DialogueSystem
{
    public class CommandController
    {
    }

    public class AudioController
    {
    }
    
    public static class DialogueComponents
    {
        public static IDialogueSaver Saver = new JsonDialogueSaver();
        
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