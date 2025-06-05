namespace PotikotTools.UniTalks
{
    public class TimerNodeData : MultipleChoiceNodeData
    {
        public float Duration;
        
        public TimerNodeData(int id, float duration = 0f) : base(id)
        {
            Duration = duration;
        }
    }
}