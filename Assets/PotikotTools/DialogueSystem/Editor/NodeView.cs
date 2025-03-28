using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem
{
    public abstract class NodeView<T> : Node where T : NodeData
    {
        protected T nodeData;

        public virtual void Initialize(NodeData nodeData)
        {
            this.nodeData = nodeData as T;

            title = "Dialogue Node";

            inputContainer.Add(CreatePort("Input", Direction.Input, Port.Capacity.Multi));
            outputContainer.Add(CreatePort(nodeData.OutputConnections[0].Text, Direction.Output, Port.Capacity.Single));
            
            // mainContainer.style.backgroundColor = Color.gray;
            mainContainer.Add(CreateButtons());
            
            AddManipulators();
        }

        protected virtual VisualElement CreateButtons()
        {
            VisualElement c = new();
            c.style.flexDirection = FlexDirection.Row;
            c.style.backgroundColor = Color.gray;

            c.Add(new Button()
            {
                text = "Add",
            });
            c.Add(new Button()
            {
                text = "Remove",
            });
            
            return c;
        }
        
        protected virtual VisualElement CreatePort(string text, Direction direction, Port.Capacity capacity)
        {
            VisualElement c = new();
            c.style.flexDirection = direction == Direction.Input ? FlexDirection.Row : FlexDirection.RowReverse;

            c.Add(InstantiatePort(Orientation.Horizontal, direction, capacity, null));
            c.Add(new Label(text)
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            });
            
            return c;
        }
        
        protected virtual void AddManipulators()
        {
            this.AddManipulator(new Dragger());
        }
    }
}