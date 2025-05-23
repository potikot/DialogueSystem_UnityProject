using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

namespace PotikotTools.DialogueSystem.Editor
{
    [InitializeOnLoad]
    public static class NodeEditorWindowsManager
    {
        private const string LaunchConfigRelativeFilepath = "Assets/PotikotTools/DialogueSystem/Editor/Resources/Cache/node_editors.txt";
        private static readonly string LaunchConfigFilepath = FileUtility.GetAbsolutePath(LaunchConfigRelativeFilepath);
        
        private static List<NodeEditorWindow> Windows;

        static NodeEditorWindowsManager() => EditorApplication.delayCall += Initialize;

        private static void Initialize()
        {
            var nodeEditorWindows = Resources.FindObjectsOfTypeAll<NodeEditorWindow>();
            Windows = new List<NodeEditorWindow>(nodeEditorWindows.Length);
            
            string[] dialogueNames = FileUtility.ReadAllLines(LaunchConfigFilepath);

            foreach (var window in nodeEditorWindows)
            {
                string dialogueName = dialogueNames.FirstOrDefault(dn => window.titleContent.text.Contains(dn));
                if (string.IsNullOrEmpty(dialogueName))
                {
                    window.Close(); // TODO: null ref sometimes (1 window stay open and hided, and there is no names in save file)
                    continue;
                }

                var editorDialogueData = EditorComponents.Database.LoadDialogue(dialogueName);
                if (editorDialogueData == null)
                {
                    window.Close();
                    continue;
                }
                
                window.EditorData = editorDialogueData;
                window.OnClose += OnWindowClosed;
                editorDialogueData.OnNameChanged += OnDialogueNameChanged;

                Windows.Add(window);
            }

            Save();
        }
        
        public static void Open(EditorDialogueData editorDialogueData)
        {
            NodeEditorWindow window = Windows.FirstOrDefault(w => w.EditorData == editorDialogueData);

            if (window == null)
            {
                window = CreateWindow(editorDialogueData);
                Save();
            }
            
            window.Focus();
        }
        
        private static NodeEditorWindow CreateWindow(EditorDialogueData editorDialogueData)
        {
            var window = ScriptableObject.CreateInstance<NodeEditorWindow>();

            window.EditorData = editorDialogueData;
            window.OnClose += OnWindowClosed;
            window.EditorData.OnNameChanged += OnDialogueNameChanged;
            
            Windows.Add(window);
            window.Show();
            window.position = new Rect(DialogueSystemPreferences.InitialDialogueEditorWindowPosition, DialogueSystemPreferences.InitialDialogueEditorWindowSize);

            return window;
        }
        
        private static void Save()
        {
            string[] lines = Windows.Select(w => w.EditorData.Name).ToArray();
            FileUtility.WriteAllLines(LaunchConfigFilepath, lines);
        }

        private static void OnWindowClosed(NodeEditorWindow window)
        {
            if (Windows.Remove(window))
            {
                window.EditorData.OnNameChanged -= OnDialogueNameChanged;
                Save();
            }
        }

        private static void OnDialogueNameChanged(string newName)
        {
            Save();
        }
    }
}