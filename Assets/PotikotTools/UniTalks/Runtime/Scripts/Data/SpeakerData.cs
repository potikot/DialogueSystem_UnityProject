using System;
using MessagePack;

namespace PotikotTools.UniTalks
{
    [MessagePackObject(AllowPrivate = true)]
    public partial class SpeakerData
    {
        public event Action<string> OnNameChanged;
        
        [Key(0)]
        protected string name;

        [IgnoreMember]
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