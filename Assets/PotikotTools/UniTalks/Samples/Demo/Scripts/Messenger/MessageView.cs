using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PotikotTools.UniTalks.Demo
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

        public void SetText(string text)
        {
            SetTextWithRowLimit(text);
        }

        public void SetTime(DateTime time)
        {
            _timeLabel.text = time.TimeOfDay.ToString()[..5];
        }
        
        public void SetAvatar(Sprite avatar)
        {
            // _avatarImage.sprite = avatar;
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
            _textLabel.enableWordWrapping = true;
            _textLabel.overflowMode = TextOverflowModes.Overflow;

            _textLabel.text = ProcessTextWithRowLimit(text, _maxSymbolsPerRow);
            UpdateContainerSize();
        }

        private void UpdateContainerSize()
        {
            _textLabel.ForceMeshUpdate();

            float characterWidthEstimate = _textLabel.fontSize * 0.5f;
            Vector2 preferredValues = _textLabel.GetPreferredValues(_textLabel.text, _maxSymbolsPerRow * characterWidthEstimate, 0);

            preferredValues.x += 115f;
            preferredValues.y += 10f;
            
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredValues.x + _padding.x);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredValues.y + _padding.y);
        }

        // TODO: fix bug while single word length more than maxSymbolsPerRow
        private string ProcessTextWithRowLimit(string text, int maxSymbolsPerRow)
        {
            if (string.IsNullOrEmpty(text) || maxSymbolsPerRow <= 0)
                return string.Empty;

            StringBuilder result = new StringBuilder();
            StringBuilder currentLine = new StringBuilder();
            int currentLineLength = 0;
            
            string[] words = text.Split(' ');
            foreach (string word in words)
            {
                if (currentLineLength + word.Length + 1 > maxSymbolsPerRow)
                {
                    result.AppendLine(currentLine.ToString());
                    currentLine.Clear();
                    currentLineLength = 0;
                }

                if (currentLineLength > 0)
                {
                    currentLine.Append(' ');
                    currentLineLength += 1;
                }
                
                currentLine.Append(word);
                currentLineLength += word.Length;
            }
            
            if (currentLineLength > 0)
                result.AppendLine(currentLine.ToString());
            
            return result.ToString().TrimEnd() + "\n ";
        }
    }
}