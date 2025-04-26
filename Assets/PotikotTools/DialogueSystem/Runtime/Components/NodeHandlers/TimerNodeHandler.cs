using System.Linq;

namespace PotikotTools.DialogueSystem
{
    public class TimerNodeHandler : INodeHandler
    {
        public bool CanHandle(NodeData data) => data is TimerNodeData;

        public void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView)
        {
            if (data is not TimerNodeData castedData)
                return;

            dialogueView.SetText(castedData.Text);
            dialogueView.SetOptions(castedData.OutputConnections.Select(oc => oc.Text).ToArray());
            dialogueView.OnOptionSelected(index =>
            {
                controller.Next(index);
            });

            var tdv = dialogueView.GetMenu<ITimerDialogueView>();
            if (tdv != null)
                tdv.SetTimer(new Timer(castedData.Duration));
        }
    }
}