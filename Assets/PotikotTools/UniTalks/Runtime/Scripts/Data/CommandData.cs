using System;
using MessagePack;
using Newtonsoft.Json;

namespace PotikotTools.UniTalks
{
    public enum CommandExecutionOrder
    {
        Immediately,
        ExitNode
    }
    
    [MessagePackObject]
    public class CommandData : IChangeNotifier
    {
        public event Action OnChanged;
        
        private string _text;
        private CommandExecutionOrder _executionOrder;
        private float _delay;

        [Key(0)]
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnChanged?.Invoke();
            }
        }
        
        [Key(1)]
        public CommandExecutionOrder ExecutionOrder
        {
            get => _executionOrder;
            set
            {
                _executionOrder = value;
                OnChanged?.Invoke();
            }
        }
        
        [Key(2)]
        public float Delay
        {
            get => _delay;
            set
            {
                _delay = value;
                OnChanged?.Invoke();
            }
        }
        
        [JsonIgnore, IgnoreMember]
        public bool HasDelay => Delay > 0;
    }
}