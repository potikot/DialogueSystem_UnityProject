using System;
using UnityEngine;

namespace PotikotTools.DialogueSystem.Demo
{
    public enum PotionType
    {
        Body, Mental, Mystic
    }
    
    [Serializable]
    public class Potion
    {
        public string Name;
        [TextArea] public string Description;
        public string SideEffect;
        public PotionType Type;
        public Sprite Sprite;
    }
}