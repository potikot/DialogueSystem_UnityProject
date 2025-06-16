using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PotikotTools.UniTalks
{
    public class DialogueData : IChangeNotifier
    {
        public event Action OnChanged;
        public event Action<NodeData, int> OnNodeAdded;
        public event Action<NodeData, int> OnNodeRemoved;
        
        public readonly ObservableList<string> Tags;
        public readonly ObservableList<SpeakerData> Speakers;
        
        public bool LoadResourcesImmediately;

        [JsonProperty("Id")] protected int id;
        [JsonProperty("Name")] protected string name;
        [JsonProperty("Nodes")] protected List<NodeData> nodes;

        [JsonProperty("NextNodeId")] protected int nextNodeId;

        internal readonly Action Internal_OnChanged;
        
        [JsonIgnore] public int Id => id;
        [JsonIgnore] public string Name => name;
        [JsonIgnore] public IReadOnlyList<NodeData> Nodes => nodes;

        [JsonIgnore] public bool IsResourcesLoaded { get; protected set; }

        public DialogueData()
        {
            Internal_OnChanged += () => OnChanged?.Invoke();
            
            Tags = new ObservableList<string>();
            Speakers = new ObservableList<SpeakerData>();
            nodes = new List<NodeData>();
        }
        
        public DialogueData(string name) : this()
        {
            if (!TrySetName(name))
            {
                // TODO: set unique id
            }
        }

        public bool TrySetName(string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                if (DialoguesComponents.Database.ContainsDialogue(value))
                    return false;
            }
            else if (!DialoguesComponents.Database.TryChangeDialogueName(name, value))
                return false;

            name = value;
            return true;
        }

        public T AddNode<T>(params object[] args) where T : NodeData => (T)AddNode(typeof(T), args);
        
        public NodeData AddNode(Type type, params object[] args)
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

            NodeData node = (NodeData)Activator.CreateInstance(type, args);
            node.DialogueData = this;
            node.OnChanged += OnChanged;
            
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

        #region Speakers

        // public void AddSpeaker<T>(T speaker) where T : SpeakerData
        // {
        //     if (speakers.Contains(speaker))
        //         return;
        //     
        //     speakers.Add(speaker);
        //     OnSpeakerAdded?.Invoke(speaker);
        // }
        
        // public void AddSpeaker(string name)
        // {
        //     var speaker = new SpeakerData(name);
        //
        //     speakers.Add(speaker);
        //     OnSpeakerAdded?.Invoke(speaker);
        // }
        
        // public bool RemoveSpeaker<T>(T speaker) where T : SpeakerData
        // {
        //     if (!speakers.Remove(speaker))
        //         return false;
        //     
        //     OnSpeakerRemoved?.Invoke(speaker);
        //     return true;
        // }
        
        // public bool RemoveSpeaker(string name)
        // {
        //     int index = speakers.FindIndex(s => s.Name == name);
        //     return RemoveSpeaker(index);
        // }
        
        // public bool RemoveSpeaker(int index)
        // {
        //     if (!HasSpeaker(index))
        //         return false;
        //     
        //     var speaker = speakers[index];
        //     
        //     speakers.RemoveAt(index);
        //     OnSpeakerRemoved?.Invoke(speaker);
        //     
        //     return true;
        // }
        
        public bool HasSpeaker(string speakerName) => Speakers.Any(s => s.Name == speakerName);
        public bool HasSpeaker(int speakerIndex) => speakerIndex >= 0 && speakerIndex < Speakers.Count;

        public bool TryGetSpeaker(int speakerIndex, out SpeakerData speaker)
        {
            if (HasSpeaker(speakerIndex))
            {
                speaker = Speakers[speakerIndex];
                return true;
            }
            
            speaker = null;
            return false;
        }

        public bool TryGetSpeaker(string speakerName, out SpeakerData speaker)
        {
            speaker = Speakers.First(s => s.Name == speakerName);
            return speaker != null;
        }

        #endregion

        #region Tags

        // public void AddTag(string name)
        // {
        //     if (string.IsNullOrEmpty(name)
        //         || tags.Contains(name))
        //         return;
        //     
        //     tags.Add(name);
        //     OnTagAdded?.Invoke(name);
        // }
        //
        // public bool RemoveTag(string name)
        // {
        //     if (!tags.Remove(name))
        //         return false;
        //     
        //     OnTagRemoved?.Invoke(name);
        //     return true;
        // }
        //
        // public bool RemoveTag(int index)
        // {
        //     if (HasTag(index))
        //         return false;
        //     
        //     string tag = tags[index];
        //     
        //     tags.RemoveAt(index);
        //     OnTagRemoved?.Invoke(tag);
        //     
        //     return true;
        // }

        public bool HasTag(string tag) => Tags.Contains(tag);
        public bool HasTag(int tagIndex) => tagIndex >= 0 && tagIndex < Tags.Count;
        
        #endregion

        public async Task LoadResources()
        {
            foreach (NodeData node in nodes)
                await node.LoadResourcesAsync();

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