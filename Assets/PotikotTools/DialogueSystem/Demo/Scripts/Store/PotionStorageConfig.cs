using System.Collections.Generic;
using UnityEngine;

namespace PotikotTools.DialogueSystem.Demo
{
    [CreateAssetMenu(fileName = "New Potion Storage Config", menuName = "Potion Storage Config")]
    public class PotionStorageConfig : ScriptableObject
    {
        public List<Potion> Potions = new();
    }
}