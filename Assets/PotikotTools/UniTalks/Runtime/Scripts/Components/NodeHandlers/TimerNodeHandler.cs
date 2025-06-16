using System;
using System.Linq;

namespace PotikotTools.UniTalks
{
    public sealed class TimerNodeHandler : BaseNodeHandler
    {
        public override bool CanHandle(Type type) => type == typeof(TimerNodeData);
        public override bool CanHandle(NodeData data) => data is TimerNodeData;

        public override void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView)
        {
            if (data is not TimerNodeData castedData)
            {
                UniTalksAPI.LogError($"Invalid type of '{nameof(data)}'. Expected {nameof(TimerNodeData)}, got {data.GetType().Name}");
                return;
            }

            base.Handle(data, controller, dialogueView);
            dialogueView.SetAnswerOptions(castedData.OutputConnections.Select(oc => VariablesParser.Parse(oc.Text)).ToArray());

            var tdv = dialogueView.GetMenu<ITimerDialogueView>();
            tdv?.SetTimer(new Timer(castedData.Duration));
        }
    }
}