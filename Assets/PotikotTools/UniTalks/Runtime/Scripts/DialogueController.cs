using System;
using System.Collections.Generic;

namespace PotikotTools.UniTalks
{
    public class DialogueController
    {
        public event Action<DialogueData, DialogueData> OnDialogueDataChanged;
        
        protected IDialogueView currentDialogueView;
        protected DialogueData currentDialogueData;

        protected NodeData currentNodeData;
        protected Dictionary<Type, INodeHandler> nodeHandlers;

        protected List<CommandData> commandsToExecuteOnExitNode;
        
        public bool IsDialogueStarted { get; protected set; }
        public DialogueData CurrentDialogueData => currentDialogueData;

        public virtual void Initialize(DialogueData dialogueData, IDialogueView dialogueView)
        {
            currentDialogueView = dialogueView;
            commandsToExecuteOnExitNode = new List<CommandData>();

            nodeHandlers = new Dictionary<Type, INodeHandler>
            {
                { typeof(SingleChoiceNodeData), new SingleChoiceNodeHandler() },
                { typeof(MultipleChoiceNodeData), new MultipleChoiceNodeHandler() },
                { typeof(TimerNodeData), new TimerNodeHandler() }
            };

            SetDialogueData(dialogueData);
        }

        public virtual void AddNodeHandler(Type nodeType, INodeHandler handler)
        {
            if (!nodeType.IsSubclassOf(typeof(NodeData)))
            {
                DL.LogWarning($"{nameof(nodeType)} should be a subclass of {nameof(NodeData)}");
                return;
            }
            if (!handler.CanHandle(nodeType))
            {
                DL.LogWarning($"{nameof(handler)} can't handle node type {nodeType}");
                return;
            }
            
            nodeHandlers.Add(nodeType, handler);
        }

        public virtual bool RemoveNodeHandler(Type nodeType)
        {
            return nodeHandlers.Remove(nodeType);
        }
        
        public virtual void StartDialogue()
        {
            if (IsDialogueStarted)
            {
                DL.LogWarning("Dialogue is already started");
                return;
            }
            if (currentDialogueData == null)
            {
                DL.LogWarning("Dialogue Data is null");
                return;
            }
            
            IsDialogueStarted = true;
            currentDialogueView.Show();
            currentNodeData = currentDialogueData.GetFirstNode();

            if (currentNodeData == null)
            {
                DL.LogWarning($"Dialogue graph '{currentDialogueData.Name}' is empty");
                return;
            }
            
            HandleNode(currentNodeData);
        }

        public virtual void StartDialogue(DialogueData dialogueData)
        {
            SetDialogueData(dialogueData);
            StartDialogue();
        }
        
        public virtual void EndDialogue()
        {
            if (!IsDialogueStarted)
            {
                DL.LogWarning("Dialogue is not started");
                return;
            }
            
            currentDialogueView.OnOptionSelected(null);
            currentDialogueView.SetAnswerOptions(null);
            currentDialogueView.Hide();
            IsDialogueStarted = false;
        }
        
        public virtual void Next(int choice = 0)
        {
            if (!IsDialogueStarted)
                StartDialogue();

            foreach (var command in commandsToExecuteOnExitNode)
                ExecuteCommandAsync(command);
            
            if (currentNodeData.HasOutputConnections)
            {
                if (currentNodeData.OutputConnections[choice].To == null)
                {
                    EndDialogue();
                    return;
                }

                currentNodeData = currentNodeData.OutputConnections[choice].To;
                HandleNode(currentNodeData);
            }
            else
            {
                EndDialogue();
            }
        }

        public virtual void HandleCommand(CommandData command)
        {
            switch (command.ExecutionOrder)
            {
                case CommandExecutionOrder.Immediately:
                    ExecuteCommandAsync(command);
                    break;
                case CommandExecutionOrder.ExitNode:
                    commandsToExecuteOnExitNode.Add(command);
                    break;
                default:
                    DL.LogError($"Unknown Execution Order: {command.ExecutionOrder}");
                    break;
            }
        }
        
        protected virtual void SetDialogueData(DialogueData dialogueData)
        {
            if (dialogueData == null)
            {
                DL.LogError("Dialogue Data is null");
                return;
            }
            if (dialogueData == currentDialogueData)
                return;
            if (IsDialogueStarted)
                EndDialogue();

            var prevDialogueData = currentDialogueData;
            currentDialogueData = dialogueData;
            OnDialogueDataChanged?.Invoke(prevDialogueData, currentDialogueData);
        }
        
        protected virtual void HandleNode(NodeData node)
        {
            Type nodeType = node.GetType();

            if (nodeHandlers.TryGetValue(nodeType, out var handler))
                handler.Handle(node, this, currentDialogueView);
            else
                DL.LogError($"Unknown Node Type: {nodeType}");
        }
        
        protected virtual async void ExecuteCommandAsync(CommandData command)
        {
            if (command.HasDelay)
                await DialoguesComponents.CommandHandler.ExecuteWithDelayAsync(command.Text, command.Delay);
            else
                DialoguesComponents.CommandHandler.Execute(command.Text);
        }
    }
}