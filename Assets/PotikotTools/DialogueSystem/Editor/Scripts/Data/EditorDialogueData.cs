using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace PotikotTools.DialogueSystem.Editor
{
    public class EditorDialogueData
    {
        public string Description;
        
        public List<EditorNodeData> EditorNodeDataList;
        [JsonIgnore] public DialogueData RuntimeData;

        [JsonIgnore] public string Id => RuntimeData.Id;
        
        public EditorDialogueData() { }
        
        public EditorDialogueData(DialogueData runtimeData) : this(runtimeData, null) { }

        public EditorDialogueData(DialogueData runtimeData, List<EditorNodeData> editorNodeDataList)
        {
            if (runtimeData == null)
            {
                DL.LogError($"{nameof(runtimeData)} is null");
                return;
            }

            RuntimeData = runtimeData;
            EditorNodeDataList = editorNodeDataList;
            
            if (EditorNodeDataList == null)
                GenerateEditorNodeDatas();
            else if (runtimeData.Nodes.Count != editorNodeDataList.Count)
            {
                DL.LogWarning("Nodes count does not match");
                GenerateEditorNodeDatas();
            }
        }

        public async Task<bool> TrySetId(string value)
        {
            string previousId = Id;
            if (!RuntimeData.TrySetId(value))
                return false;

            string oldPath = Path.Combine(Components.Database.RelativeRootPath, previousId);
            string newPath = Path.Combine(Components.Database.RelativeRootPath, Id);

            string error = AssetDatabase.MoveAsset(oldPath, newPath);

            if (!string.IsNullOrEmpty(error))
            {
                DL.LogError(error);
                return false;
            }

            await EditorDatabase.SaveDialogueAsync(this);
            return true;
        }

        public void GenerateEditorNodeDatas()
        {
            // TODO: write func that calculates position of the node based on hierarchy

            if (RuntimeData == null)
            {
                DL.LogError($"{nameof(RuntimeData)} is null");
                return;
            }

            if (EditorNodeDataList == null)
                EditorNodeDataList = new List<EditorNodeData>(RuntimeData.Nodes.Count);
            
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