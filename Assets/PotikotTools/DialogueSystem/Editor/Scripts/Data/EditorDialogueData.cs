using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class EditorDialogueData
    {
        [JsonIgnore] public DialogueData Data;
        public List<EditorNodeData> EditorNodeDataList;

        public EditorDialogueData() { }
        
        public EditorDialogueData(DialogueData data)
        {
            // TODO: write func that calculates position of the node based on hierarchy
            Data = data;

            EditorNodeDataList = new List<EditorNodeData>(Data.Nodes.Count);
            for (int i = 0; i < Data.Nodes.Count; i++)
                EditorNodeDataList.Add(new EditorNodeData());
        }

        public EditorDialogueData(DialogueData data, List<EditorNodeData> editorNodeDataList)
        {
            if (data.Nodes.Count != editorNodeDataList.Count)
            {
                DL.LogWarning("Nodes count does not match");
                GenerateEditorNodeDatas();
            }
            
            Data = data;
            EditorNodeDataList = editorNodeDataList;
        }

        public void GenerateEditorNodeDatas()
        {
            if (Data == null)
                return;

            if (EditorNodeDataList == null)
            {
                DL.LogWarning("EditorNodeDataList is null");
                EditorNodeDataList = new List<EditorNodeData>(Data.Nodes.Count);
            }
            
            int lackedCount = Data.Nodes.Count - EditorNodeDataList.Count;
            if (lackedCount < 0)
            {
                lackedCount = Mathf.Abs(lackedCount);
                EditorNodeDataList.RemoveRange(EditorNodeDataList.Count - lackedCount, lackedCount);
            }
            else
            {
                for (int i = 0; i < lackedCount; i++)
                    EditorNodeDataList.Add(new EditorNodeData());
            }
        }
    }
}