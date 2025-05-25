using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem.Editor
{
    public class EditorDialogueView : VisualElement, IDialogueView
    {
        private List<VisualElement> _menus;
        
        private Label _label;

        private VisualElement _optionsContainer;
        private List<EditorOptionView> _optionViews;
        
        private Action<int> _onOptionSelected;
        
        public bool IsEnabled { get; private set; }

        public EditorDialogueView()
        {
            _menus = new List<VisualElement>();
            _optionViews = new List<EditorOptionView>();

            this.AddUSSClasses("dialogue-view");

            _label = new Label("Dialogue Text");
            Add(_label);

            _optionsContainer = new VisualElement().AddUSSClasses("option-view-container");
            Add(_optionsContainer);
        }
        
        public void Show()
        {
            if (IsEnabled) return;
            IsEnabled = true;
            
            style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            if (!IsEnabled) return;
            IsEnabled = false;
            
            style.display = DisplayStyle.None;
        }

        public void SetSpeakerText(string text)
        {
            _label.text = $"<color=red>Dialogue Text:</color> {text}";
        }

        public void SetAnswerOptions(string[] options)
        {
            if (options == null || options.Length == 0)
            {
                RemoveOptions();
                return;
            }
            
            GenerateOptions(options);
        }

        public void OnOptionSelected(Action<int> callback)
        {
            _onOptionSelected = callback;
        }

        public T GetMenu<T>()
        {
            var m = _menus.FirstOrDefault(m => m is T);
            if (m is T cm)
                return cm;
            
            return default;
        }

        protected virtual void GenerateOptions(string[] options)
        {
            int optionsCount = options.Length;
            int i = 0;

            for (; i < optionsCount; i++)
            {
                if (_optionViews.Count <= i)
                {
                    var optionView = new EditorOptionView();
                    _optionViews.Add(optionView);
                    _optionsContainer.Add(optionView);
                }
                
                int optionIndex = i;
                _optionViews[i].OnSelected(() => _onOptionSelected?.Invoke(optionIndex));
                _optionViews[i].SetText(options[i]);
                _optionViews[i].Show();
            }

            for (int j = i; j < _optionViews.Count; j++)
            {
                _optionViews[j].OnSelected(null);
                _optionViews[j].Hide();
            }
        }
        
        protected virtual void RemoveOptions()
        {
            _optionViews.RemoveAll(ov =>
            {
                ov.RemoveFromHierarchy();
                return true;
            });
            
            _optionViews.Clear();
        }
    }
}