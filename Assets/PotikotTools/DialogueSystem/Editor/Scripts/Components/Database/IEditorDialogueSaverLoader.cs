using System.Threading.Tasks;

namespace PotikotTools.DialogueSystem.Editor
{
    public interface IEditorDialogueSaverLoader : IDialogueSaver, IDialogueLoader
    {
        public Task<bool> SaveEditorDataAsync(string directoryPath, EditorDialogueData editorDialogueData, bool refreshAsset = true);
        public bool SaveEditorData(string directoryPath, EditorDialogueData editorDialogueData, bool refreshAsset = true);

        public Task<EditorDialogueData> LoadEditorDataAsync(string directoryPath, string dialogueId);
        public EditorDialogueData LoadEditorData(string directoryPath, string dialogueId);
    }
}