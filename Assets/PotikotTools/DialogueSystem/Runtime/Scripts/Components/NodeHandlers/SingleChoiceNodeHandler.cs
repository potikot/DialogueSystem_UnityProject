using System;

namespace PotikotTools.DialogueSystem
{
    public class SingleChoiceNodeHandler : INodeHandler
    {
        private readonly string[] _options = { "Next" };

        public bool CanHandle(Type type) => type == typeof(SingleChoiceNodeData);
        public bool CanHandle(NodeData data) => data is SingleChoiceNodeData;

        public void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView)
        {
            if (data is not SingleChoiceNodeData castedData)
            {
                DL.LogError($"Invalid type of '{nameof(data)}'. Expected {nameof(SingleChoiceNodeData)}, got {data.GetType().Name}");
                return;
            }
            
            foreach (var command in castedData.Commands)
                controller.HandleCommand(command);
            
            dialogueView.SetSpeakerText(castedData.Text);
            dialogueView.SetAnswerOptions(_options);
            dialogueView.OnOptionSelected(controller.Next);
        }
    }
}