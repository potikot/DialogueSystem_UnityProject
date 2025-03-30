using System;

namespace PotikotTools.DialogueSystem
{
    public enum CommandExecutionOrder
    {
        BeforePhrase,
        AfterPhrase
    }
    
    [Serializable]
    public class CommandData
    {
        public string Command;
        public CommandExecutionOrder ExecutionOrder;
        public float Delay;

        public bool HasDelay => Delay > 0f;
    }
}