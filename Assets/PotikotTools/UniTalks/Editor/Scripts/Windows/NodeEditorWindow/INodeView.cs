using System;

namespace PotikotTools.UniTalks.Editor
{
    public interface INodeView
    {
        event Action OnChanged;
        
        void Initialize(EditorNodeData editorData, NodeData data, DialogueGraphView graphView);
        NodeData GetData();
        void Draw();
        void OnDelete();
    }
}