using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace PotikotTools.DialogueSystem.Editor
{
    public class EditorDialogueData
    {
        [JsonIgnore] public DialogueData RuntimeData;
        public List<EditorNodeData> EditorNodeDataList;

        public EditorDialogueData() { }
        
        public EditorDialogueData(DialogueData runtimeData)
        {
            // TODO: write func that calculates position of the node based on hierarchy
            RuntimeData = runtimeData;

            EditorNodeDataList = new List<EditorNodeData>(RuntimeData.Nodes.Count);
            for (int i = 0; i < RuntimeData.Nodes.Count; i++)
                EditorNodeDataList.Add(new EditorNodeData());
        }

        public EditorDialogueData(DialogueData runtimeData, List<EditorNodeData> editorNodeDataList)
        {
            if (runtimeData.Nodes.Count != editorNodeDataList.Count)
            {
                DL.LogWarning("Nodes count does not match");
                GenerateEditorNodeDatas();
            }
            
            RuntimeData = runtimeData;
            EditorNodeDataList = editorNodeDataList;
        }

        public void GenerateEditorNodeDatas()
        {
            if (RuntimeData == null)
                return;

            if (EditorNodeDataList == null)
            {
                DL.LogWarning("EditorNodeDataList is null");
                EditorNodeDataList = new List<EditorNodeData>(RuntimeData.Nodes.Count);
            }
            
            int lackedCount = RuntimeData.Nodes.Count - EditorNodeDataList.Count;
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