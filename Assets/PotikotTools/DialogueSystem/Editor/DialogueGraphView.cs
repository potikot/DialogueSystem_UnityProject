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
        public DialogueGraphView()
        {
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
            evt.menu.AppendAction("Create Node", action =>
            {
                SingleChoiceNodeView nodeView = new SingleChoiceNodeView();
                nodeView.Initialize(new SingleChoiceNodeData(0));

                AddElement(nodeView);
                nodeView.SetPosition(new Rect(action.eventInfo.mousePosition, Vector2.zero));
            });
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