using UnityEngine;

namespace PotikotTools.UniTalks.Demo
{
    public class ChatSpeakerData : SpeakerData
    {
        public Sprite Avatar;
        
        public ChatSpeakerData(string name, Sprite avatar) : base(name)
        {
            Avatar = avatar;
        }
    }
}