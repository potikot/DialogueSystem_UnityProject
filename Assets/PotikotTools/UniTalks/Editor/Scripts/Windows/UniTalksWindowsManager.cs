using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PotikotTools.UniTalks.Editor
{
    public abstract class UniTalksWindowsManager<T> where T : BaseUniTalksEditorWindow
    {
        protected readonly List<T> windows;
        protected readonly Type[] desiredDockNextTo;

        protected UniTalksWindowsManager()
        {
            var nodeEditorWindows = Resources.FindObjectsOfTypeAll<T>();
            
            windows = new List<T>(nodeEditorWindows.Length);
            desiredDockNextTo = new[] { typeof(T), typeof(SceneView), typeof(DatabaseEditorWindow) };
            
            foreach (var window in nodeEditorWindows)
            {
                if (!TryRestoreWindow(window))
                    window.Close();
                else
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
            var editorDialogueData = EditorDialogueComponents.Database.LoadDialogue(window.DialogueName);
            if (editorDialogueData == null)
                return false;

            window.EditorData = editorDialogueData;
            
            return true;
        }

        private T CreateWindow(EditorDialogueData editorDialogueData)
        {
            var window = EditorWindow.CreateWindow<T>(desiredDockNextTo);
            windows.Add(window);

            window.EditorData = editorDialogueData;
            window.Show();

            if (!window.docked)
                window.position = new Rect(UniTalksPreferences.InitialDialogueEditorWindowPosition, UniTalksPreferences.InitialDialogueEditorWindowSize);

            return window;
        }
    }
}