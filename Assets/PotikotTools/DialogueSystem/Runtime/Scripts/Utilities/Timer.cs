using System;

namespace PotikotTools.DialogueSystem
{
    public class Timer
    {
        public Action<float> OnTick;

        public float RemainingTime { get; private set; }
        
        public Timer(float time)
        {
            RemainingTime = time;
        }
    }
}