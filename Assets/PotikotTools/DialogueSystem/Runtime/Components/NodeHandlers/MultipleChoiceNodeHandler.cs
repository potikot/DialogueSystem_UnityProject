using System.Linq;

namespace PotikotTools.DialogueSystem
{
    public class MultipleChoiceNodeHandler : INodeHandler
    {
        public bool CanHandle(NodeData data) => data is MultipleChoiceNodeData;

        public void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView)
        {
            if (data is not MultipleChoiceNodeData castedData)
                return;
            
            dialogueView.SetText(castedData.Text);
            dialogueView.SetOptions(castedData.OutputConnections.Select(oc => oc.Text).ToArray());
            dialogueView.OnOptionSelected(optionIndex =>
            {
                controller.Next(optionIndex);
            });
        }
    }
}