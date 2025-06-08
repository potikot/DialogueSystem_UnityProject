using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace PotikotTools.UniTalks
{
    public abstract class NodeData : IChangeNotifier
    {
        public event Action OnChanged;
        
        public readonly int Id;
        
        [JsonIgnore] public AudioClip AudioResource;
        
        public ObservableList<CommandData> Commands;

        [JsonIgnore] public ConnectionData InputConnection;
        public ObservableList<ConnectionData> OutputConnections;

        [JsonIgnore] public DialogueData DialogueData;

        [JsonProperty("SpeakerIndex")] private int _speakerIndex;
        [JsonProperty("ListenerIndex")] private int _listenerIndex;
        [JsonProperty("Text")] private string _text;
        [JsonProperty("AudioResourceName")] private string _audioResourceName;
        
        internal readonly Action Internal_OnChanged;
        
        [JsonIgnore]
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
        
        [JsonIgnore]
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
        
        [JsonIgnore]
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

        [JsonIgnore]
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

        [JsonIgnore] public bool HasInputConnection => InputConnection != null;
        [JsonIgnore] public bool HasOutputConnections => OutputConnections.Count > 0;

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
                AudioResource = await DialoguesComponents.Database.LoadResourceAsync<AudioClip>(AudioResourceName);
        }

        public virtual void ReleaseResources()
        {
            Resources.UnloadAsset(AudioResource);
        }
    }
}