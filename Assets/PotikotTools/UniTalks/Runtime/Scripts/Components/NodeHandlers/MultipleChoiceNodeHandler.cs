using System;
using System.Linq;

namespace PotikotTools.UniTalks
{
    public class MultipleChoiceNodeHandler : INodeHandler
    {
        public bool CanHandle(Type type) => type == typeof(MultipleChoiceNodeData);
        public bool CanHandle(NodeData data) => data is MultipleChoiceNodeData;

        public void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView)
        {
            if (data is not MultipleChoiceNodeData castedData)
            {
                DL.LogError($"Invalid type of '{nameof(data)}'. Expected {nameof(MultipleChoiceNodeData)}, got {data.GetType().Name}");
                return;
            }

            foreach (var command in castedData.Commands)
                controller.HandleCommand(command);

            dialogueView.SetSpeaker(castedData.GetSpeaker());
            dialogueView.SetText(castedData.Text);
            dialogueView.SetAnswerOptions(castedData.OutputConnections.Select(oc => oc.Text).ToArray());
            dialogueView.OnOptionSelected(controller.Next);
        }
    }
}