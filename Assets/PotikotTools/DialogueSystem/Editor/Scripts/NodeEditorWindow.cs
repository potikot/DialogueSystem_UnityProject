using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PotikotTools.DialogueSystem.Editor
{
    public class NodeEditorWindow : EditorWindow
    {
        private DialogueGraphView _graph;

        private EditorDialogueData editorData
        {
            get => _graph.EditorData;
            set => AddGraphView(value);
        }

        public static void Open(EditorDialogueData editorData)
        {
            var window = GetWindow<NodeEditorWindow>("Dialogue Editor");
            window.editorData = editorData;
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

                await EditorDatabase.SaveDialogueAsync(editorData);

            }) { text = "Save Dialogue" });

            rootVisualElement.Add(c);
        }

        private void AddGraphView(EditorDialogueData editorData)
        {
            if (editorData == null)
                return;
            
            if (_graph != null)
                _graph.RemoveFromHierarchy();
            
            _graph = new DialogueGraphView(editorData);
            _graph.StretchToParentSize();
            
            rootVisualElement.Insert(0, _graph);
        }
    }
}