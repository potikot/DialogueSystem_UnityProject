using System.Collections.Generic;

namespace PotikotTools.DialogueSystem
{
    public class ConnectionData
    {
        public string Text;
        public NodeData From;
        public NodeData To;

        public ObservableList<CommandData> Commands;

        public ConnectionData()
        {
            Commands = new ObservableList<CommandData>();
        }

        public ConnectionData(string text, NodeData from, NodeData to) : this()
        {
            Text = text;
            From = from;
            To = to;
        }
    }
}