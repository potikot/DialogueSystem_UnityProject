using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Editor
{
    public interface INodeView
    {
        event Action OnChanged;
        
        void Initialize(EditorNodeData editorData, NodeData data, DialogueGraphView graphView);
        NodeData GetData();
        void Draw();
        void OnDelete();
        
        void OnConnected(Edge edge);
        void OnDisconnected(Edge edge);
        void DrawEdges(UQueryState<Node> nodes);
    }
}