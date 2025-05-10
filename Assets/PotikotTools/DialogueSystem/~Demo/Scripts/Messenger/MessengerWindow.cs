using System.Collections.Generic;
using UnityEngine;

namespace PotikotTools.DialogueSystem.Demo
{
    public class MessengerWindow : MonoBehaviour
    {
        [SerializeField] private MessengerWindowConfig _config;
        [SerializeField] private RectTransform _chatsContainer;
        [SerializeField] private ChatPanelView _chatPanelViewPrefab;
        
        private Dictionary<string, ChatDialogueController> _chats;
        
        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _chats = new Dictionary<string, ChatDialogueController>(_config.ChatDatas.Count);
            
            foreach (var chatData in _config.ChatDatas)
            {
                ChatPanelView chatPanelView = Instantiate(_chatPanelViewPrefab, _chatsContainer);
                chatPanelView.SetData(chatData);
            }
        }

        public void Show()
        {
            
        }

        public void Hide()
        {
            
        }
        
        public void OpenChat(string dialogueName)
        {
            DL.Log("Open chat: " + dialogueName);
        }
    }
}