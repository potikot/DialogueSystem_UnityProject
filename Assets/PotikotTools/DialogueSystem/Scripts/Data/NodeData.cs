using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class NodeData
    {
        public int Id { get; private set; }

        public int SpeakerIndex;
        public string Text;
        public string AudioResourceName;
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

            SpeakerIndex = -1;
            OutputConnections = new List<ConnectionData>();
            Commands = new List<CommandData>() {new CommandData(), new CommandData()};
        }

        public string GetSpeakerName()
        {
            if (DialogueData.TryGetSpeaker(SpeakerIndex, out SpeakerData speaker))
                return speaker.Name;

            return null;
        }
    }
}