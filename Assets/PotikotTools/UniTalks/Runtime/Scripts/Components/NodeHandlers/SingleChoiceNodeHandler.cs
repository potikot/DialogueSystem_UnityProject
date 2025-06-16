using System;

namespace PotikotTools.UniTalks
{
    public sealed class SingleChoiceNodeHandler : BaseNodeHandler
    {
        public override bool CanHandle(Type type) => type == typeof(SingleChoiceNodeData);
        public override bool CanHandle(NodeData data) => data is SingleChoiceNodeData;

        public override void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView)
        {
            if (data is not SingleChoiceNodeData)
            {
                UniTalksAPI.LogError($"Invalid type of '{nameof(data)}'. Expected {nameof(SingleChoiceNodeData)}, got {data.GetType().Name}");
                return;
            }
            
            base.Handle(data, controller, dialogueView);
            dialogueView.SetAnswerOptions(UniTalksPreferences.EmptyDialogueOptions);
        }
    }
}