namespace PotikotTools.DialogueSystem.Demo
{
    public class ChatDialogueController : DialogueController
    {
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