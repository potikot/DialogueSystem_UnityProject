using System.Collections.Generic;
using System.Threading.Tasks;

namespace PotikotTools.DialogueSystem
{
    public interface IDialogueSaver
    {
        bool SaveData(string directoryPath, DialogueData dialogueData, bool refreshAsset = true);
        Task<bool> SaveDataAsync(string directoryPath, DialogueData dialogueData, bool refreshAsset = true);
    }

    public interface IDialogueLoader
    {
        DialogueData LoadData(string directory, string dialogueId);
        Task<DialogueData> LoadDataAsync(string directoryPath, string dialogueId);
        
        List<string> LoadTags(string directory, string dialogueId);
        Task<List<string>> LoadTagsAsync(string directoryPath, string dialogueId);
    }
}