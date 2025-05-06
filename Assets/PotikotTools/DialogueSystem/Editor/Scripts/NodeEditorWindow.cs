using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem.Editor
{
    public class NodeEditorWindow : EditorWindow
    {
        public event Action<NodeEditorWindow> OnClose;
        
        private DialogueGraphView _graph;

        public EditorDialogueData EditorData
        {
            get => _graph.EditorData;
            set => AddGraphView(value);
        }

        private void OnDestroy()
        {
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
        }

        private void AddGraphView(EditorDialogueData editorData)
        {
            if (editorData == null)
                return;

            _graph?.RemoveFromHierarchy();
            _graph = new DialogueGraphView(editorData);
            _graph.StretchToParentSize();
            
            rootVisualElement.Insert(0, _graph);
        }
    }
}