using UnityEngine;

namespace PotikotTools.UniTalks.Demo
{
    public class TooltipManager
    {
        private Tooltip _tooltip;

        public TooltipManager()
        {
            var tooltipTemplate = Resources.Load<Tooltip>("Tooltip");
            _tooltip = Object.Instantiate(tooltipTemplate, G.Hud.ComputerScreenCanvas.transform);
        }

        public void Show(string header, string content, Vector3 position)
        {
            _tooltip.Show(header, content, position);
        }

        public void Hide()
        {
            _tooltip.Hide();
        }
    }
}