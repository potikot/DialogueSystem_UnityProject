using System.Threading.Tasks;

namespace PotikotTools.DialogueSystem
{
    public interface IDialogueSaver
    {
        bool Save(string directoryPath, DialogueData dialogueData);
        Task<bool> SaveAsync(string directoryPath, DialogueData dialogueData);
    }

    public interface IDialogueLoader
    {
        DialogueData Load(string directory, string dialogueId);
        Task<DialogueData> LoadAsync(string directoryPath, string dialogueId);
    }
}