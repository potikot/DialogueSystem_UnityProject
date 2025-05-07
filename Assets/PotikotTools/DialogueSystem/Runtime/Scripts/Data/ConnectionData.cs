namespace PotikotTools.DialogueSystem
{
    public class ConnectionData
    {
        public string Text;
        public NodeData From;
        public NodeData To;

        public ConnectionData() { }

        public ConnectionData(string text, NodeData from, NodeData to)
        {
            Text = text;
            From = from;
            To = to;
        }
    }
}