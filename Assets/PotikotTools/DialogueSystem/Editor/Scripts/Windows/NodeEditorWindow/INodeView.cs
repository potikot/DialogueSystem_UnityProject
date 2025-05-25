using System;

namespace PotikotTools.DialogueSystem.Editor
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