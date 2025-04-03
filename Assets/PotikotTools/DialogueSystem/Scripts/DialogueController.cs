using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class DialogueController
    {
        public event Action OnDialogueStarted;
        public event Action OnDialogueEnded;
        public event Action<NodeData> OnDialogueProgress;
        
        private DialogueData _currentDialogueData;

        private NodeData _currentNodeData;

        public bool IsDialogueStarted { get; private set; }
        public Dictionary<Type, Action<NodeData>> NodeHandlers = new Dictionary<Type, Action<NodeData>>();

        public void SetDialogueData(DialogueData dialogueData)
        {
            if (dialogueData == null)
            {
                DL.LogError("Dialogue Data is null");
                return;
            }
            if (IsDialogueStarted)
                EndDialogue();

            _currentDialogueData = dialogueData;
        }

        public void StartDialogue()
        {
            if (IsDialogueStarted)
            {
                DL.LogError("Dialogue is already started");
                return;
            }
            if (_currentDialogueData == null)
            {
                DL.LogError("Dialogue Data is null");
                return;
            }

            IsDialogueStarted = true;
            _currentNodeData = _currentDialogueData.GetFirstNode();
            
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
            
            IsDialogueStarted = false;
            OnDialogueEnded?.Invoke();
        }
        
        public void Next(int choice = 0)
        {
            if (!IsDialogueStarted)
                StartDialogue();

            if (_currentNodeData.HasOutputConnections)
            {
                _currentNodeData = _currentNodeData.OutputConnections[choice].To;
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
    }
}