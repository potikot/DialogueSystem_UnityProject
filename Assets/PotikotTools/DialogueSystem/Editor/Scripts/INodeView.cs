namespace PotikotTools.DialogueSystem.Editor
{
    public interface INodeView
    {
        void Initialize(EditorNodeData editorData, NodeData data, DialogueGraphView graphView);
        NodeData GetData();
        void Draw();
        void OnDelete();
    }
}