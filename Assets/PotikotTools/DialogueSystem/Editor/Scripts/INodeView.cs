namespace PotikotTools.DialogueSystem
{
    public interface INodeView
    {
        void Initialize(NodeData nodeData);
        NodeData GetData();
        void Draw();
        void Delete();
    }
}