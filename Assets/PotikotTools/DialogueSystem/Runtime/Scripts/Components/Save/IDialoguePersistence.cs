using System.Collections.Generic;
using System.Threading.Tasks;

namespace PotikotTools.DialogueSystem
{
    public interface IDialoguePersistence : IPersistence<DialogueData>
    {
        List<string> LoadTags(string directoryPath, string id);
        Task<List<string>> LoadTagsAsync(string directoryPath, string id);
    }
}