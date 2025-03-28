namespace PotikotTools.DialogueSystem
{
    public class SingleChoiceNodeHandler : INodeHandler
    {
        public bool CanHandle(NodeData data) => data is SingleChoiceNodeData;

        public void Handle(NodeData data, DialogueController controller)
        {
            
        }
    }
}