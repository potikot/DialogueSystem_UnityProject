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

        protected Dictionary<string, List<string>> tags;
        protected Dictionary<string, DialogueData> dialogues;

        protected Dictionary<Type, string> resourceDirectories;
        
        protected bool isInitialized = false;

        public int DialoguesCount { get; private set; }
        public IDialogueLoader Loader => loader;
        public string RootPath => rootPath;
        public string RelativeRootPath => relativeRootPath;

        public virtual async void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            loader = new JsonDialogueSaverLoader();
            rootPath = Path.Combine(Application.dataPath, DialogueSystemPreferences.Data.DatabaseDirectory);
            relativeRootPath = Path.Combine("Assets", DialogueSystemPreferences.Data.DatabaseDirectory);
            
            tags = new Dictionary<string, List<string>>();
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
            DialoguesCount = dialogueDirectories.Length;
            
            // TODO: optimize tags saving/loading
            foreach (string dialogueDirectory in dialogueDirectories)
            {
                string dialogueName = Path.GetFileName(dialogueDirectory);

                List<string> data = await GetDialogueTagsAsync(dialogueName);
                foreach (string tag in data)
                {
                    if (tags.TryGetValue(tag, out var idList))
                        idList.Add(dialogueName);
                    else
                        tags.Add(tag, new List<string>() { dialogueName });
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
            else
                return null;
        }

        public virtual DialogueData GetDialogue(string dialogueName)
        {
            if (!dialogues.TryGetValue(dialogueName, out DialogueData data))
                return data;

            if (LoadDialogue(dialogueName))
                return dialogues[dialogueName];
            else
                return null;
        }

        public virtual bool ContainsDialogue(string dialogueName)
        {
            return dialogues.ContainsKey(dialogueName);
        }

        public virtual async Task<bool> LoadDialogueAsync(string dialogueName)
        {
            DialogueData dialogueData = await loader.LoadAsync(rootPath, dialogueName);
            Components.NodeLinker.SetConnections(dialogueData);
            Components.NodeLinker.Clear();

            if (dialogueData != null)
            {
                dialogues.Add(dialogueName, dialogueData);
                foreach (NodeData node in dialogueData.Nodes)
                    node.DialogueData = dialogueData;
                
                return true;
            }

            DL.LogError($"Dialogue data doesn't exist: {dialogueName}");
            return false;
        }

        public virtual bool LoadDialogue(string dialogueName)
        {
            DialogueData dialogueData = loader.Load(rootPath, dialogueName);
            Components.NodeLinker.SetConnections(dialogueData);
            Components.NodeLinker.Clear();

            if (dialogueData != null)
            {
                dialogues.Add(dialogueName, dialogueData);
                foreach (NodeData node in dialogueData.Nodes)
                    node.DialogueData = dialogueData;
                
                return true;
            }

            DL.LogError($"Dialogue data doesn't exist: {dialogueName}");
            return false;
        }

        public virtual async Task<bool> LoadDialoguesByTagAsync(string tag)
        {
            if (!tags.TryGetValue(tag, out List<string> dialogueNames))
                return false;

            bool flag = true;
            foreach (string dialogueName in dialogueNames)
                if (!await LoadDialogueAsync(dialogueName))
                    flag = false;

            return flag;
        }

        public virtual bool LoadDialoguesByTag(string tag)
        {
            if (!tags.TryGetValue(tag, out List<string> dialogueNames)
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
            if (!tags.TryGetValue(tag, out List<string> dialogueNames))
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
            request.completed += ao =>
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
    }
}