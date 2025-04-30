using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem.Editor
{
    public class DialogueGraphView : GraphView
    {
        protected EditorDialogueData editorData;

        protected readonly Dictionary<Type, Type> nodeTypes = new()
        {
            { typeof(SingleChoiceNodeData), typeof(SingleChoiceNodeView) },
            { typeof(MultipleChoiceNodeData), typeof(MultipleChoiceNodeView) },
            { typeof(TimerNodeData), typeof(TimerNodeView) }
        };
        
        protected DialogueData RuntimeData => editorData.RuntimeData;

        public EditorDialogueData EditorData => editorData;
        
        public DialogueGraphView(EditorDialogueData editorDialogueData)
        {
            AddGridBackground();
            AddManipulators();
            AddNodes(editorDialogueData);

            this.AddStyleSheets("Styles/DialogueGraph");
            
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
                        if (nodeView is Node node)
                        {
                            node.inputContainer.Q<Port>().DisconnectAll();
                            node.outputContainer.Query<Port>().ForEach(p => p.DisconnectAll());
                        }
                        
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
            EditorNodeData editorData = new EditorNodeData()
            {
                position = dropdownMenuAction.eventInfo.mousePosition
            };
            
            EditorData.EditorNodeDataList.Add(editorData);
            
            nodeView.Initialize(editorData, RuntimeData.AddNode<TData>(dataArgs));
            nodeView.Draw();
            
            AddElement(nodeView);
        }

        private void AddNodes(EditorDialogueData editorDialogueData)
        {
            editorData = editorDialogueData;

            int nodesCount = editorData.EditorNodeDataList.Count;
            for (int i = 0; i < nodesCount; i++)
            {
                NodeData nodeData = editorData.RuntimeData.Nodes[i];
                INodeView nodeView = Activator.CreateInstance(nodeTypes[nodeData.GetType()]) as INodeView;
                nodeView.Initialize(editorData.EditorNodeDataList[i], nodeData);
                nodeView.Draw();

                Node node = nodeView as Node;
                AddElement(node);
            }
            
            foreach (Node fromNode in nodes)
            {
                INodeView fromNodeView = fromNode as INodeView;
                NodeData fromNodeData = fromNodeView.GetData();
                
                for (int i = 0; i < fromNodeData.OutputConnections.Count; i++)
                {
                    NodeData toNodeData = fromNodeData.OutputConnections[i].To;
                    if (toNodeData != null)
                    {
                        Node toNode = nodes.First(n => toNodeData.Id == (n as INodeView).GetData().Id);
                        
                        Port toPort = toNode.inputContainer.Q<Port>();
                        Port fromPort = fromNode.outputContainer.Query<Port>().ToList()[i];

                        AddElement(fromPort.ConnectTo(toPort));
                    }
                }
            }
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