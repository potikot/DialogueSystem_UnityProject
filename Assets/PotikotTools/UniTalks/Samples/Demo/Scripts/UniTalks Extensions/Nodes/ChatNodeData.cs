using System;
using System.Linq;

namespace PotikotTools.UniTalks.Demo
{
    public class ChatNodeData : NodeData
    {
        public ChatNodeData(int id) : base(id) { }
    }
    
    public sealed class ChatNodeHandler : BaseNodeHandler
    {
        public override bool CanHandle(Type type) => type == typeof(ChatNodeData);
        public override bool CanHandle(NodeData data) => data is ChatNodeData;

        public override void Handle(NodeData data, DialogueController controller, IDialogueView dialogueView)
        {
            if (data is not ChatNodeData castedData)
            {
                UniTalksAPI.LogError($"Invalid type of '{nameof(data)}'. Expected {nameof(ChatNodeData)}, got {data.GetType().Name}");
                return;
            }

            base.Handle(data, controller, dialogueView);

            string[] options = controller switch
            {
                ChatDialogueController chatController => chatController.Options,
                _ => castedData.OutputConnections.Select(oc => oc.Text).ToArray()
            };

            if (options.Length == 0)
                options = UniTalksPreferences.EmptyDialogueOptions;
            
            dialogueView.SetAnswerOptions(options);
        }
    }
}