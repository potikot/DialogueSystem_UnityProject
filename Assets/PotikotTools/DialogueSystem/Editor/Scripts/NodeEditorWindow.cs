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

        private bool _isSubbed;
        
        public EditorDialogueData EditorData
        {
            get => _editorData;
            set
            {
                if (value == null || _editorData == value)
                    return;
                
                _editorData = value;
                ChangeTitle(_editorData.Name);
                _editorData.OnNameChanged += ChangeTitle;
                _editorData.OnDeleted += Close;
                _isSubbed = true;
                AddGraphView();
                AddFloatingSettings();
            }
        }

        private void OnDestroy()
        {
            if (_isSubbed) // TODO: fix event sub on maximize window
            {
                _editorData.OnNameChanged -= ChangeTitle;
                _editorData.OnDeleted -= Close;
                _isSubbed = false;
            }
            
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
            DL.Log("ChangeTitle");
            titleContent = new GUIContent($"'{value}' Dialogue Editor");
        }
    }
}