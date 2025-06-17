using System.Collections.Generic;
using System.Linq;

namespace PotikotTools.UniTalks
{
    public class NodeBinder
    {
        private readonly Dictionary<int, List<int>> _connections = new();
        private readonly List<(int from, int to)> _chainedConnections = new();
        
        public void AddConnection(int from, int to)
        {
            if (_connections.TryGetValue(from, out List<int> connections))
                connections.Add(to);
            else
                _connections.Add(from, new List<int> { to });
        }
        
        public void AddChainedConnection(int from, int to)
        {
            _chainedConnections.Add((from, to));
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

            foreach (var connection in _chainedConnections)
            {
                NodeData outputNode = null;
                NodeData inputNode = null;
                
                foreach (var node in nodes)
                {
                    if (node.Id == connection.from)
                        outputNode = node;
                    if (node.Id == connection.to)
                        inputNode = node;
                }
                
                if (outputNode != null && inputNode != null)
                    outputNode.ChainNode(inputNode);
            }
        }

        public void Clear()
        {
            _connections.Clear();
        }
    }
}