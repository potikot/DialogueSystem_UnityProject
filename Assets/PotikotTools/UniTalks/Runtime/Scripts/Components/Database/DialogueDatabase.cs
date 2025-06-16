using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PotikotTools.UniTalks
{
    // TODO: mb load dialogues as resources?
    public class DialogueDatabase
    {
        protected IDialoguePersistence persistence;
        protected string rootPath;
        protected string relativeRootPath;
        protected string dialoguesRootPath;
        protected string dialoguesRelativeRootPath;
        protected string resourcesPath;
        
        // TODO: update tags when tag added to dialogue data after initialization
        protected Dictionary<string, HashSet<string>> tags;
        protected Dictionary<string, DialogueData> dialogues;

        protected Dictionary<Type, string> resourceDirectories;
        
        protected bool isInitialized;

        public IReadOnlyDictionary<string, HashSet<string>> Tags => tags;
        public IDialoguePersistence Persistence => persistence;
        public string RootPath => rootPath;
        public string RelativeRootPath => relativeRootPath;
        public string DialoguesRootPath => dialoguesRootPath;
        public string DialoguesRelativeRootPath => dialoguesRelativeRootPath;

        internal IReadOnlyDictionary<string, DialogueData> Dialogues => dialogues;
        
        public virtual async void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            persistence = new JsonDialoguePersistence();
            
            rootPath = Path.Combine(Application.dataPath, UniTalksPreferences.Data.DatabaseDirectory).Replace('\\', '/');
            relativeRootPath = Path.Combine("Assets", UniTalksPreferences.Data.DatabaseDirectory).Replace('\\', '/');
            dialoguesRootPath = Path.Combine(rootPath, "Dialogues").Replace('\\', '/');
            dialoguesRelativeRootPath = Path.Combine(relativeRootPath, "Dialogues").Replace('\\', '/');
            resourcesPath = UniTalksPreferences.Data.DatabaseDirectory[10..];
            
            tags = new Dictionary<string, HashSet<string>>();
            dialogues = new Dictionary<string, DialogueData>();

            resourceDirectories = new Dictionary<Type, string>
            {
                { typeof(AudioClip), "Audio" },
                { typeof(Sprite), "Images" },
                { typeof(Texture), "Images" }
            };

            if (!Directory.Exists(dialoguesRootPath))
            {
                Directory.CreateDirectory(dialoguesRootPath);
                #if UNITY_EDITOR
                UnityEditor.AssetDatabase.ImportAsset(dialoguesRelativeRootPath);
                #endif
            }
            
            string[] dialogueDirectories = Directory.GetDirectories(dialoguesRootPath);
            
            foreach (string dialogueDirectory in dialogueDirectories)
            {
                string dialogueName = Path.GetFileName(dialogueDirectory);
                await AddDialogueTagsAsync(dialogueName);
            }
        }

        public virtual bool TryChangeDialogueName(string oldName, string newName)
        {
            if (!dialogues.TryGetValue(oldName, out DialogueData dialogueData)
                || ContainsDialogue(newName))
                return false;

            dialogues.Remove(oldName);
            dialogues.Add(newName, dialogueData);
            
            return true;
        }

        public virtual async Task<List<string>> GetDialogueTagsAsync(string dialogueName)
        {
            return await persistence.LoadTagsAsync(dialoguesRootPath, dialogueName);
        }

        public virtual async Task<DialogueData> GetDialogueAsync(string dialogueName)
        {
            if (dialogues.TryGetValue(dialogueName, out DialogueData data))
                return data;

            if (await LoadDialogueAsync(dialogueName))
                return dialogues[dialogueName];
            
            return null;
        }

        public virtual DialogueData GetDialogue(string dialogueName)
        {
            if (dialogues.TryGetValue(dialogueName, out DialogueData data))
                return data;

            if (LoadDialogue(dialogueName))
                return dialogues[dialogueName];

            return null;
        }

        public virtual bool ContainsDialogue(string dialogueName)
        {
            return dialogues.ContainsKey(dialogueName);
        }

        public virtual async Task<bool> LoadDialogueAsync(string dialogueName)
        {
            DialogueData dialogueData = await persistence.LoadAsync(dialoguesRootPath, dialogueName);
            if (dialogueData == null)
            {
                UniTalksAPI.LogError($"Dialogue data doesn't exist: {dialogueName}");
                return false;
            }
            
            DialoguesComponents.NodeBinder.Bind(dialogueData);
            DialoguesComponents.NodeBinder.Clear();

            foreach (NodeData node in dialogueData.Nodes)
            {
                node.DialogueData = dialogueData;
                node.OnChanged += dialogueData.Internal_OnChanged;
            }

            AddDialogue(dialogueData);
            return true;
        }

        public virtual bool LoadDialogue(string dialogueName)
        {
            DialogueData dialogueData = persistence.Load(dialoguesRootPath, dialogueName);
            if (dialogueData == null)
            {
                UniTalksAPI.LogError($"Dialogue data doesn't exist: {dialogueName}");
                return false;
            }
            
            DialoguesComponents.NodeBinder.Bind(dialogueData);
            DialoguesComponents.NodeBinder.Clear();

            foreach (NodeData node in dialogueData.Nodes)
            {
                node.DialogueData = dialogueData;
                node.OnChanged += dialogueData.Internal_OnChanged;
            }

            AddDialogue(dialogueData);
            return true;
        }

        public virtual async Task<bool> LoadDialoguesByTagAsync(string tag)
        {
            if (!tags.TryGetValue(tag, out var dialogueNames))
                return false;

            bool flag = true;
            foreach (string dialogueName in dialogueNames)
                if (!await LoadDialogueAsync(dialogueName))
                    flag = false;

            return flag;
        }

        public virtual bool LoadDialoguesByTag(string tag)
        {
            if (!tags.TryGetValue(tag, out var dialogueNames)
                || dialogueNames.Count == 0)
                return false;

            bool flag = true;
            foreach (string dialogueName in dialogueNames)
                if (!LoadDialogue(dialogueName))
                    flag = false;

            UniTalksAPI.Log($"Loaded {dialogueNames.Count} dialogues");
            
            return flag;
        }

        public virtual void ReleaseDialogue(string dialogueName)
        {
            if (dialogues.TryGetValue(dialogueName, out DialogueData data))
            {
                data.ReleaseResources();
                dialogues.Remove(dialogueName);
            }
        }

        public virtual void ReleaseDialoguesByTag(string tag)
        {
            if (!tags.TryGetValue(tag, out var dialogueNames))
                return;

            foreach (string dialogueName in dialogueNames)
                ReleaseDialogue(dialogueName);
        }

        public virtual void ReleaseAllDialogues()
        {
            var dialogueNames = new List<string>(dialogues.Keys);
            foreach (var dialogueName in dialogueNames)
                ReleaseDialogue(dialogueName);
        }

        public virtual bool TryAddResourceType<T>(string folderName) where T : Object
        {
            return resourceDirectories.TryAdd(typeof(T), folderName);
        }
        
        public virtual bool TryAddResourceType(Type resourceType, string folderName)
        {
            return resourceDirectories.TryAdd(resourceType, folderName);
        }
        
        public virtual async Task<T> LoadResourceAsync<T>(string resourceName) where T : Object
        {
            string path = GetResourcePath<T>(resourceName);
            
            if (path == null)
                return null;
            
            ResourceRequest request = Resources.LoadAsync<T>(path);
            
            #if UNITY_2023_1_OR_NEWER

            await request;
            if (request.asset is T resource)
            {
                DL.Log($"Loaded: {resource.name}");
                return resource;
            }

            DL.LogError($"Failed to load AudioClip at path: {path}");
            return null;
            
            #else
            
            TaskCompletionSource<T> tcs = new();
            request.completed += _ =>
            {
                if (request.asset is T resource)
                {
                    // DL.Log($"Loaded: {resource.name}");
                    tcs.SetResult(resource);
                }
                else
                {
                    UniTalksAPI.LogError($"Failed to load AudioClip at path: {path}");
                    tcs.SetCanceled();
                }
            };
            
            return await tcs.Task;
            #endif
        }

        public virtual T LoadResource<T>(string resourceName) where T : Object
        {
            string path = GetResourcePath<T>(resourceName);
            return path == null ? null : Resources.Load<T>(path);
        }

        public virtual string GetResourcePath(Type resourceType, string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName)
                || !resourceDirectories.TryGetValue(resourceType, out string directory))
                return null;
            
            return Path.Combine(resourcesPath, directory, resourceName);
        }
        
        public virtual string GetResourcePath<T>(string resourceName) where T : Object
        {
            return GetResourcePath(typeof(T), resourceName);
        }
        
        public virtual string GetProjectRelativeResourcePath(Type resourceType, string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName)
                || !resourceDirectories.TryGetValue(resourceType, out string directory))
                return null;
            
            return Path.Combine(relativeRootPath, directory, resourceName);
        }
        
        public virtual string GetProjectRelativeResourcePath<T>(string resourceName) where T : Object
        {
            return GetProjectRelativeResourcePath(typeof(T), resourceName);
        }
        
        private void RegisterTagsChangedEvents(DialogueData dialogueData)
        {
            if (dialogueData == null)
                return;
            
            dialogueData.Tags.OnElementAdded += OnTagAdded;
            dialogueData.Tags.OnElementChanged += OnTagChanged;
            dialogueData.Tags.OnElementRemoved += OnTagRemoved;
            
            void OnTagAdded(string tag)
            {
                var tagDialogues = GetOrAddTag(tag);
                tagDialogues.Add(dialogueData.Name);
            }
            
            void OnTagChanged(int index, string prevValue, string newValue)
            {
                if (prevValue == newValue)
                    return;
                
                var prevTagDialogues = GetOrAddTag(prevValue);
                prevTagDialogues.Remove(dialogueData.Name);
                if (prevTagDialogues.Count == 0)
                    tags.Remove(prevValue);
                
                var newTagDialogues = GetOrAddTag(newValue);
                newTagDialogues.Add(dialogueData.Name);
            }
            
            void OnTagRemoved(string tag)
            {
                if (!tags.TryGetValue(tag, out var tagDialogues))
                    return;
                
                tagDialogues.Remove(dialogueData.Name);
                if (tagDialogues.Count == 0)
                    tags.Remove(tag);
            }
        }
        
        // TODO: check async/await error when changing data from many places
        private async Task AddDialogueTagsAsync(string dialogueName)
        {
            if (dialogues.TryGetValue(dialogueName, out DialogueData dialogueData))
            {
                AddDialogueTags(dialogueData);
                return;
            }
            
            var dialogueTags = await GetDialogueTagsAsync(dialogueName);
            foreach (string tag in dialogueTags)
                GetOrAddTag(tag).Add(dialogueName);
        }
        
        private void AddDialogueTags(DialogueData dialogueData)
        {
            foreach (string tag in dialogueData.Tags)
                GetOrAddTag(tag).Add(dialogueData.Name);
            
            RegisterTagsChangedEvents(dialogueData);
        }
        
        internal void AddDialogue(DialogueData dialogueData)
        {
            if (dialogueData?.Name == null)
                return;

            dialogues[dialogueData.Name] = dialogueData;
            AddDialogueTags(dialogueData);
        }

        internal HashSet<string> GetOrAddTag(string tag)
        {
            if (tags.TryGetValue(tag, out var dialogueNames))
                return dialogueNames;

            dialogueNames = new HashSet<string>();
            tags.Add(tag, dialogueNames);

            return dialogueNames;
        }
    }
}