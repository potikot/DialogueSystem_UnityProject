namespace PotikotTools.DialogueSystem
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