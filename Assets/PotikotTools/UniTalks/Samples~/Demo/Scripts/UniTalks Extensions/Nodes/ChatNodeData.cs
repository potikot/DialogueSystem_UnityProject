using System;
using System.Linq;

namespace PotikotTools.UniTalks.Demo
{
    public class ChatNodeData : NodeData
    {
        public ChatNodeData(int id) : base(id) { }
    }
    
    public class ChatNodeHandler : INodeHandler
    {
        public bool CanHandle(Type type) => type == typeof(ChatNodeData);
        public bool CanHandle(NodeData data) => data is ChatNodeData;

        public void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView)
        {
            if (data is not ChatNodeData castedData)
            {
                DL.LogError($"Invalid type of '{nameof(data)}'. Expected {nameof(ChatNodeData)}, got {data.GetType().Name}");
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