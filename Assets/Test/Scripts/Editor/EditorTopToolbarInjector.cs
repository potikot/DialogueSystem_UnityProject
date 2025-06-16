using System;
using System.Reflection;
using PotikotTools.UniTalks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.Editor
{
    [InitializeOnLoad]
    public static class EditorTopToolbarInjector
    {
        private const string ToolbarName = "ToolbarZoneRightAlign";
        private static readonly Type ToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");

        private static VisualElement Container;

        static EditorTopToolbarInjector()
        {
            EditorApplication.delayCall += () =>
            {
                AddContainer();
                AddButton(
                    EditorGUIUtility.IconContent("d_Refresh").image as Texture2D,
                    "Request Script Reloading",
                    EditorUtility.RequestScriptReload
                );
            };
        }

        public static void AddElement(VisualElement element) => Container.Add(element);

        public static void AddButton(string text, string tooltip, Action action) => AddButton(text, null, tooltip, action);
        public static void AddButton(Texture2D texture, string tooltip, Action action) => AddButton(null, texture, tooltip, action);

        private static void AddButton(string text, Texture2D texture, string tooltip, Action action)
        {
            var button = new ToolbarButton(() =>
            {
                if (!Application.isPlaying)
                    action?.Invoke();
            })
            {
                text = text,
                tooltip = tooltip,
                style =
                {
                    width = 32f,
                    height = 20f,
                    alignItems = Align.Center
                }
            };

            if (texture)
            {
                var image = new VisualElement
                {
                    style =
                    {
                        backgroundImage = texture,
                        height = 16f,
                        width = 16f
                    }
                };

                button.Add(image);
            }
            
            AddElement(button);
        }
        
        private static void AddContainer()
        {
            var toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
            
            var currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
            if (currentToolbar == null)
                return;

            var mRootFieldInfo = currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            if (mRootFieldInfo == null)
                return;

            var mRoot = mRootFieldInfo.GetValue(currentToolbar) as VisualElement;
            var toolbarElement = mRoot.Q(ToolbarName);

            Container = new VisualElement
            {
                style =
                {
                    left = -8f,
                    flexGrow = 1f,
                    flexDirection = FlexDirection.Row,
                }
            };

            toolbarElement.Add(Container);
        }
    }
}
