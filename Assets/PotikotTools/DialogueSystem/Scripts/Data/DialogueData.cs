using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace PotikotTools.DialogueSystem
{
    public class DialogueData
    {
        public string[] Tags;
        public List<SpeakerData> Speakers;

        protected List<NodeData> nodes;
        
        protected string _id;

        public string Id
        {
            get => _id;
            set
            {
                // if (value valid)
                // set
                
                _id = value;
            }
        }
        
        public IReadOnlyList<NodeData> Nodes => nodes;
        
        public DialogueData(string id)
        {
            Id = id;
            nodes = new List<NodeData>();
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

            DL.Log($"Adding node to graph with id({GetNextNodeId()}), args({args.Length})");
            
            T node = Activator.CreateInstance(typeof(T), args) as T;
            node.DialogueData = this;

            nodes.Add(node);

            return node;
        }
        
        public bool RemoveNode(NodeData node)
        {
            return nodes.Remove(node);
        }
        
        public NodeData GetFirstNode()
        {
            return nodes.First(n => !n.HasInputConnection);
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
        
        private int GetNextNodeId()
        {
            return nodes.Count;
        }
    }
}