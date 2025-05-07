using System.Collections.Generic;
using System.Linq;

namespace PotikotTools.DialogueSystem
{
    public class NodeLinker
    {
        private Dictionary<int, List<int>> _connections;

        public NodeLinker()
        {
            _connections = new Dictionary<int, List<int>>();
        }

        public void AddConnection(int from, int to)
        {
            if (_connections.TryGetValue(from, out List<int> connections))
                connections.Add(to);
            else
                _connections.Add(from, new List<int> { to });
        }
        
        public void SetConnections(DialogueData data)
        {
            IReadOnlyList<NodeData> nodes = data.Nodes;

            foreach (var connection in _connections)
            {
                NodeData node = nodes.First(n => n.Id == connection.Key);
                int i = 0;
                foreach (int toNodeId in connection.Value)
                {
                    node.OutputConnections[i].From = node;
                    node.OutputConnections[i].To = nodes.First(n => n.Id == toNodeId);
                    node.OutputConnections[i].To.InputConnection = node.OutputConnections[i];
                    i++;
                }
            }
        }

        public void Clear()
        {
            _connections.Clear();
        }
    }
}