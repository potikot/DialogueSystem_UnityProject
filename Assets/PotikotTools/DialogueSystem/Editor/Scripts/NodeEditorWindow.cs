using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem.Editor
{
    public class NodeEditorWindow : EditorWindow
    {
        public event Action<NodeEditorWindow> OnClose;

        private EditorDialogueData _editorData;
        
        private DialogueGraphView _graph;
        private FloatingSettingsPanel _floatingSettngsPanel;

        public EditorDialogueData EditorData
        {
            get => _editorData;
            set
            {
                if (value == null || _editorData == value)
                    return;
                
                _editorData = value;
                ChangeTitle(_editorData.Id);
                _editorData.OnNameChanged += ChangeTitle;
                AddGraphView();
            }
        }

        private void OnDestroy()
        {
            _editorData.OnNameChanged -= ChangeTitle;
            OnClose?.Invoke(this);
        }

        private void CreateGUI()
        {
            var c = new VisualElement()
                .AddStyleSheets("Styles/NodeEditorWindow")
                .AddUSSClasses("body");

            c.Add(new Button(async () =>
            {
                if (_graph == null)
                    return;

                await EditorComponents.Database.SaveDialogueAsync(EditorData);

            }) { text = "Save Dialogue" });

            rootVisualElement.Add(c);
            AddFloatingSettings();
        }

        private void AddGraphView()
        {
            if (_editorData == null)
                return;

            _graph?.RemoveFromHierarchy();
            _graph = new DialogueGraphView(_editorData);
            _graph.StretchToParentSize();
            
            rootVisualElement.Insert(0, _graph);
        }

        private void AddFloatingSettings()
        {
            if (_editorData == null)
                return;

            _floatingSettngsPanel?.RemoveFromHierarchy();
            _floatingSettngsPanel = new FloatingSettingsPanel(_editorData);
            
            rootVisualElement.Add(_floatingSettngsPanel);
        }

        private void ChangeTitle(string value)
        {
            titleContent = new GUIContent($"'{value}' Dialogue Editor");
        }
    }
}