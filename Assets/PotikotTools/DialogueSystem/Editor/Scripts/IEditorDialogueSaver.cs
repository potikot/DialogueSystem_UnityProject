using System.Threading.Tasks;

namespace PotikotTools.DialogueSystem
{
    public interface IEditorDialogueSaver : IDialogueSaver, IDialogueLoader
    {
        public Task<bool> SaveEditorDataAsync(string directoryPath, EditorDialogueData editorDialogueData);
        public bool SaveEditorData(string directoryPath, EditorDialogueData editorDialogueData);

        public Task<EditorDialogueData> LoadEditorDataAsync(string directoryPath, string dialogueId);
        public EditorDialogueData LoadEditorData(string directoryPath, string dialogueId);
    }
}