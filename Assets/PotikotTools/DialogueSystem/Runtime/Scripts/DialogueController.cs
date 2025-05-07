using System;
using System.Collections.Generic;

namespace PotikotTools.DialogueSystem
{
    public class DialogueController
    {
        private IDialogueView _dialogueView;
        private DialogueData _dialogueData;

        private NodeData _currentNodeData;
        private Dictionary<Type, INodeHandler> _nodeHandlers;

        private List<CommandData> _commandsToExecuteOnExitNode;
        
        public bool IsDialogueStarted { get; private set; }

        public void Initialize(DialogueData dialogueData, IDialogueView dialogueView)
        {
            _dialogueView = dialogueView;
            _commandsToExecuteOnExitNode = new List<CommandData>();
            
            _nodeHandlers = new Dictionary<Type, INodeHandler>
            {
                { typeof(SingleChoiceNodeData), new SingleChoiceNodeHandler() },
                { typeof(MultipleChoiceNodeData), new MultipleChoiceNodeHandler() },
                { typeof(TimerNodeData), new TimerNodeHandler() }
            };

            SetDialogueData(dialogueData);
        }

        public void AddNodeHandler(Type nodeType, INodeHandler handler)
        {
            if (!nodeType.IsSubclassOf(typeof(NodeData)))
            {
                DL.LogError($"{nameof(nodeType)} should be a subclass of {nameof(NodeData)}");
                return;
            }
            if (!handler.CanHandle(nodeType))
            {
                DL.LogError($"{nameof(handler)} can't handle node type {nodeType}");
                return;
            }
            
            _nodeHandlers.Add(nodeType, handler);
        }
        
        public void StartDialogue()
        {
            if (IsDialogueStarted)
            {
                DL.LogError("Dialogue is already started");
                return;
            }
            if (_dialogueData == null)
            {
                DL.LogError("Dialogue Data is null");
                return;
            }
            
            DL.Log("Start Dialogue");
            IsDialogueStarted = true;
            _dialogueView.Show();
            _currentNodeData = _dialogueData.GetFirstNode();
            HandleNode(_currentNodeData);
        }

        public void StartDialogue(DialogueData dialogueData)
        {
            SetDialogueData(dialogueData);
            StartDialogue();
        }
        
        public void EndDialogue()
        {
            if (!IsDialogueStarted)
            {
                DL.LogError("Dialogue is not started");
                return;
            }
            
            DL.Log("End Dialogue");
            _dialogueView.OnOptionSelected(null);
            _dialogueView.Hide();
            IsDialogueStarted = false;
        }
        
        public void Next(int choice = 0)
        {
            if (!IsDialogueStarted)
                StartDialogue();

            foreach (var command in _commandsToExecuteOnExitNode)
                ExecuteCommandAsync(command);
            
            if (_currentNodeData.HasOutputConnections)
            {
                if (_currentNodeData.OutputConnections[choice].To == null)
                {
                    EndDialogue();
                    return;
                }

                _currentNodeData = _currentNodeData.OutputConnections[choice].To;
                HandleNode(_currentNodeData);
            }
            else
            {
                EndDialogue();
            }
        }

        private void SetDialogueData(DialogueData dialogueData)
        {
            if (dialogueData == null)
            {
                DL.LogError("Dialogue Data is null");
                return;
            }
            if (IsDialogueStarted)
                EndDialogue();

            _dialogueData = dialogueData;
        }
        
        private void HandleNode(NodeData node)
        {
            Type nodeType = node.GetType();

            if (_nodeHandlers.TryGetValue(nodeType, out var handler))
                handler.Handle(node, this, _dialogueView);
            else
                DL.LogError($"Unknown Node Type: {nodeType}");
        }
        
        public void HandleCommand(CommandData command)
        {
            switch (command.ExecutionOrder)
            {
                case CommandExecutionOrder.Immediately:
                    ExecuteCommandAsync(command);
                    break;
                case CommandExecutionOrder.ExitNode:
                    _commandsToExecuteOnExitNode.Add(command);
                    break;
                default:
                    DL.LogError($"Unknown Execution Order: {command.ExecutionOrder}");
                    break;
            }
        }

        private async void ExecuteCommandAsync(CommandData command)
        {
            DL.LogError(command.Text);
            if (command.HasDelay)
                await Components.CommandHandler.ExecuteWithDelayAsync(command.Text, command.Delay);
            else
                Components.CommandHandler.Execute(command.Text);
        }
    }
}