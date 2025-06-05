using MessagePack;

namespace PotikotTools.UniTalks
{
    [MessagePackObject(AllowPrivate = true)]
    public partial class MultipleChoiceNodeData : NodeData
    {
        public MultipleChoiceNodeData(int id) : base(id)
        {
            OutputConnections.Add(new ConnectionData("New Choice", this, null));
        }
    }
}