using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PotikotTools.UniTalks
{
    public class DialogueView : MonoBehaviour, IDialogueView, ITimerDialogueView
    {
        [SerializeField] protected List<MonoBehaviour> menus;
        
        [SerializeField] protected GameObject container;

        [SerializeField] protected TextMeshProUGUI label;

        [SerializeField] protected RectTransform optionsContainer;
        [SerializeField] protected OptionView optionViewPrefab;

        [SerializeField] protected Image timerImage;
        
        protected List<OptionView> optionViews;
        protected Action<int> onOptionSelected;

        public bool IsEnabled { get; protected set; }

        protected virtual void Awake()
        {
            optionViews = new List<OptionView>();
            menus ??= new List<MonoBehaviour>();

            menus.Add(this);
        }

        public virtual void Show()
        {
            if (IsEnabled) return;
            IsEnabled = true;
            
            container.SetActive(true);
        }

        public virtual void Hide()
        {
            if (!IsEnabled) return;
            IsEnabled = false;
            
            container.SetActive(false);
        }

        public void SetSpeaker(SpeakerData speakerData)
        {
            
        }
        
        public virtual void SetText(string text)
        {
            label.text = text;
        }

        public virtual void SetAnswerOptions(string[] options)
        {
            if (options == null || options.Length == 0)
            {
                DestroyOptions();
                return;
            }

            GenerateOptions(options);
        }
        
        public virtual void SetTimer(Timer timer)
        {
            timer.OnTick += p => timerImage.fillAmount = p;
        }

        public virtual void OnOptionSelected(Action<int> callback)
        {
            onOptionSelected = callback;
        }

        public void AddMenu(object menu)
        {
            if (menu is MonoBehaviour m)
                menus.Add(m);
        }
        
        public void AddMenu(MonoBehaviour menu)
        {
            if (menu)
                menus.Add(menu);
        }
        
        public virtual T GetMenu<T>()
        {
            foreach (var menu in menus)
                if (menu is T castedMenu)
                    return castedMenu;
            
            return default;
        }
        
        protected virtual void GenerateOptions(string[] options)
        {
            int optionsCount = options.Length;
            int i = 0;

            optionsContainer.gameObject.SetActive(optionsCount > 0);
            
            for (; i < optionsCount; i++)
            {
                if (optionViews.Count <= i)
                    optionViews.Add(Instantiate(optionViewPrefab, optionsContainer));
                
                int optionIndex = i;
                optionViews[i].OnSelected(() => onOptionSelected?.Invoke(optionIndex));
                optionViews[i].Show();
                optionViews[i].SetText(options[i]);
            }

            for (int j = i; j < optionViews.Count; j++)
            {
                optionViews[j].OnSelected(null);
                optionViews[j].Hide();
            }
        }

        protected virtual void DestroyOptions()
        {
            foreach (OptionView option in optionViews)
                Destroy(option.gameObject);

            optionViews.Clear();
            optionsContainer.gameObject.SetActive(false);
        }
    }
}