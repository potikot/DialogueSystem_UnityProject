namespace PotikotTools.DialogueSystem
{
    public class MultipleChoiceNodeData : NodeData
    {
        public MultipleChoiceNodeData(int id) : base(id)
        {
            OutputConnections.Add(new ConnectionData("New Choice", this, null));
        }
    }
}