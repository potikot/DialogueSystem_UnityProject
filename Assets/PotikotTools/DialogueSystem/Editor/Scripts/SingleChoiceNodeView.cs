using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem.Editor
{
    public class SingleChoiceNodeView : NodeView<SingleChoiceNodeData>
    {
        public override void Draw()
        {
            base.Draw();
            title = "Single Choice Node";
        }
        
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