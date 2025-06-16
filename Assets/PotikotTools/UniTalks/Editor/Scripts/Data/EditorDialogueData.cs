using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace PotikotTools.UniTalks.Editor
{
    public class EditorDialogueData
    {
        public event Action<string> OnNameChanged;
        public event Action OnDeleted;
        
        private DialogueData _runtimeData;
        
        public string Description;
        
        public List<EditorNodeData> EditorNodeDataList;
        
        public Vector3 GraphViewPosition;
        public Vector3 GraphViewScale;
        
        public bool SettingsPanelOpened;
        public Vector2 SettingsPanelPosition;
        
        [JsonIgnore] public DialogueData RuntimeData
        {
            get => _runtimeData;
            set => Initialize(value);
        }

        [JsonIgnore] public string Name => RuntimeData.Name;
        
        public EditorDialogueData() { }
        
        public EditorDialogueData(DialogueData runtimeData) : this(runtimeData, null) { }

        public EditorDialogueData(DialogueData runtimeData, List<EditorNodeData> editorNodeDataList)
        {
            if (runtimeData == null)
            {
                UniTalksAPI.LogError($"{nameof(runtimeData)} is null");
                return;
            }

            EditorNodeDataList = editorNodeDataList;
            Initialize(runtimeData);
        }

        private void Initialize(DialogueData runtimeData)
        {
            if (runtimeData == null)
            {
                UniTalksAPI.LogError($"{nameof(runtimeData)} is null");
                return;
            }
            
            _runtimeData = runtimeData;
            
            if (EditorNodeDataList == null)
                GenerateEditorNodeDatas();
            else if (_runtimeData.Nodes.Count != EditorNodeDataList.Count)
            {
                UniTalksAPI.LogWarning("Nodes count does not match");
                GenerateEditorNodeDatas();
            }
            
            RuntimeData.OnNodeRemoved += (data, index) =>
            {
                EditorNodeDataList.RemoveAt(index);
            };
        }
        
        public async Task<bool> TrySetName(string value)
        {
            string previousName = Name;
            if (!RuntimeData.TrySetName(value))
                return false;

            string oldPath = Path.Combine(DialoguesComponents.Database.DialoguesRelativeRootPath, previousName);
            string newPath = Path.Combine(DialoguesComponents.Database.DialoguesRelativeRootPath, Name);

            string error = AssetDatabase.MoveAsset(oldPath, newPath);

            if (!string.IsNullOrEmpty(error))
            {
                UniTalksAPI.LogError(error);
                return false;
            }
            
            await EditorDialogueComponents.Database.SaveDialogueAsync(this);
            OnNameChanged?.Invoke(value);
            return true;
        }

        public void GenerateEditorNodeDatas()
        {
            // TODO: write func that calculates position of the node based on hierarchy

            if (RuntimeData == null)
            {
                UniTalksAPI.LogError($"{nameof(RuntimeData)} is null");
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

        internal void OnDelete() => OnDeleted?.Invoke();
    }
}