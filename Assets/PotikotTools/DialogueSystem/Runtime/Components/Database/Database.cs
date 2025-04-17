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

        protected Dictionary<string, List<string>> tags;
        protected Dictionary<string, DialogueData> dialogues;

        protected Dictionary<Type, string> resourceDirectories;
        
        protected bool isInitialized = false;

        public IDialogueLoader Loader => loader;
        public string RootPath => rootPath;

        public virtual void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            loader = new JsonDialogueSaverLoader();
            rootPath = Path.Combine(Application.dataPath, DialogueSystemPreferences.Preferences.DatabaseDirectory);

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

            foreach (string dialogueDirectory in dialogueDirectories)
            {
                string tagsFilePath = Path.Combine(dialogueDirectory, "tags");

                if (File.Exists(tagsFilePath))
                {
                    string dialogueId = Path.GetFileName(dialogueDirectory);
                    string[] lines = File.ReadAllLines(tagsFilePath);

                    foreach (var line in lines)
                    {
                        if (tags.TryGetValue(line, out var idList))
                            idList.Add(dialogueId);
                        else
                            tags.Add(line, new List<string>()
                            {
                                dialogueId
                            });
                    }
                }
            }
        }

        public virtual async Task<DialogueData> GetDialogueAsync(string dialogueId)
        {
            if (!dialogues.TryGetValue(dialogueId, out DialogueData data)
                && await LoadDialogueAsync(dialogueId))
                return dialogues[dialogueId];

            return data;
        }

        public virtual DialogueData GetDialogue(string dialogueId)
        {
            if (!dialogues.TryGetValue(dialogueId, out DialogueData data)
                && LoadDialogue(dialogueId))
                return dialogues[dialogueId];

            return data;
        }

        public virtual bool ContainsDialogue(string dialogueId)
        {
            return dialogues.ContainsKey(dialogueId);
        }

        public virtual async Task<bool> LoadDialogueAsync(string dialogueId)
        {
            DL.Log("LoadDialogue " + dialogueId);
            
            Components.NodeLinker = new NodeLinker();
            DialogueData dialogueData = await loader.LoadAsync(rootPath, dialogueId);
            Components.NodeLinker.SetConnections(dialogueData);
            Components.NodeLinker = null;
            
            if (dialogueData != null)
            {
                DL.Log($"Dialogue data exist: {dialogueId}");
                dialogues.Add(dialogueId, dialogueData);
                return true;
            }

            DL.LogError($"Dialogue data doesn't exist: {dialogueId}");
            return false;
        }

        public virtual bool LoadDialogue(string dialogueId)
        {
            DL.Log("LoadDialogue");
            
            Components.NodeLinker = new NodeLinker();
            DialogueData dialogueData = loader.Load(rootPath, dialogueId);
            Components.NodeLinker.SetConnections(dialogueData);
            Components.NodeLinker = null;

            if (dialogueData != null)
            {
                DL.Log($"Dialogue data exist: {dialogueId}");
                dialogues.Add(dialogueId, dialogueData);
                return true;
            }

            DL.LogError($"Dialogue data doesn't exist: {dialogueId}");
            return false;
        }

        public virtual async Task<bool> LoadDialogueGroupAsync(string tag)
        {
            if (!tags.TryGetValue(tag, out List<string> dialogueIds))
                return false;

            bool flag = true;
            foreach (string dialogueId in dialogueIds)
                if (!await LoadDialogueAsync(dialogueId))
                    flag = false;

            return flag;
        }

        public virtual bool LoadDialogueGroup(string tag)
        {
            if (!tags.TryGetValue(tag, out List<string> dialogueIds)
                || dialogueIds.Count == 0)
                return false;

            bool flag = true;
            foreach (string dialogueId in dialogueIds)
                if (!LoadDialogue(dialogueId))
                    flag = false;

            return flag;
        }

        public virtual void ReleaseDialogue(string dialogueId)
        {
            if (dialogues.TryGetValue(dialogueId, out DialogueData data))
            {
                data.ReleaseResources();
                dialogues.Remove(dialogueId);
            }
        }

        public virtual void ReleaseDialogueGroup(string tag)
        {
            if (!tags.TryGetValue(tag, out List<string> dialogueIds))
                return;

            foreach (string dialogueId in dialogueIds)
                ReleaseDialogue(dialogueId);
        }
        
        public virtual async Task<T> LoadResourceAsync<T>(string dialogueId, string resourceName) where T : Object
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