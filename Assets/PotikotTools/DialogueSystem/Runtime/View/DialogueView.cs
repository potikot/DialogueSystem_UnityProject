using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PotikotTools.DialogueSystem
{
    public class DialogueView : MonoBehaviour, IDialogueView, ITimerDialogueView
    {
        [SerializeField] private MonoBehaviour[] _menus;
        
        [SerializeField] private GameObject _container;

        [SerializeField] private TextMeshProUGUI _label;

        [SerializeField] private RectTransform _optionsContainer;
        [SerializeField] private OptionView _optionViewPrefab;

        [SerializeField] private Image _timerImage;
        
        private List<OptionView> _options;
        private Action<int> _onOptionSelected;

        public bool IsEnabled { get; private set; }

        private void Awake()
        {
            _options = new List<OptionView>();
        }

        public void Show()
        {
            if (IsEnabled) return;
            IsEnabled = true;
            
            _container.SetActive(true);
        }

        public void Hide()
        {
            if (!IsEnabled) return;
            IsEnabled = false;
            
            _container.SetActive(false);
        }

        public void SetText(string text)
        {
            _label.text = text;
        }

        public void SetOptions(string[] options)
        {
            if (options == null || options.Length == 0)
            {
                DestroyOptions();
                return;
            }

            GenereateOptions(options);
        }
        
        public void SetTimer(Timer timer)
        {
            timer.OnTick += p => _timerImage.fillAmount = p;
        }

        public void OnOptionSelected(Action<int> callback)
        {
            _onOptionSelected = callback;
        }

        public T GetMenu<T>()
        {
            foreach (var menu in _menus)
                if (menu is T castedMenu)
                    return castedMenu;
            
            return default;
        }
        
        private void GenereateOptions(string[] options)
        {
            int optionsCount = options.Length;
            int i = 0;

            for (; i < optionsCount; i++)
            {
                if (_options.Count <= i)
                    _options.Add(Instantiate(_optionViewPrefab, _optionsContainer));
                
                int optionIndex = i;
                _options[i].OnSelected(() => _onOptionSelected?.Invoke(optionIndex));
                _options[i].Show();
                _options[i].SetText(options[i]);
            }

            for (int j = i; j < _options.Count; j++)
            {
                _options[j].OnSelected(null);
                _options[j].Hide();
            }
        }

        private void DestroyOptions()
        {
            foreach (OptionView option in _options)
                Destroy(option.gameObject);
                
            _options.Clear();
        }
    }
}