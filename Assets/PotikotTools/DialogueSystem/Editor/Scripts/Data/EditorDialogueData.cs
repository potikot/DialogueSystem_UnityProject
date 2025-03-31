using System.Collections.Generic;

namespace PotikotTools.DialogueSystem
{
    public class EditorDialogueData
    {
        private string _id;
        private List<EditorNodeData> _editorNodeDataList;

        public string Id => _id;

        public void SetDialogueData(DialogueData dialogueData)
        {
            _id = dialogueData.Id;
            
            int nodesCount = dialogueData.Nodes.Count;
            _editorNodeDataList = new List<EditorNodeData>(nodesCount);
            for (int i = 0; i < nodesCount; i++)
                _editorNodeDataList.Add(new EditorNodeData());
        }
    }
}