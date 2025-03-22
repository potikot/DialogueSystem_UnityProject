using System.Collections.Generic;

namespace PotikotTools.DialogueSystem
{
    public class NodeData
    {
        public int Id { get; private set; }
        public string Text;
        public string AudioResourceName;
        public List<CommandData> Commands;

        public ConnectionData InputConnection;
        public List<ConnectionData> OutputConnections;
        
        public bool HasInputConnection => InputConnection != null;
        public bool HasOutputConnections => OutputConnections.Count > 0;
        
        private NodeData() { }

        public NodeData(int id)
        {
            Id = id;
            
            OutputConnections = new List<ConnectionData>();
            Commands = new List<CommandData>();
        }
    }
}