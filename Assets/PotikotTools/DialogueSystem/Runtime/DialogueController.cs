using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class DialogueController
    {
        public event Action OnDialogueStarted;
        public event Action OnDialogueEnded;
        public event Action<NodeData> OnDialogueProgress;
        
        private IDialogueView _dialogueView;
        private DialogueData _dialogueData;

        private NodeData _currentNodeData;

        public bool IsDialogueStarted { get; private set; }

        public List<INodeHandler> NodeHandlers;

        public void Initialize(DialogueData dialogueData, IDialogueView dialogueView)
        {
            _dialogueView = dialogueView;
            SetDialogueData(dialogueData);

            NodeHandlers = new List<INodeHandler>
            {
                new SingleChoiceNodeHandler(),
                new MultipleChoiceNodeHandler()
            };
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

            OnDialogueStarted?.Invoke();
            OnDialogueProgress?.Invoke(_currentNodeData);
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
            OnDialogueEnded?.Invoke();
        }
        
        public void Next(int choice = 0)
        {
            if (!IsDialogueStarted)
                StartDialogue();

            if (_currentNodeData.HasOutputConnections)
            {
                if (_currentNodeData.OutputConnections[choice].To == null)
                {
                    EndDialogue();
                    return;
                }

                _currentNodeData = _currentNodeData.OutputConnections[choice].To;
                HandleNode(_currentNodeData);
                OnDialogueProgress?.Invoke(_currentNodeData);
                
                // all this in OnDialogueProgress callbacks
                // update ui
                // handle audio
                // handle command
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
            NodeHandlers.First(h => h.CanHandle(node)).Handle(node, this, _dialogueView);
        }
    }
}