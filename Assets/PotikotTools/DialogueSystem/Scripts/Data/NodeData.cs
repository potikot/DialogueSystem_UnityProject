using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PotikotTools.DialogueSystem
{
    public class NodeData
    {
        public int Id { get; private set; }

        public int SpeakerId;
        public string Text;
        public AssetReferenceT<AudioClip> AudioAssetReference;
        public List<CommandData> Commands;

        public ConnectionData InputConnection;
        public List<ConnectionData> OutputConnections;
        
        [JsonIgnore] public DialogueData DialogueData;

        [JsonIgnore] public bool HasInputConnection => InputConnection != null;
        [JsonIgnore] public bool HasOutputConnections => OutputConnections.Count > 0;
        
        private NodeData() { }

        public NodeData(int id)
        {
            new DialogueController().NodeHandlers.Add(typeof(SingleChoiceNodeData), node => { });
            Id = id;
            
            OutputConnections = new List<ConnectionData>();
            Commands = new List<CommandData>();
        }
    }
}