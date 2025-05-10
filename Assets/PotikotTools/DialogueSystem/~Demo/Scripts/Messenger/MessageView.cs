using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PotikotTools.DialogueSystem.Demo
{
    public class MessageView : MonoBehaviour
    {
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TextMeshProUGUI _textLabel;
        [SerializeField] private TextMeshProUGUI _timeLabel;

        [SerializeField] private int _maxSymbolsPerRow;
        [SerializeField] private Vector2 _padding;
        
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
        }

        private void Start()
        {
            SetTextWithRowLimit(_textLabel.text);
        }

        public void SetText(string text)
        {
            SetTextWithRowLimit(text);
        }

        public void SetTime(DateTime time)
        {
            _timeLabel.text = time.TimeOfDay.ToString();
        }
        
        public void SetAvatar(Sprite avatar)
        {
            _avatarImage.sprite = avatar;

        }
        
        public void ShowAvatar()
        {
            _avatarImage.gameObject.SetActive(true);
        }

        public void HideAvatar()
        {
            _avatarImage.gameObject.SetActive(false);
        }
        
        private void SetTextWithRowLimit(string text)
        {
            _textLabel.text = ProcessTextWithRowLimit(text, _maxSymbolsPerRow);
            UpdateContainerSize();
        }

        private void UpdateContainerSize()
        {
            _textLabel.ForceMeshUpdate();
            Canvas.ForceUpdateCanvases();

            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _textLabel.preferredWidth + _padding.x);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _textLabel.preferredHeight + _padding.y);
        }

        private string ProcessTextWithRowLimit(string input, int maxPerRow)
        {
            StringBuilder result = new();
            int currentIndex = 0;

            while (currentIndex < input.Length)
            {
                int length = Mathf.Min(maxPerRow, input.Length - currentIndex);
                result.AppendLine(input.Substring(currentIndex, length).TrimEnd());
                currentIndex += length;
            }

            return result.ToString().TrimEnd();
        }
    }
}