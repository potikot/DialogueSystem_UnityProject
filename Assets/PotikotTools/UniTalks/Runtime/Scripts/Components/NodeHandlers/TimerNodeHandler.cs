using System;
using System.Linq;

namespace PotikotTools.UniTalks
{
    public class TimerNodeHandler : INodeHandler
    {
        public bool CanHandle(Type type) => type == typeof(TimerNodeData);
        public bool CanHandle(NodeData data) => data is TimerNodeData;

        public void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView)
        {
            if (data is not TimerNodeData castedData)
            {
                DL.LogError($"Invalid type of '{nameof(data)}'. Expected {nameof(TimerNodeData)}, got {data.GetType().Name}");
                return;
            }

            foreach (var command in castedData.Commands)
                controller.HandleCommand(command);
            
            dialogueView.SetSpeaker(castedData.GetSpeaker());
            dialogueView.SetText(castedData.Text);
            dialogueView.SetAnswerOptions(castedData.OutputConnections.Select(oc => oc.Text).ToArray());
            dialogueView.OnOptionSelected(controller.Next);

            var tdv = dialogueView.GetMenu<ITimerDialogueView>();
            tdv?.SetTimer(new Timer(castedData.Duration));
        }
    }
}