using System;

namespace PotikotTools.UniTalks
{
    public class SpeakerData
    {
        public event Action<string> OnNameChanged;
        
        protected string name;

        public string Name
        {
            get => name;
            set
            {
                name = value.Trim();
                OnNameChanged?.Invoke(name);
            }
        }
        
        public SpeakerData(string name)
        {
            this.name = name;
        }
        
        
    }
}