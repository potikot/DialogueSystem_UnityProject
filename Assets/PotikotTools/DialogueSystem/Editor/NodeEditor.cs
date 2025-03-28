using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem
{
    public class NodeEditor : EditorWindow
    {
        [MenuItem("Tools/DialogueSystem/NodeEditor")]
        public static void Open()
        {
            GetWindow<NodeEditor>("Dialogue Editor");
        }

        private void CreateGUI()
        {
            AddGraphView();
        }

        private void AddGraphView()
        {
            DialogueGraphView graph = new();
            graph.StretchToParentSize();

            rootVisualElement.Add(graph);
        }
    }
}