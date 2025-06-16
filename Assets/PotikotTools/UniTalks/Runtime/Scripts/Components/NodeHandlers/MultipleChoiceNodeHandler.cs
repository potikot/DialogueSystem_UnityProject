using System;
using System.Linq;

namespace PotikotTools.UniTalks
{
    public sealed class MultipleChoiceNodeHandler : BaseNodeHandler
    {
        public override bool CanHandle(Type type) => type == typeof(MultipleChoiceNodeData);
        public override bool CanHandle(NodeData data) => data is MultipleChoiceNodeData;

        public override void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView)
        {
            if (data is not MultipleChoiceNodeData castedData)
            {
                UniTalksAPI.LogError($"Invalid type of '{nameof(data)}'. Expected {nameof(MultipleChoiceNodeData)}, got {data.GetType().Name}");
                return;
            }

            base.Handle(data, controller, dialogueView);
            dialogueView.SetAnswerOptions(castedData.OutputConnections.Select(oc => VariablesParser.Parse(oc.Text)).ToArray());
        }
    }
}