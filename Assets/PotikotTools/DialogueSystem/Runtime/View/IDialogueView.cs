using System;

namespace PotikotTools.DialogueSystem
{
    public interface ITimerDialogueView
    {
        void SetTimer(Timer timer);
    }
    
    public interface IDialogueView
    {
        bool IsEnabled { get; }
        
        void Show();
        void Hide();

        void SetText(string text);
        void SetOptions(string[] options);
        
        void OnOptionSelected(Action<int> callback);
    }
}