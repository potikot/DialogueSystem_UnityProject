using System.Linq;

namespace PotikotTools.DialogueSystem
{
    public class SingleChoiceNodeHandler : INodeHandler
    {
        private string[] _options = new string[]
        {
            "Next"
        };
        
        public bool CanHandle(NodeData data) => data is SingleChoiceNodeData;

        public void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView)
        {
            if (data is not SingleChoiceNodeData castedData)
                return;
            
            dialogueView.SetText(castedData.Text);
            dialogueView.SetOptions(_options);
            dialogueView.OnOptionSelected(optionIndex =>
            {
                controller.Next(optionIndex);
            });
        }
    }
}