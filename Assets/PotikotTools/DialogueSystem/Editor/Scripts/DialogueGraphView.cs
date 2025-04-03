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
            
            this.AddStyles("DialogueGraph");
            
            graphViewChanged += HandleGraphViewChanged;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.Where(p => p.direction != startPort.direction && p.node != startPort.node).ToList();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Create Single Choice Node", a => AddNode<SingleChoiceNodeView, SingleChoiceNodeData>(a));
            evt.menu.AppendAction("Create Multiple Choice Node", a => AddNode<MultipleChoiceNodeView, MultipleChoiceNodeData>(a));
            evt.menu.AppendAction("Create Timer Node", a => AddNode<TimerNodeView, TimerNodeData>(a, 5f));
        }

        private GraphViewChange HandleGraphViewChanged(GraphViewChange change)
        {
            // TODO: optimize algorithm

            if (change.edgesToCreate != null)
            {
                foreach (Edge edge in change.edgesToCreate)
                {
                    NodeData from = (edge.output.node as INodeView).GetData();
                    NodeData to = (edge.input.node as INodeView).GetData();

                    List<Port> outputPorts = edge.output.node.outputContainer.Query<Port>().ToList();
                    int i = 0;
                    foreach (var port in outputPorts)
                    {
                        if (port == edge.output)
                            break;

                        i++;
                    }

                    from.OutputConnections[i].To = to;
                    to.InputConnection = from.OutputConnections[i];

                    DL.Log($"Connected: {edge.output.node.title} -> {edge.input.node.title}");
                }
            }

            if (change.elementsToRemove != null)
            {
                foreach (GraphElement element in change.elementsToRemove)
                {
                    if (element is Edge edge)
                    {
                        NodeData from = (edge.output.node as INodeView).GetData();
                        NodeData to = (edge.input.node as INodeView).GetData();

                        List<Port> outputPorts = edge.output.node.outputContainer.Query<Port>().ToList();
                        int i = 0;
                        foreach (var port in outputPorts)
                        {
                            if (port == edge.output)
                                break;

                            i++;
                        }
                        
                        from.OutputConnections[i].To = null;
                        to.InputConnection = null;
                        
                        DL.Log($"Disconnected: {edge.output.node.title} -> {edge.input.node.title}");
                    }
                    else if (element is INodeView nodeView)
                    {
                        nodeView.Delete();
                    }
                }
            }

            return change;
        }
        
        private void AddNode<TView, TData>(DropdownMenuAction dropdownMenuAction, params object[] dataArgs)
            where TView : Node, INodeView
            where TData : NodeData
        {
            TView nodeView = Activator.CreateInstance<TView>();
            nodeView.Initialize(dialogueData.AddNode<TData>(dataArgs));
            nodeView.Draw();

            AddElement(nodeView);
            nodeView.SetPosition(new Rect(dropdownMenuAction.eventInfo.mousePosition, Vector2.zero));
        }
        
        private void AddManipulators()
        {
            this.AddManipulator(new ContentZoomer() { maxScale = 2f });
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