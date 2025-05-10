using System;

namespace PotikotTools.DialogueSystem
{
    public interface IDialogueView
    {
        bool IsEnabled { get; }
        
        void Show();
        void Hide();

        void SetSpeakerText(string text);
        void SetAnswerOptions(string[] options);
        
        void OnOptionSelected(Action<int> callback);

        T GetMenu<T>();
    }
}