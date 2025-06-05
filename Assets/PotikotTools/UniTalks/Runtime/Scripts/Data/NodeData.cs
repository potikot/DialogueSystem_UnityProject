using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using MessagePack;
using Newtonsoft.Json;
using UnityEngine;

namespace PotikotTools.UniTalks
{
    [MessagePackObject(AllowPrivate = true)]
    public partial class NodeData : IChangeNotifier
    {
        public event Action OnChanged;
        
        [Key(0)]
        public readonly int Id;
        
        [JsonIgnore, IgnoreMember]
        public AudioClip AudioResource;
        
        [Key(1)]
        public ObservableList<CommandData> Commands;

        [JsonIgnore, IgnoreMember]
        public ConnectionData InputConnection;
        [Key(2)]
        public ObservableList<ConnectionData> OutputConnections;

        [JsonIgnore, IgnoreMember]
        public DialogueData DialogueData;

        [JsonProperty("SpeakerIndex"), IgnoreMember] private int _speakerIndex;
        [JsonProperty("ListenerIndex"), IgnoreMember] private int _listenerIndex;
        [JsonProperty("Text"), IgnoreMember] private string _text;
        [JsonProperty("AudioResourceName"), IgnoreMember] private string _audioResourceName;
        
        [IgnoreMember]
        internal readonly Action Internal_OnChanged;
        
        [JsonIgnore, Key(3)]
        public int SpeakerIndex
        {
            get => _speakerIndex;
            set
            {
                if (_speakerIndex == value)
                    return;
                
                _speakerIndex = value;
                OnChanged?.Invoke();
            }
        }
        
        [JsonIgnore, Key(4)]
        public int ListenerIndex
        {
            get => _listenerIndex;
            set
            {
                if (_listenerIndex == value)
                    return;
                
                _listenerIndex = value;
                OnChanged?.Invoke();
            }
        }
        
        [JsonIgnore, Key(5)]
        public string Text
        {
            get => _text;
            set
            {
                if (_text == value)
                    return;
                
                _text = value;
                OnChanged?.Invoke();
            }
        }

        [JsonIgnore, Key(6)]
        public string AudioResourceName
        {
            get => _audioResourceName;
            set
            {
                if (_audioResourceName == value)
                    return;
                
                _audioResourceName = value;
                OnChanged?.Invoke();
            }
        }

        [JsonIgnore, IgnoreMember]
        public bool HasInputConnection => InputConnection != null;
        [JsonIgnore, IgnoreMember]
        public bool HasOutputConnections => OutputConnections.Count > 0;

        protected NodeData()
        {
            Internal_OnChanged = () => OnChanged?.Invoke();
        }
        
        protected NodeData(int id) : this()
        {
            Id = id;
            _speakerIndex = -1;
            OutputConnections = new ObservableList<ConnectionData>();
            Commands = new ObservableList<CommandData>();

            NotifyCollectionChangedEventHandler collectionChanged = (_, _) => OnChanged?.Invoke();

            OutputConnections.CollectionChanged += collectionChanged;
            OutputConnections.OnElementAdded += OnElementAdded;
            OutputConnections.OnElementRemoved += OnElementRemoved;
            OutputConnections.OnElementChanged += OnElementChanged;

            Commands.CollectionChanged += collectionChanged;
            Commands.OnElementAdded += OnElementAdded;
            Commands.OnElementRemoved += OnElementRemoved;
            Commands.OnElementChanged += OnElementChanged;
        }

        private void OnElementAdded(IChangeNotifier element)
        {
            element.OnChanged += OnChanged;
            OnChanged?.Invoke();
        }

        private void OnElementRemoved(IChangeNotifier element)
        {
            element.OnChanged -= OnChanged;
            OnChanged?.Invoke();
        }

        private void OnElementChanged(int idx, IChangeNotifier prevElement, IChangeNotifier newElement)
        {
            prevElement.OnChanged -= OnChanged;
            newElement.OnChanged += OnChanged;
            OnChanged?.Invoke();
        }

        public virtual SpeakerData GetSpeaker()
        {
            if (DialogueData.TryGetSpeaker(SpeakerIndex, out SpeakerData speaker))
                return speaker;

            return null;
        }
        
        public virtual string GetSpeakerName()
        {
            if (DialogueData.TryGetSpeaker(SpeakerIndex, out SpeakerData speaker))
                return speaker.Name;

            return null;
        }

        public virtual async Task LoadResourcesAsync()
        {
            if (!string.IsNullOrEmpty(AudioResourceName))
                AudioResource = await UniTalksComponents.Database.LoadResourceAsync<AudioClip>(AudioResourceName);
        }

        public virtual void ReleaseResources()
        {
            Resources.UnloadAsset(AudioResource);
        }
    }
}