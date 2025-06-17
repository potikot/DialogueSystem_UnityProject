using System.Collections.Generic;
using UnityEngine;

namespace PotikotTools.UniTalks.Demo
{
    public static class G
    {
        public static HUD Hud;

        public static BookFallingTrigger BookActionTrigger;
        public static ClockTrigger ClockActionTrigger;
        public static List<LightTrigger> LightActionTriggerList = new();
        
        public static CameraController CameraController;

        [Command(false)]
        public static void Trigger(string actionName)
        {
            switch (actionName)
            {
                case "BookFalling":
                    BookActionTrigger.Trigger();
                    break;
                case "ClockReversing":
                    ClockActionTrigger.Trigger();
                    break;
                case "LightFlicking":
                    foreach (var light in LightActionTriggerList)
                        light.Trigger();
                    break;
                default:
                    UniTalksAPI.LogWarning("Incorrect action name: " + actionName);
                    break;
            }
        }

        [Command(false)]
        public static void Clock_SetTimeScale(float timeScale)
        {
            ClockActionTrigger.TimeScale = timeScale;
        }
        
        [Command(false)]
        public static void PlayerLook(float x, float y, float z, float speed, float time)
        {
            CameraController.Look(new Vector3(x, y, z), speed, time);
        }
    }
}