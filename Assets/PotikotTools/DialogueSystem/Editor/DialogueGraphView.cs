using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem
{
    public class DialogueGraphView : GraphView
    {
        protected DialogueData dialogueData;
        
        public DialogueGraphView(DialogueData dialogueData)
        {
            this.dialogueData = dialogueData;
            
            AddGridBackground();
            AddManipulators();
            
            graphViewChanged += change =>
            {
                foreach (var edge in change.edgesToCreate)
                {
                    var from = edge.output.node as NodeView<SingleChoiceNodeData>;
                    var to = edge.input.node as NodeView<SingleChoiceNodeData>;
                    
                    to.Data.InputConnection = new ConnectionData("", from.Data, to.Data);
                    from.Data.OutputConnections[0] = new ConnectionData("", from.Data, to.Data);
                }

                return change;
            };
            
            this.AddStyles("DialogueGraph");
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.Where(p => p.direction != startPort.direction && p.node != startPort.node).ToList();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Create Single Choice Node", AddSingleChoiceNode);
            evt.menu.AppendAction("Create Multiple Choice Node", AddMultipleChoiceNode);
        }

        // TODO: write universal method for node view creation
        
        private void AddSingleChoiceNode(DropdownMenuAction dropdownMenuAction)
        {
            var nodeData = dialogueData.AddNode<SingleChoiceNodeData>();

            nodeData.Commands.Add(new CommandData());

            SingleChoiceNodeView nodeView = new();
            nodeView.Initialize(nodeData);
            nodeView.Draw();

            AddElement(nodeView);
            nodeView.SetPosition(new Rect(dropdownMenuAction.eventInfo.mousePosition, Vector2.zero));
        }
        
        private void AddMultipleChoiceNode(DropdownMenuAction dropdownMenuAction)
        {
            var nodeData = dialogueData.AddNode<MultipleChoiceNodeData>();
            nodeData.OutputConnections.Add(new ConnectionData("Choice 2", nodeData, null));
            nodeData.OutputConnections.Add(new ConnectionData("Choice 3", nodeData, null));

            nodeData.Commands.Add(new CommandData());

            MultipleChoiceNodeView nodeView = new();
            nodeView.Initialize(nodeData);
            nodeView.Draw();

            AddElement(nodeView);
            nodeView.SetPosition(new Rect(dropdownMenuAction.eventInfo.mousePosition, Vector2.zero));
        }
        
        private void AddManipulators()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new RectangleSelector());
        }
        
        private void AddGridBackground()
        {
            GridBackground grid = new();
            grid.StretchToParentSize();
            
            Insert(0, grid);
        }
    }
}