namespace PotikotTools.DialogueSystem
{
    public interface INodeView
    {
        void Initialize(EditorNodeData editorData, NodeData data);
        NodeData GetData();
        void Draw();
        void Delete();
    }
}