using UnityEngine;

namespace PotikotTools.DialogueSystem.Editor
{
    public class DialogueTestWindowsManager : WindowsManager<DialogueTestWindow> { }
    
    public class DialogueTestWindow : BaseDialogueSystemEditorWindow
    {
        private EditorDialogueView _dialogueView;
        private DialogueController _dialogueController;
        
        protected override void OnEditorDataChanged()
        {
            AddDialogueView();
            CreateDialogueController();
            
            _dialogueController.StartDialogue();
        }
        
        protected override void ChangeTitle(string value)
        {
            titleContent = new GUIContent($"'{value}' Dialogue Test");
        }
        
        private void CreateGUI()
        {
            rootVisualElement.AddStyleSheets(
                "Styles/Variables",
                "Styles/DialogueTestEditorWindow"
            );
        }

        private void AddDialogueView()
        {
            _dialogueView?.RemoveFromHierarchy();
            _dialogueView = new EditorDialogueView();
            rootVisualElement.Add(_dialogueView);
        }
        
        private void CreateDialogueController()
        {
            _dialogueController = new DialogueController();
            _dialogueController.Initialize(editorData.RuntimeData, _dialogueView);
        }
    }
}