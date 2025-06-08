using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Editor
{
    public class DialogueGraphView : GraphView
    {
        public event Action OnChanged;
        
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
            editorData = editorDialogueData;

            AddGridBackground();
            AddManipulators();
            AddNodes();

            this.AddStyleSheets("Styles/DialogueGraph", "Styles/Variables");
            
            RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.ctrlKey && evt.keyCode == KeyCode.S)
                    EditorDialogueComponents.Database.SaveDialogue(editorData);
            });
            
            graphViewChanged += HandleGraphViewChanged;
            
            if (editorData.GraphViewPosition != Vector3.zero && editorData.GraphViewScale != Vector3.zero)
                UpdateViewTransform(editorData.GraphViewPosition, editorData.GraphViewScale);
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

        private void AddNode<TView, TData>(DropdownMenuAction dropdownMenuAction, params object[] dataArgs)
            where TView : Node, INodeView
            where TData : NodeData
        {
            TView nodeView = Activator.CreateInstance<TView>();
            EditorNodeData editorNodeData = new EditorNodeData()
            {
                position = this.ChangeCoordinatesTo(contentViewContainer, dropdownMenuAction.eventInfo.localMousePosition)
            };
            
            EditorData.EditorNodeDataList.Add(editorNodeData);
            
            nodeView.Initialize(editorNodeData, RuntimeData.AddNode<TData>(dataArgs), this);
            nodeView.OnChanged += Internal_OnGraphChanged;
            nodeView.Draw();

            AddElement(nodeView);
        }

        private void AddNodes()
        {
            int nodesCount = editorData.EditorNodeDataList.Count;
            for (int i = 0; i < nodesCount; i++)
            {
                NodeData nodeData = editorData.RuntimeData.Nodes[i];
                INodeView nodeView = (INodeView)Activator.CreateInstance(nodeTypes[nodeData.GetType()]);
                nodeView.Initialize(editorData.EditorNodeDataList[i], nodeData, this);
                nodeView.OnChanged += Internal_OnGraphChanged;
                nodeView.Draw();

                Node node = nodeView as Node;
                AddElement(node);
            }
            
            foreach (Node fromNode in nodes)
            {
                if (fromNode is not INodeView fromNodeView)
                    return;

                NodeData fromNodeData = fromNodeView.GetData();
                
                for (int i = 0; i < fromNodeData.OutputConnections.Count; i++)
                {
                    NodeData toNodeData = fromNodeData.OutputConnections[i].To;
                    if (toNodeData != null)
                    {
                        Node toNode = nodes.First(n => toNodeData.Id == ((INodeView)n).GetData().Id);
                        
                        Port toPort = toNode.inputContainer.Q<Port>();
                        Port fromPort = fromNode.outputContainer.Query<Port>().ToList()[i];

                        AddElement(fromPort.ConnectTo(toPort));
                    }
                }
            }
        }
        
        private void AddManipulators()
        {
            this.AddManipulator(new ContentZoomer { minScale = 0.1f, maxScale = 2f });
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }
        
        private void AddGridBackground()
        {
            GridBackground grid = new();
            grid.StretchToParentSize();
            
            Insert(0, grid);
        }

        private void Internal_OnGraphChanged()
        {
            OnChanged?.Invoke();
        }
        
        #region Callbacks
        
        protected virtual GraphViewChange HandleGraphViewChanged(GraphViewChange change)
        {
            // TODO: optimize algorithm

            CreateEdges(change.edgesToCreate);
            RemoveElements(change.elementsToRemove);

            return change;
        }

        private void CreateEdges(List<Edge> elements)
        {
            if (elements == null)
                return;
            
            foreach (Edge edge in elements)
            {
                if (!TryGetNodeData(edge, out var data))
                {
                    DL.LogError("Edge data could not be parsed");
                    continue;
                }
                
                data.from.OutputConnections[data.optionIndex].From = data.from;
                data.from.OutputConnections[data.optionIndex].To = data.to;
                data.to.InputConnection = data.from.OutputConnections[data.optionIndex];

                DL.Log($"Connected: {edge.output.node.title} -> {edge.input.node.title}");
            }
        }
        
        private void RemoveElements(List<GraphElement> elements)
        {
            if (elements == null)
                return;

            for (int i = 0; i < elements.Count; i++)
            {
                switch (elements[i])
                {
                    case Edge edge:
                    {
                        if (!TryGetNodeData(edge, out var data))
                        {
                            DL.LogError("Edge data could not be parsed");
                            return;
                        }

                        data.from.OutputConnections[data.optionIndex].To = null;
                        data.to.InputConnection = null;

                        DL.Log($"Disconnected: {edge.output.node.title} -> {edge.input.node.title}");
                        break;
                    }
                    case INodeView nodeView:
                    {
                        if (nodeView is Node node)
                        {
                            DeleteElements(node.inputContainer.Q<Port>().connections);
                            node.outputContainer.Query<Port>().ForEach(p => DeleteElements(p.connections));
                            RemoveElement(node);
                        }
                        
                        nodeView.OnChanged -= Internal_OnGraphChanged;
                        nodeView.OnDelete();
                        break;
                    }
                }
            }
        }

        private static bool TryGetNodeData(Edge edge, out (NodeData from, NodeData to, int optionIndex) data)
        {
            data = default;
            if (edge.output.node is not INodeView outputNodeView
                || edge.input.node is not INodeView inputNodeView)
                return false;

            NodeData from = outputNodeView.GetData();
            NodeData to = inputNodeView.GetData();

            List<Port> outputPorts = edge.output.node.outputContainer.Query<Port>().ToList();
            int i = outputPorts.IndexOf(edge.output);

            data.from = from;
            data.to = to;
            data.optionIndex = i;
            
            if (i < 0)
            {
                DL.LogError("Port index out of range");
                return false;
            }

            return true;
        }
        
        protected virtual void HandleDeleteSelection(string operationName, AskUser askuser)
        {
            foreach (var selectable in selection)
            {
                switch (selectable)
                {
                    case INodeView nodeView:
                        
                        break;
                    case Edge edge:
                        
                        break;
                }
            }
        }
        
        #endregion
    }
}