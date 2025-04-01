using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem
{
    public class NodeEditor : EditorWindow
    {
        private DialogueData _dialogueData;
        
        [MenuItem("Tools/DialogueSystem/Node Editor")]
        public static void Open()
        {
            GetWindow<NodeEditor>("Dialogue Editor");
        }

        private void OnEnable()
        {
            _dialogueData = new DialogueData("Test Dialogue Graph")
            {
                Speakers = new List<SpeakerData>
                {
                    new("Andrew"), new("Lox")
                }
            };
        }
        
        private void CreateGUI()
        {
            AddGraphView();
            
            VisualElement c = new();
            
            c.Add(new Button(() => Components.Saver.Save(_dialogueData))
            {
                text = "Save Dialogue"
            });
            c.Add(new Button(() => Components.Saver.Load(_dialogueData.Id))
            {
                text = "Load Dialogue"
            });
            
            rootVisualElement.Add(c);
        }

        private void AddGraphView()
        {
            DialogueGraphView graph = new(_dialogueData);
            graph.StretchToParentSize();

            rootVisualElement.Add(graph);
        }
    }
}