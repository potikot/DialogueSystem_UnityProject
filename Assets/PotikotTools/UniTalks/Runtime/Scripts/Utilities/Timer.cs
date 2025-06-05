using System;

namespace PotikotTools.UniTalks
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