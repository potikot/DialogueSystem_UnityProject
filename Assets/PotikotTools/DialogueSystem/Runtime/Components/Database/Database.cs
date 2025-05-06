using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PotikotTools.DialogueSystem
{
    public class Database
    {
        protected IDialogueLoader loader;
        protected string rootPath;
        protected string relativeRootPath;

        // TODO: update tags when tag added to dialogue data after initialization
        protected Dictionary<string, HashSet<string>> tags;
        protected Dictionary<string, DialogueData> dialogues;

        protected Dictionary<Type, string> resourceDirectories;
        
        protected bool isInitialized;

        public IReadOnlyDictionary<string, HashSet<string>> Tags => tags;
        public IDialogueLoader Loader => loader;
        public string RootPath => rootPath;
        public string RelativeRootPath => relativeRootPath;

        internal IReadOnlyDictionary<string, DialogueData> Dialogues => dialogues;
        
        public virtual async void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            loader = new JsonDialogueLoader();
            rootPath = Path.Combine(Application.dataPath, DialogueSystemPreferences.Data.DatabaseDirectory);
            relativeRootPath = Path.Combine("Assets", DialogueSystemPreferences.Data.DatabaseDirectory);
            
            tags = new Dictionary<string, HashSet<string>>();
            dialogues = new Dictionary<string, DialogueData>();

            resourceDirectories = new Dictionary<Type, string>
            {
                { typeof(AudioClip), "Audio" },
                { typeof(Sprite), "Images" },
                { typeof(Texture), "Images" }
            };
            
            if (!Directory.Exists(rootPath))
            {
                DL.LogError("Directory doesn't exist: " + rootPath);
                return;
            }

            string[] dialogueDirectories = Directory.GetDirectories(rootPath);
            
            // TODO: optimize tags saving/loading
            foreach (string dialogueDirectory in dialogueDirectories)
            {
                string dialogueName = Path.GetFileName(dialogueDirectory);

                List<string> data = await GetDialogueTagsAsync(dialogueName);
                foreach (string tag in data)
                {
                    GetOrAddTag(tag).Add(dialogueName);
                }
            }
        }

        public virtual async Task<List<string>> GetDialogueTagsAsync(string dialogueName)
        {
            return await loader.LoadTagsAsync(rootPath, dialogueName);
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
            DialogueData dialogueData = await loader.LoadDataAsync(rootPath, dialogueName);
            Components.NodeLinker.SetConnections(dialogueData);
            Components.NodeLinker.Clear();

            if (dialogueData != null)
            {
                dialogues.TryAdd(dialogueName, dialogueData);
                foreach (NodeData node in dialogueData.Nodes)
                    node.DialogueData = dialogueData;
                
                return true;
            }

            DL.LogError($"Dialogue data doesn't exist: {dialogueName}");
            return false;
        }

        public virtual bool LoadDialogue(string dialogueName)
        {
            DialogueData dialogueData = loader.LoadData(rootPath, dialogueName);
            Components.NodeLinker.SetConnections(dialogueData);
            Components.NodeLinker.Clear();

            if (dialogueData != null)
            {
                dialogues.TryAdd(dialogueName, dialogueData);
                foreach (NodeData node in dialogueData.Nodes)
                    node.DialogueData = dialogueData;
                
                return true;
            }

            DL.LogError($"Dialogue data doesn't exist: {dialogueName}");
            return false;
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

            DL.Log($"Loaded {dialogueNames.Count} dialogues");
            
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
        
        public virtual async Task<T> LoadResourceAsync<T>(string dialogueName, string resourceName) where T : Object
        {
            if (!resourceDirectories.TryGetValue(typeof(T), out string directory))
                return null;
            
            string path = Path.Combine(rootPath, directory, resourceName);
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
                    DL.Log($"Loaded: {resource.name}");
                    tcs.SetResult(resource);
                }
                else
                {
                    DL.LogError($"Failed to load AudioClip at path: {path}");
                    tcs.SetCanceled();
                }
            };
            
            return await tcs.Task;
            #endif
        }

        internal void AddDialogue(DialogueData dialogueData)
        {
            if (dialogueData == null
                || !dialogues.TryAdd(dialogueData.Id, dialogueData))
                return;

            foreach (string tag in dialogueData.Tags)
            {
                GetOrAddTag(tag).Add(dialogueData.Id);
            }
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