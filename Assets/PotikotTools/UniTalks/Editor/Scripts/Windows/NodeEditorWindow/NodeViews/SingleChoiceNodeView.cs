using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Editor
{
    public class SingleChoiceNodeView : NodeView<SingleChoiceNodeData>
    {
        protected override string Title => "No Choice Node";

        protected override void AddOutputPort(ConnectionData connectionData)
        {
            VisualElement container = new()
            {
                style =
                {
                    flexDirection = FlexDirection.RowReverse
                }
            };

            container.Add(InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null));
            container.Add(new Label("Out")
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            });
            
            outputContainer.Add(container);
        }
        
        protected override void DrawChoiceButton() { }
    }
}