using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem
{
    public class SingleChoiceNodeView : NodeView<SingleChoiceNodeData>
    {
        protected override void AddOutputPort(string text)
        {
            if (outputContainer.childCount >= 1)
                return;

            base.AddOutputPort(text);
            
            outputContainer.Q<Button>().style.display = DisplayStyle.None;
        }
    }
}