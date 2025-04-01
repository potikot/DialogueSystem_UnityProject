using Newtonsoft.Json;

namespace PotikotTools.DialogueSystem
{
    public enum CommandExecutionOrder
    {
        BeforePhrase,
        AfterPhrase
    }
    
    public class CommandData
    {
        public string Command;
        public CommandExecutionOrder ExecutionOrder;
        public float Delay;

        [JsonIgnore] public bool HasDelay => Delay > 0f;
    }
}