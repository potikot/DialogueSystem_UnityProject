using System;

namespace PotikotTools.UniTalks
{
    public abstract class BaseNodeHandler
    {
        public abstract bool CanHandle(Type type);
        public abstract bool CanHandle(NodeData data);

        public virtual void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView)
        {
            foreach (var command in data.Commands)
                controller.HandleCommand(command);
            
            dialogueView.SetSpeaker(data.GetSpeaker());
            dialogueView.SetText(VariablesParser.Parse(data.Text));
            dialogueView.OnOptionSelected(controller.Next);
        }
    }
}