using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PotikotTools.DialogueSystem
{
    // TODO: add way to sync many systems that can change similar fields
    
    public class DialogueData
    {
        public event Action<NodeData, int> OnNodeAdded;
        public event Action<NodeData, int> OnNodeRemoved;
        
        public List<string> Tags;
        public List<SpeakerData> Speakers;

        public bool LoadResourcesImmediately;
        
        [JsonRequired] protected string id;
        [JsonRequired] protected List<NodeData> nodes;

        [JsonRequired] protected int nextNodeId;
        
        [JsonIgnore] public string Id => id;
        [JsonIgnore] public IReadOnlyList<NodeData> Nodes => nodes;
        [JsonIgnore] public bool IsResourcesLoaded { get; protected set; }
        
        public DialogueData() { }
        
        public DialogueData(string id)
        {
            if (!TrySetName(id))
            {
                // TODO: set unique id
            }
            
            Tags = new List<string>();
            Speakers = new List<SpeakerData>();
            nodes = new List<NodeData>();
        }

        public bool TrySetName(string value)
        {
            if (!Components.Database.TryChangeDialogueName(id, value))
                return false;

            id = value;
            return true;
        }
        
        public T AddNode<T>(params object[] args) where T : NodeData
        {
            if (args == null || args.Length == 0)
                args = new object[1] { GetNextNodeId() };
            else
            {
                object[] newArgs = new object[args.Length + 1];
                newArgs[0] = GetNextNodeId();
                args.CopyTo(newArgs, 1);
                args = newArgs;
            }

            T node = Activator.CreateInstance(typeof(T), args) as T;
            node.DialogueData = this;

            nodes.Add(node);
            OnNodeAdded?.Invoke(node, nodes.Count - 1);
            
            return node;
        }
        
        public bool RemoveNode(NodeData node)
        {
            int index = nodes.IndexOf(node);
            if (index == -1)
                return false;
            
            nodes.RemoveAt(index);
            OnNodeRemoved?.Invoke(node, index);
            return true;
        }
        
        public NodeData GetFirstNode()
        {
            return nodes.FirstOrDefault(n => !n.HasInputConnection);
        }

        public bool HasSpeaker(string name) => Speakers.Any(s => s.Name == name);
        public bool HasSpeaker(int id) => id >= 0 && id < Speakers.Count;

        public bool TryGetSpeaker(int id, out SpeakerData speaker)
        {
            if (HasSpeaker(id))
            {
                speaker = Speakers[id];
                return true;
            }
            
            speaker = null;
            return false;
        }

        public bool TryGetSpeaker(string name, out SpeakerData speaker)
        {
            speaker = Speakers.First(s => s.Name == name);
            return speaker != null;
        }

        public async Task LoadResources()
        {
            foreach (NodeData node in nodes)
                await node.LoadResources();

            IsResourcesLoaded = true;
        }

        public void ReleaseResources()
        {
            foreach (NodeData node in nodes)
                node.ReleaseResources();
            
            IsResourcesLoaded = false;
        }
        
        private int GetNextNodeId()
        {
            return nextNodeId++;
        }
    }
}