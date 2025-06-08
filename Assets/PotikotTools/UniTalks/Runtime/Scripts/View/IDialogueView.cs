using System;

namespace PotikotTools.UniTalks
{
    public interface IDialogueView
    {
        bool IsEnabled { get; }
        
        void Show();
        void Hide();

        void SetSpeaker(SpeakerData speakerData);
        void SetText(string text);
        void SetAnswerOptions(string[] options);
        
        void OnOptionSelected(Action<int> callback);

        void AddMenu(object menu);
        T GetMenu<T>();
    }
}