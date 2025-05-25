namespace PotikotTools.DialogueSystem.Editor
{
    public class MultipleChoiceNodeView : NodeView<MultipleChoiceNodeData>
    {
        protected override string Title => "Choice Node";
        
        public override void Initialize(EditorNodeData editorData, NodeData data, DialogueGraphView graphView)
        {
            base.Initialize(editorData, data, graphView);
            this.AddUSSClasses("choice-node");
        }
    }
}