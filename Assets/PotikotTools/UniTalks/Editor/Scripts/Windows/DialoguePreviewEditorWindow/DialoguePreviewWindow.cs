using UnityEngine;

namespace PotikotTools.UniTalks.Editor
{
    public class DialoguePreviewWindowsManager : UniTalksWindowsManager<DialoguePreviewWindow> { }
    
    public class DialoguePreviewWindow : BaseUniTalksEditorWindow
    {
        private EditorDialogueView _dialogueView;
        private EditorDialogueController _dialogueController;

        private bool _isVisible;
        private bool _isDialogueDataChanged;

        private void OnBecameVisible()
        {
            _isVisible = true;

            if (_isDialogueDataChanged)
            {
                _dialogueView.Rebuild();
                _isDialogueDataChanged = false;
            }
        }

        private void OnBecameInvisible()
        {
            _isVisible = false;
        }
        
        private void CreateGUI()
        {
            rootVisualElement.AddStyleSheets(
                "Styles/Variables",
                "Styles/DialoguePreviewEditorWindow"
            );
        }

        private void OnEnable()
        {
            // DL.Log("Enabling dialogue editor window");
        }

        private void OnDisable()
        {
            EditorData?.RuntimeData?.ReleaseResources();

            // DL.Log("Disabling dialogue editor window");
            if (_dialogueController?.CurrentDialogueData == null)
                return;
            
            _dialogueController.CurrentDialogueData.OnChanged -= OnDialogueDataChanged;
            _dialogueController.OnDialogueDataChanged -= OnControllerDialogueDataChanged;
        }

        protected override void OnEditorDataChanged()
        {
            AddDialogueView();
            CreateDialogueController();

            EditorData.RuntimeData.LoadResources();
            _dialogueController.StartDialogue();
        }
        
        protected override void ChangeTitle(string value)
        {
            titleContent = new GUIContent($"'{value}' Dialogue Preview");
        }
        
        private void AddDialogueView()
        {
            _dialogueView?.RemoveFromHierarchy();
            _dialogueView = new EditorDialogueView();
            rootVisualElement.Add(_dialogueView);
        }

        private void CreateDialogueController()
        {
            _dialogueController = new EditorDialogueController();
            _dialogueController.Initialize(editorData.RuntimeData, _dialogueView);
            _dialogueController.CurrentDialogueData.OnChanged += OnDialogueDataChanged;
            _dialogueController.OnDialogueDataChanged += OnControllerDialogueDataChanged;
        }

        private void OnDialogueDataChanged()
        {
            if (_isVisible)
                _dialogueView.Rebuild();
            else
                _isDialogueDataChanged = true;
        }
        
        private void OnControllerDialogueDataChanged(DialogueData prevValue, DialogueData newValue)
        {
            _isDialogueDataChanged = false;
            prevValue.OnChanged -= OnDialogueDataChanged;
            newValue.OnChanged += OnDialogueDataChanged;
        }
    }
}