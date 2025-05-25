using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PotikotTools.DialogueSystem.Editor
{
    public abstract class WindowsManager<T> where T : BaseDialogueSystemEditorWindow
    {
        protected readonly List<T> windows;
        
        protected WindowsManager()
        {
            var nodeEditorWindows = Resources.FindObjectsOfTypeAll<T>();
            windows = new List<T>(nodeEditorWindows.Length);
            
            foreach (var window in nodeEditorWindows)
            {
                if (!TryRestoreWindow(window))
                    window.Close();

                windows.Add(window);
            }
        }

        public virtual void Open(EditorDialogueData editorDialogueData)
        {
            T window = windows.FirstOrDefault(w => w.EditorData == editorDialogueData);

            if (window == null)
                window = CreateWindow(editorDialogueData);
            
            window.Focus();
        }
        
        protected virtual bool TryRestoreWindow(T window)
        {
            var editorDialogueData = EditorComponents.Database.LoadDialogue(window.DialogueName);
            if (editorDialogueData == null)
                return false;

            window.EditorData = editorDialogueData;
            
            return true;
        }
        
        private T CreateWindow(EditorDialogueData editorDialogueData)
        {
            var window = EditorWindow.CreateWindow<T>(typeof(T), typeof(SceneView));
            windows.Add(window);

            window.EditorData = editorDialogueData;
            window.Show();

            if (!window.docked)
                window.position = new Rect(DialogueSystemPreferences.InitialDialogueEditorWindowPosition, DialogueSystemPreferences.InitialDialogueEditorWindowSize);

            return window;
        }
    }
}