using PotikotTools.UniTalks.Editor;

namespace PotikotTools.UniTalks.Demo.Editor
{
    public class ChatNodeView : NodeView<ChatNodeData>
    {
        protected override string Title => "Chat Node";
        protected override bool CanBeZeroOptions => true;

        public override void Initialize(EditorNodeData editorData, NodeData data, DialogueGraphView graphView)
        {
            base.Initialize(editorData, data, graphView);
            this.AddUSSClasses("choice-node");
        }
    }
}
