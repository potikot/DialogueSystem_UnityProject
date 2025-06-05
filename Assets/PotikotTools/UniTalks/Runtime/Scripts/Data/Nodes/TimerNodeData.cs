using MessagePack;

namespace PotikotTools.UniTalks
{
    [MessagePackObject(AllowPrivate = true)]
    public partial class TimerNodeData : MultipleChoiceNodeData
    {
        [Key(7)]
        public float Duration;

        public TimerNodeData(int id) : base(id) { }
        
        public TimerNodeData(int id, float duration) : this(id)
        {
            Duration = duration;
        }
    }
}