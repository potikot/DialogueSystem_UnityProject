using System.Collections.Generic;
using UnityEngine;

namespace PotikotTools.UniTalks.Demo
{
    public class MessengerWindowConfig : ScriptableObject
    {
        public List<ChatPanelData> ChatDatas = new();
    }
}