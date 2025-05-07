namespace PotikotTools.DialogueSystem.Demo
{
    public static class G
    {
        private static TooltipManager _tooltipManager;
        
        public static HUD Hud;
        
        public static TooltipManager TooltipManager => _tooltipManager ??= new TooltipManager();
    }
}