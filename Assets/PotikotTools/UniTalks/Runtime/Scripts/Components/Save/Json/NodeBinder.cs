using System.Collections.Generic;
using System.Linq;

namespace PotikotTools.UniTalks
{
    public class NodeBinder
    {
        private readonly Dictionary<int, List<int>> _connections = new();

        public void AddConnection(int from, int to)
        {
            if (_connections.TryGetValue(from, out List<int> connections))
                connections.Add(to);
            else
                _connections.Add(from, new List<int> { to });
        }
        
        public void Bind(DialogueData data)
        {
            IReadOnlyList<NodeData> nodes = data.Nodes;

            foreach (var nodeData in nodes)
            {
                foreach (var connection in nodeData.OutputConnections)
                    connection.OnChanged += nodeData.Internal_OnChanged;
                foreach (var command in nodeData.Commands)
                    command.OnChanged += nodeData.Internal_OnChanged;
            }
            
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