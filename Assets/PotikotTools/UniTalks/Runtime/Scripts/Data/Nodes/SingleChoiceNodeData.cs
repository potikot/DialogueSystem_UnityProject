namespace PotikotTools.UniTalks
{
    public class SingleChoiceNodeData : NodeData
    {
        public SingleChoiceNodeData(int id) : base(id)
        {
            OutputConnections.Add(new ConnectionData(null, this, null));
        }
    }
}