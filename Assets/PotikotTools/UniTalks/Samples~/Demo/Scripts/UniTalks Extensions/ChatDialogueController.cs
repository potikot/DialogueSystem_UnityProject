namespace PotikotTools.UniTalks.Demo
{
    public class ChatDialogueController : DialogueController
    {
        public bool IsDialogueStopped { get; private set; }

        public override void StartDialogue()
        {
            if (IsDialogueStarted && !IsDialogueStopped)
            {
                DL.LogError("Dialogue is already started");
                return;
            }
            if (currentDialogueData == null)
            {
                DL.LogError("Dialogue Data is null");
                return;
            }
            
            IsDialogueStarted = true;
            currentDialogueView.Show();
            currentNodeData = currentDialogueData.GetFirstNode();

            if (currentNodeData == null)
            {
                DL.LogError($"Dialogue graph '{currentDialogueData.Name}' is empty");
                return;
            }
            
            if (!IsDialogueStopped)
                HandleNode(currentNodeData);
            
            IsDialogueStopped = false;
        }

        public override void EndDialogue()
        {
            base.EndDialogue();
            currentDialogueView.Show();
        }
        
        public void StopDialogue()
        {
            currentDialogueView.Hide();
            IsDialogueStopped = true;
        }
        
        protected override void SetDialogueData(DialogueData dialogueData)
        {
            base.SetDialogueData(dialogueData);

            if (currentDialogueData != null
                && currentDialogueView is ChatDialogueView cv
                && currentDialogueData.TryGetSpeaker(0, out var speaker)
                && speaker is ChatSpeakerData cs)
            {
                cv.SetAvatarImage(cs.Avatar);
            }
        }
    }
}