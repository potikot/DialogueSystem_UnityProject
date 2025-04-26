using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public abstract class NodeData
    {
        public int Id { get; private set; }

        public int SpeakerIndex;
        public int ListenerIndex;
        public string Text;
        
        public AudioClip AudioResource;
        public string AudioResourceName;
        
        public List<CommandData> Commands;

        [JsonIgnore] public ConnectionData InputConnection;
        public List<ConnectionData> OutputConnections;
        
        [JsonIgnore] public DialogueData DialogueData;

        [JsonIgnore] public bool HasInputConnection => InputConnection != null;
        [JsonIgnore] public bool HasOutputConnections => OutputConnections.Count > 0;

        private NodeData() { }

        public NodeData(int id)
        {
            Id = id;

            SpeakerIndex = -1;
            OutputConnections = new List<ConnectionData>();
            Commands = new List<CommandData>();
        }

        public virtual string GetSpeakerName()
        {
            if (DialogueData.TryGetSpeaker(SpeakerIndex, out SpeakerData speaker))
                return speaker.Name;

            return null;
        }

        public virtual async Task LoadResources()
        {
            if (!string.IsNullOrEmpty(AudioResourceName))
                AudioResource = await Components.Database.LoadResourceAsync<AudioClip>(DialogueData.Id, AudioResourceName);
        }

        public void ReleaseResources()
        {
            Resources.UnloadAsset(AudioResource);
            DL.Log($"Loaded: {AudioResource.name}");
        }
    }
}