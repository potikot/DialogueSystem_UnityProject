namespace PotikotTools.DialogueSystem
{
    public interface INodeHandler
    {
        bool CanHandle(NodeData data);
        void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView);
    }
}