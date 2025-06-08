using UnityEngine;

namespace PotikotTools.UniTalks.Demo
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private Canvas _computerScreenCanvas;
        [SerializeField] private MessengerWindow _messengerWindow;
        
        private static TooltipManager _tooltipManager;
        
        public Canvas ComputerScreenCanvas => _computerScreenCanvas;
        public MessengerWindow MessengerWindow => _messengerWindow;
        
        public static TooltipManager TooltipManager => _tooltipManager ??= new TooltipManager();
        
        public void Awake() => G.Hud = this;
    }
}