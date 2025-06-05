namespace PotikotTools.UniTalks.Editor
{
    public class EditorDialogueController : DialogueController
    {
        public override void EndDialogue()
        {
            base.EndDialogue();
            StartDialogue();
        }

        protected override void HandleNode(NodeData node)
        {
            if (currentDialogueView is EditorDialogueView edv)
                edv.SetData(currentNodeData);
            
            base.HandleNode(node);
        }
    }
}