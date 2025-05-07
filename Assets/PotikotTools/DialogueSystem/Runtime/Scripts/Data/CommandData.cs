using Newtonsoft.Json;

namespace PotikotTools.DialogueSystem
{
    public enum CommandExecutionOrder
    {
        Immediately,
        ExitNode
    }
    
    public class CommandData
    {
        public string Text;
        public CommandExecutionOrder ExecutionOrder;
        public float Delay;

        [JsonIgnore] public bool HasDelay => Delay > 0;
    }
}