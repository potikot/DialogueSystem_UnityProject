using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PotikotTools.UniTalks
{
    public class OptionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private Button _button;
        
        private Action _onSelected;
        
        private void Start()
        {
            _button.onClick.AddListener(() => _onSelected?.Invoke());
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void SetText(string text)
        {
            _label.text = text;
        }

        public void OnSelected(Action onSelected)
        {
            _onSelected = onSelected;
        }
    }
}