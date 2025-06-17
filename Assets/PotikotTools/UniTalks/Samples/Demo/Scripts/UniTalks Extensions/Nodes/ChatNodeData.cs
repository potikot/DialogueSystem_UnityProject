using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace PotikotTools.UniTalks.Demo
{
    public class ChatNodeData : NodeData
    {
        private float _delay;
        private ConnectionData _nextChainedNode;
        
        public float Delay
        {
            get => _delay;
            set
            {
                if (Mathf.Approximately(_delay, value))
                    return;
                
                _delay = value;
                CallOnChanged();
            }
        }

        public ConnectionData NextChainedNode
        {
            get => _nextChainedNode;
            set
            {
                if (_nextChainedNode == value)
                    return;
                
                _nextChainedNode = value;
                CallOnChanged();
            }
        }
        
        public ChatNodeData(int id) : base(id) { }

        public override void ChainNode(NodeData node)
        {
            if (node == null)
                return;
            
            NextChainedNode ??= new ConnectionData("$_$", null, null);
            NextChainedNode.From = this;
            NextChainedNode.To = node;
        }
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

            CoroutineRunner.Run(Handle(castedData, controller, dialogueView));
        }

        private IEnumerator Handle(ChatNodeData data, DialogueController controller, IDialogueView dialogueView)
        {
            yield return new WaitForSeconds(data.Delay);
            
            base.Handle(data, controller, dialogueView);

            if (data.NextChainedNode != null)
            {
                controller.Next();
                if (dialogueView is ChatDialogueView cdv)
                    cdv.DisableOptions();
            }
            else
            {
                string[] options = controller switch
                {
                    ChatDialogueController chatController => chatController.Options,
                    _ => data.OutputConnections.Select(oc => oc.Text).ToArray()
                };

                if (options.Length == 0)
                    options = UniTalksPreferences.EmptyDialogueOptions;
                
                dialogueView.SetAnswerOptions(options);
            }
        }
    }
}