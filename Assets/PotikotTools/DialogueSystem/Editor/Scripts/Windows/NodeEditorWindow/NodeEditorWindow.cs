using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem.Editor
{
    public class NodeEditorWindowsManager : WindowsManager<NodeEditorWindow> { }

    public class NodeEditorWindow : BaseDialogueSystemEditorWindow
    {
        private DialogueGraphView _graph;
        private SettingsPanel _settngsPanel;

        protected override void OnEditorDataChanged()
        {
            AddGraphView();
            AddFloatingSettings();
        }

        protected override void ChangeTitle(string value)
        {
            titleContent = new GUIContent($"'{value}' Dialogue Editor");
        }

        private void CreateGUI()
        {
            var c = new VisualElement()
                .AddStyleSheets("Styles/NodeEditorWindow")
                .AddUSSClasses("body");

            c.Add(new Button(() =>
            {
                if (_graph == null)
                    return;

                editorData.GraphViewPosition = _graph.contentViewContainer.transform.position;
                editorData.GraphViewScale = _graph.contentViewContainer.transform.scale;

                SaveChanges();

            }) { text = "Save Dialogue" });

            rootVisualElement.Add(c);
        }

        private void AddGraphView()
        {
            if (editorData == null)
                return;

            if (_graph != null)
            {
                _graph.RemoveFromHierarchy();
                _graph.OnChanged -= OnGraphChanged;
            }
            
            _graph = new DialogueGraphView(editorData);
            _graph.OnChanged += OnGraphChanged;
            _graph.StretchToParentSize();
            
            rootVisualElement.Insert(0, _graph);
        }

        private void OnGraphChanged()
        {
            hasUnsavedChanges = true;
        }

        private void AddFloatingSettings()
        {
            if (editorData == null)
                return;

            _settngsPanel?.RemoveFromHierarchy();
            _settngsPanel = new SettingsPanel(editorData);
            
            rootVisualElement.Add(_settngsPanel);
        }

        public override void DiscardChanges()
        {
            // TODO: discard changes
            
            hasUnsavedChanges = false;
        }

        public override void SaveChanges()
        {
            EditorComponents.Database.SaveDialogue(EditorData);
            hasUnsavedChanges = false;
        }
    }
}