namespace PotikotTools.DialogueSystem
{
    public interface IDialogueSaver
    {
        void Save(DialogueData dialogueData);
        DialogueData Load(string dialogueId);
    }
}