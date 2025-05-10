using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PotikotTools.DialogueSystem.Demo
{
    public class ChatDialogueView : DialogueView
    {
        [Header("Chat Dialogue View")]
        [Header("Title")]
        [SerializeField] private Image _titleAvatarImage;
        [SerializeField] private TextMeshProUGUI _titleNameLabel;
        [SerializeField] private TextMeshProUGUI _titleLastSeenLabel;

        [Header("Messages")]
        [SerializeField] private RectTransform _messagesContainer;
        [SerializeField] private MessageView _messageViewPrefab;

        private List<MessageView> _messages;

        protected override void Awake()
        {
            base.Awake();
            
            _messages = new List<MessageView>();
        }

        public void SetAvatarImage(Sprite avatar)
        {
            _titleAvatarImage.sprite = avatar;
        }
        
        public override void SetSpeakerText(string text)
        {
            var message = Instantiate(_messageViewPrefab, _messagesContainer);
            
            message.SetText(text);
            message.SetAvatar(_titleAvatarImage.sprite);
            message.SetTime(DateTime.Now);
        }
    }
}