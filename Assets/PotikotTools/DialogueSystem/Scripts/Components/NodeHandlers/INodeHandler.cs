namespace PotikotTools.DialogueSystem
{
    public interface INodeHandler
    {
        public bool CanHandle(NodeData data);
        public void Handle(NodeData data, DialogueController controller);
    }
}