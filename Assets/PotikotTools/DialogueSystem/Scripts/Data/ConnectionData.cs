using Newtonsoft.Json;

namespace PotikotTools.DialogueSystem
{
    public class ConnectionData
    {
        public string Text;
        [JsonIgnore] public NodeData From;
        [JsonIgnore] public NodeData To;

        public ConnectionData(string text)
        {
            Text = text;
        }
        
        public ConnectionData(string text, NodeData from, NodeData to) : this(text)
        {
            From = from;
            To = to;
        }
    }
}