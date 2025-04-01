namespace PotikotTools.DialogueSystem
{
    public interface INodeView
    {
        void Initialize(NodeData nodeData);
        void Draw();
        void Delete();
    }
}