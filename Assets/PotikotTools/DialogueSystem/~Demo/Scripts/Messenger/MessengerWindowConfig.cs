using System.Collections.Generic;
using UnityEngine;

namespace PotikotTools.DialogueSystem.Demo
{
    public class MessengerWindowConfig : ScriptableObject
    {
        public List<ChatPanelData> ChatDatas = new();
    }
}