using System;
using Newtonsoft.Json;

namespace PotikotTools.UniTalks
{
    public enum CommandExecutionOrder
    {
        Immediately,
        ExitNode
    }
    
    public class CommandData : IChangeNotifier
    {
        public event Action OnChanged;
        
        private string _text;
        private CommandExecutionOrder _executionOrder;
        private float _delay;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnChanged?.Invoke();
            }
        }
        
        public CommandExecutionOrder ExecutionOrder
        {
            get => _executionOrder;
            set
            {
                _executionOrder = value;
                OnChanged?.Invoke();
            }
        }
        
        public float Delay
        {
            get => _delay;
            set
            {
                _delay = value;
                OnChanged?.Invoke();
            }
        }
        [JsonIgnore] public bool HasDelay => Delay > 0;
    }
}