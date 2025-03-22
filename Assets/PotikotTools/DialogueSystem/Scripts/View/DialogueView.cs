using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class DialogueView : MonoBehaviour
    {
        [SerializeField] private DialogueController _controller;

        [SerializeField] private GameObject _container;

        [SerializeField] private TextMeshProUGUI _text;

        [SerializeField] private RectTransform _choiceContainer;
        [SerializeField] private ChoiceView _choiceViewPrefab;

        private List<ChoiceView> _choices;

        public bool IsEnabled { get; private set; }
        
        public void Show()
        {
            if (IsEnabled) return;
            IsEnabled = true;
            
            _container.SetActive(true);
        }

        public void Hide()
        {
            if (!IsEnabled) return;
            IsEnabled = false;
            
            _container.SetActive(false);
        }

        public void SetController(DialogueController dialogueController)
        {
            RemoveController();
            
            _controller = dialogueController;
            
            _controller.OnDialogueStarted += OnDialogueStarted;
            _controller.OnDialogueEnded += OnDialogueEnded;
            _controller.OnDialogueProgress += OnDialogueProgress;
        }

        public void RemoveController()
        {
            if (_controller == null)
                return;

            _controller.OnDialogueStarted -= OnDialogueStarted;
            _controller.OnDialogueEnded -= OnDialogueEnded;
            _controller.OnDialogueProgress -= OnDialogueProgress;
        }

        private void GenerateChoices(NodeData nodeData)
        {
            int choicesCount = nodeData.OutputConnections.Count;
            int i = 0;

            for (; i < choicesCount; i++)
            {
                if (_choices.Count <= i)
                    _choices.Add(Instantiate(_choiceViewPrefab, _choiceContainer));
                
                _choices[i].Show();
                _choices[i].SetData(nodeData.OutputConnections[i]);
            }

            for (int j = i; j < _choices.Count; j++)
            {
                _choices[j].Hide();
            }
        }
        
        private void OnDialogueStarted()
        {
            Show();
        }

        private void OnDialogueEnded()
        {
            Hide();
        }

        private void OnDialogueProgress(NodeData nodeData)
        {
            _text.text = nodeData.Text;
            GenerateChoices(nodeData);
        }
    }
}