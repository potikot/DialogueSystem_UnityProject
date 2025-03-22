using System.Collections.Generic;
using System.Linq;

namespace PotikotTools.DialogueSystem
{
    public class DialogueData
    {
        public string Id { get; private set; }

        public List<NodeData> Nodes;

        public DialogueData(string id)
        {
            Id = id;
            
            Nodes = new List<NodeData>();
        }

        public void AddNode(NodeData node)
        {
            Nodes.Add(node);
        }

        public bool RemoveNode(NodeData node)
        {
            return Nodes.Remove(node);
        }
        
        public NodeData GetFirstNode()
        {
            return Nodes.First(n => !n.HasInputConnection);
        }
    }
}