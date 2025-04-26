using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PotikotTools.DialogueSystem.Editor
{
    public class NodeEditor : EditorWindow
    {
        private DialogueGraphView _graph;
        
        private string _dialogueId_TEMP = "Test Dialogue Graph";
        
        [MenuItem("Tools/DialogueSystem/Node Editor")]
        public static void Open()
        {
            GetWindow<NodeEditor>("Dialogue Editor");
        }

        private void CreateGUI()
        {
            VisualElement c = new()
            {
                style =
                {
                    backgroundColor = new Color(0.2470588f, 0.2470588f, 0.2470588f),
                    paddingTop = 5f,
                    paddingBottom = 5f,
                    paddingLeft = 5f,
                    paddingRight = 5f
                }
            };
            
            TextField textField = new() { value = _dialogueId_TEMP };
            textField.RegisterValueChangedCallback(evt => _dialogueId_TEMP = evt.newValue);
            c.Add(textField);
            
            c.Add(new Button(async () =>
            {
                if (_graph == null)
                    return;
                
                await EditorDatabase.SaveDialogueAsync(_graph.EditorData);
                
            }) { text = "Save Dialogue" });
            c.Add(new Button(async () =>
            {
                AddGraphView(await EditorDatabase.LoadDialogueAsync(_dialogueId_TEMP));
                
            }) { text = "Load Dialogue" });
            c.Add(new Button(() =>
            {
                _graph = null;
                _dialogueId_TEMP = "Test Dialogue Graph";
                
                rootVisualElement.Clear();
                CreateGUI();
                
            }) { text = "Reload" });
            
            rootVisualElement.Add(c);
        }

        private void AddGraphView(EditorDialogueData editorData)
        {
            if (_graph != null)
                _graph.RemoveFromHierarchy();
            
            _graph = new DialogueGraphView(editorData);
            _graph.StretchToParentSize();

            rootVisualElement.Insert(0, _graph);
        }
    }
}