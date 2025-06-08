using System.Collections.Generic;
using UnityEngine;

namespace PotikotTools.UniTalks.Demo
{
    public class MessengerWindow : MonoBehaviour
    {
        [SerializeField] private MessengerWindowConfig _config;
        
        [SerializeField] private RectTransform _chatsContainer;
        [SerializeField] private ChatPanelView _chatPanelViewPrefab;
        
        [SerializeField] private RectTransform _bodyContainer;
        [SerializeField] private ChatDialogueView _chatViewPrefab;
        
        private Dictionary<string, ChatDialogueController> _chats;
        private ChatDialogueController _activeDialogueController;
        
        private void Start()
        {
            Debug.Log("Execute Command");
            DialoguesComponents.CommandHandler.Execute("UniTalksAPI.StartDialogue", "test");
            // Initialize();
        }

        private void Initialize()
        {
            _chats = new Dictionary<string, ChatDialogueController>(_config.ChatDatas.Count);
            
            foreach (var chatData in _config.ChatDatas)
            {
                if (!DialoguesComponents.Database.LoadDialogue(chatData.DialogueName))
                    continue;
                
                ChatPanelView chatPanelView = Instantiate(_chatPanelViewPrefab, _chatsContainer);
                chatPanelView.SetData(chatData);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void OpenChat(string dialogueName)
        {
            if (_chats.TryGetValue(dialogueName, out var dialogueController))
            {
                _activeDialogueController?.StopDialogue();
                _activeDialogueController = dialogueController;
                dialogueController.StartDialogue();
                _chatsContainer.gameObject.SetActive(false);
                return;
            }
            
            DialogueData dialogueData = UniTalksAPI.GetDialogue(dialogueName);
                
            if (dialogueData == null)
                return;

            var dialogueView = Instantiate(_chatViewPrefab, _bodyContainer);

            dialogueController = new ChatDialogueController();
            dialogueController.Initialize(dialogueData, dialogueView);
            
            _activeDialogueController?.StopDialogue();
            _activeDialogueController = dialogueController;
            _chats.Add(dialogueName, dialogueController);
            _chatsContainer.gameObject.SetActive(false);
            dialogueController.StartDialogue();
        }

        public void CloseChat()
        {
            _chatsContainer.gameObject.SetActive(true);
            _activeDialogueController?.StopDialogue();
            _activeDialogueController = null;
        }
    }
}