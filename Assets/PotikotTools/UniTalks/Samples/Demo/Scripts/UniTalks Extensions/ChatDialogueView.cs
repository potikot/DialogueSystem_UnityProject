using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PotikotTools.UniTalks.Demo
{
    public class ChatDialogueView : DialogueView
    {
        [Header("Chat Dialogue View")]
        [Header("Title")]
        [SerializeField] private Image _titleAvatarImage;
        [SerializeField] private TextMeshProUGUI _titleNameLabel;
        [SerializeField] private TextMeshProUGUI _titleLastSeenLabel;
        [SerializeField] private Button _closeButton;

        [Header("Messages")]
        [SerializeField] private RectTransform _messagesContainer;
        [SerializeField] private GameObject _leftSideMessageViewPrefab;
        [SerializeField] private GameObject _rightSideMessageViewPrefab;

        private List<MessageView> _messages;
        
        private int _lastMessageSide;
        
        protected override void Awake()
        {
            base.Awake();
            
            _messages = new List<MessageView>();
        }

        private void Start()
        {
            _closeButton.onClick.AddListener(G.Hud.MessengerWindow.CloseChat);
        }
        
        public void SetAvatarImage(Sprite avatar)
        {
            _titleAvatarImage.sprite = avatar;
        }
        
        public override void SetText(string text)
        {
            if (_lastMessageSide == 1)
                _messages[^1].HideAvatar();
            
            var message = Instantiate(_leftSideMessageViewPrefab, _messagesContainer)
                .GetComponentInChildren<MessageView>();
            
            message.SetText(text);
            message.SetAvatar(_titleAvatarImage.sprite);
            message.SetTime(DateTime.Now);
            _messages.Add(message);
            _lastMessageSide = 1;
        }

        public void SetAnswerText(string text)
        {
            if (_lastMessageSide == 2)
                _messages[^1].HideAvatar();
            
            var message = Instantiate(_rightSideMessageViewPrefab, _messagesContainer)
                .GetComponentInChildren<MessageView>();
            
            message.SetText(text);
            message.SetAvatar(_titleAvatarImage.sprite);
            message.SetTime(DateTime.Now);
            _messages.Add(message);
            _lastMessageSide = 2;
        }

        public void DisableOptions()
        {
            
        }
    }
}