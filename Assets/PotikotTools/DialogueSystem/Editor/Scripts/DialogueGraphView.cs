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
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.Where(p => p.direction != startPort.direction && p.node != startPort.node).ToList();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Create Single Choice Node", AddNode<SingleChoiceNodeView, SingleChoiceNodeData>);
            evt.menu.AppendAction("Create Multiple Choice Node", AddNode<MultipleChoiceNodeView, MultipleChoiceNodeData>);
        }

        private void AddNode<TView, TData>(DropdownMenuAction dropdownMenuAction)
            where TView : Node, INodeView
            where TData : NodeData
        {
            TView nodeView = Activator.CreateInstance<TView>();
            nodeView.Initialize(dialogueData.AddNode<TData>());
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