using UnityEngine;

namespace PotikotTools.DialogueSystem.Demo
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private Canvas _mainCanvas;

        public Canvas MainCanvas => _mainCanvas;
        
        public void Awake() => G.Hud = this;
    }
}