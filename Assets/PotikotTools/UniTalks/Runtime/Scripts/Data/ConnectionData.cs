using System;

namespace PotikotTools.UniTalks
{
    public class ConnectionData : IChangeNotifier
    {
        public event Action OnChanged;
        
        public NodeData From;
        public NodeData To;

        public ObservableList<CommandData> Commands;

        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnChanged?.Invoke();
            }
        }
        
        public ConnectionData()
        {
            Commands = new ObservableList<CommandData>();
            Commands.OnElementAdded += OnCommandAdded;
            Commands.OnElementRemoved += OnCommandRemoved;
            Commands.OnElementChanged += OnCommandChanged;
        }

        private void OnCommandAdded(CommandData cmd) => cmd.OnChanged += Internal_OnChanged;
        private void OnCommandRemoved(CommandData cmd) => cmd.OnChanged -= Internal_OnChanged;
        private void OnCommandChanged(int idx, CommandData prevCmd, CommandData newCmd)
        {
            prevCmd.OnChanged -= Internal_OnChanged;
            newCmd.OnChanged += Internal_OnChanged;
        }

        public ConnectionData(string text, NodeData from, NodeData to) : this()
        {
            Text = text;
            From = from;
            To = to;
        }

        private void Internal_OnChanged()
        {
            OnChanged?.Invoke();
        }
    }
}