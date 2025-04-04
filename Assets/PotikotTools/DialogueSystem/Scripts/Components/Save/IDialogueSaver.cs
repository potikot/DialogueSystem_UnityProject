namespace PotikotTools.DialogueSystem
{
    public interface IDialogueSaver
    {
        void Save(string directoryPath, DialogueData dialogueData);
        DialogueData Load(string directory, string dialogueId);
    }
}