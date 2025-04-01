using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem
{
    public class SingleChoiceNodeView : NodeView<SingleChoiceNodeData>
    {
        protected override void CreateAddButton() { }

        protected override void AddOutputPort(ConnectionData connectionData)
        {
            // if (outputContainer.childCount >= 1)
            //     return;

            VisualElement c = new()
            {
                style =
                {
                    flexDirection = FlexDirection.RowReverse
                }
            };

            c.Add(InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null));
            c.Add(new Label("Out")
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            });
            
            outputContainer.Add(c);
        }
    }
}