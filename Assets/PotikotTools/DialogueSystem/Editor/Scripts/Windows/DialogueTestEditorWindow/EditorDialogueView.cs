using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem.Editor
{
    public class EditorDialogueView : VisualElement, IEditorDialogueView
    {
        private List<VisualElement> _menus;
        
        private Label _textLabel;
        private Label _speakerLabel;

        private VisualElement _optionsContainer;
        private List<EditorOptionView> _optionViews;
        
        private Action<int> _onOptionSelected;

        private VisualElement _historyContainer;
        private VisualElement _previewContainer;
        private VisualElement _infoContainer;

        private NodeData _data;
        
        public bool IsEnabled { get; private set; }

        public EditorDialogueView()
        {
            _menus = new List<VisualElement>();
            _optionViews = new List<EditorOptionView>();

            this.AddUSSClasses("dialogue-view");

            _historyContainer = new ScrollView().AddUSSClasses("dialogue-history-container");
            _previewContainer = new VisualElement().AddUSSClasses("dialogue-preview-container");
            _infoContainer = new ScrollView().AddUSSClasses("dialogue-info-container");
            
            Add(_historyContainer);
            Add(_previewContainer);
            Add(_infoContainer);
        }

        public void Rebuild()
        {
            CreatePreviewContainer();
            CreateInfoContainer();
        }
        
        public void SetData(NodeData data)
        {
            _data = data;
            CreatePreviewContainer();
            CreateInfoContainer();
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

        public void SetSpeaker(SpeakerData speakerData)
        {
            if (speakerData != null)
                _speakerLabel.text = speakerData.Name;
        }
        
        public void SetText(string text)
        {
            _textLabel.text = text;
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
                
                int optionIdx = i;
                _optionViews[i].OnSelected(() => _onOptionSelected?.Invoke(optionIdx));
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
            _optionsContainer.Clear();
            _optionViews.Clear();
        }
        
        private void CreatePreviewContainer()
        {
            _previewContainer.Clear();
            _optionViews.Clear();
            
            _speakerLabel = new Label(_data == null ? "Speaker" : _data.GetSpeakerName()).AddUSSClasses("speaker-label");
            _textLabel = new Label(_data == null ? "Dialogue Text" : _data.Text).AddUSSClasses("text-label");
            _optionsContainer = new ScrollView().AddUSSClasses("option-view-container");
            
            if (_data != null)
                GenerateOptions(_data.OutputConnections.Select(o => o.Text).ToArray());

            var textContainer = new VisualElement().AddUSSClasses("text-container");

            textContainer.Add(_speakerLabel);
            textContainer.Add(_textLabel);

            _previewContainer.Add(textContainer);
            _previewContainer.Add(_optionsContainer);
        }

        private void CreateInfoContainer()
        {
            _infoContainer.Clear();
            if (_data == null)
                return;

            InspectorUtility.CreateInspectorWindow(_data, _infoContainer);
        }
    }
}