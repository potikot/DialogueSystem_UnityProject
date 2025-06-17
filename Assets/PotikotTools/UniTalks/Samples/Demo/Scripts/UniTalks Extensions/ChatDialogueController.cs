using System.Collections.Generic;
using System.Linq;

namespace PotikotTools.UniTalks.Demo
{
    public class ChatDialogueController : DialogueController
    {
        private readonly List<ConnectionData> _availableOptions = new();
        private ChatDialogueView _chatDialogueView;
        
        public bool IsDialogueStopped { get; private set; }
        
        public string[] Options => _availableOptions.Select(o => o.Text).ToArray();

        public override void Initialize(DialogueData dialogueData, IDialogueView dialogueView)
        {
            base.Initialize(dialogueData, dialogueView);
            
            _chatDialogueView = dialogueView as ChatDialogueView;
            nodeHandlers.Add(typeof(ChatNodeData), new ChatNodeHandler());
        }

        public override void StartDialogue()
        {
            if (IsDialogueStarted && !IsDialogueStopped)
            {
                UniTalksAPI.LogError("Dialogue is already started");
                return;
            }
            if (currentDialogueData == null)
            {
                UniTalksAPI.LogError("Dialogue Data is null");
                return;
            }
            
            IsDialogueStarted = true;
            currentDialogueView.Show();
            currentNodeData = currentDialogueData.GetFirstNode();

            if (currentNodeData == null)
            {
                UniTalksAPI.LogError($"Dialogue graph '{currentDialogueData.Name}' is empty");
                return;
            }
            
            if (!IsDialogueStopped)
                HandleNode(currentNodeData);
            
            IsDialogueStopped = false;
        }

        public override void EndDialogue()
        {
            base.EndDialogue();
            currentDialogueView.Show();
        }
        
        public void StopDialogue()
        {
            currentDialogueView.Hide();
            IsDialogueStopped = true;
        }

        public override void Next(int choice = 0)
        {
            if (!IsDialogueStarted)
                StartDialogue();

            foreach (var command in commandsToExecuteOnExitNode)
                ExecuteCommandAsync(command);

            if (!currentNodeData.HasOutputConnections
                && currentNodeData is ChatNodeData { NextChainedNode: not null } cnd)
            {
                if (cnd.NextChainedNode.To == null)
                    EndDialogue();
                else
                {
                    currentNodeData = cnd.NextChainedNode.To;
                    HandleNode(currentNodeData);
                }
                
                return;
            }
            
            if (_availableOptions.Count > 0)
            {
                var opt = _availableOptions[choice];
                foreach (var cmd in opt.Commands)
                    ExecuteCommandAsync(cmd);
                
                _chatDialogueView?.SetAnswerText(opt.Text);
                
                if (opt.To == null)
                {
                    EndDialogue();
                    return;
                }

                currentNodeData = opt.To;
                HandleNode(currentNodeData);
            }
            else
            {
                EndDialogue();
            }
        }

        public void ClearOptions()
        {
            _availableOptions.Clear();
            _chatDialogueView.SetAnswerOptions(null);
        }
        
        protected override void SetDialogueData(DialogueData dialogueData)
        {
            base.SetDialogueData(dialogueData);

            if (currentDialogueData != null
                && currentDialogueView is ChatDialogueView cv
                && currentDialogueData.TryGetSpeaker(0, out var speaker)
                && speaker is ChatSpeakerData cs)
            {
                cv.SetAvatarImage(cs.Avatar);
            }
        }

        protected override void HandleNode(NodeData node)
        {
            _availableOptions.AddRange(node.OutputConnections);
            
            for (int i = 0; i < _availableOptions.Count; i++)
            {
                var opt = _availableOptions[i];
                if (opt.To == null) 
                    UniTalksAPI.LogWarning("Dialogue graph 'opt' is null");
                if (opt.To == node)
                {
                    _availableOptions.RemoveAt(i);
                    break;
                }
            }
            
            base.HandleNode(node);
        }
    }
}