namespace PotikotTools.DialogueSystem
{
    public class MultipleChoiceNodeHandler : INodeHandler
    {
        public bool CanHandle(NodeData data) => data is MultipleChoiceNodeData;

        public void Handle(NodeData data, DialogueController controller)
        {
            if (data is not MultipleChoiceNodeData castedData)
                return;
            
            // TODO: logic
        }
    }
}