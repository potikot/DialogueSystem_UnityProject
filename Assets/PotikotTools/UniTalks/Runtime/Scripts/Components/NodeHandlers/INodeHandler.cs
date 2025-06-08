using System;

namespace PotikotTools.UniTalks
{
    public interface INodeHandler
    {
        bool CanHandle(Type type);
        bool CanHandle(NodeData data);
        void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView);
    }
}