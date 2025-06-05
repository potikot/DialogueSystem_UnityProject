using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Editor
{
    [InitializeOnLoad]
    public static class InspectorUtilityInitializer
    {
        static InspectorUtilityInitializer()
        {
            InspectorUtility.RegisterRenderer<NodeData>(NodeDataRenderer);
            InspectorUtility.RegisterRenderer<CommandData>(UniversalRenderer);
            InspectorUtility.RegisterRenderer<ConnectionData>(UniversalRenderer);
        }

        private static VisualElement NodeDataRenderer(string name, Type type, object value, Action<object> setValue)
        {
            VisualElement e = value switch
            {
                null => new Label($"<color=#ea0>No '{name}' connection</color>")
                {
                    style = { marginLeft = 3 }
                },
                _ => new IntegerField(name)
                {
                    value = ((NodeData)value).Id,
                    isReadOnly = true,
                    labelElement =
                    {
                        pickingMode = PickingMode.Ignore
                    }
                }
            };
            
            return e;
        }
        
        private static VisualElement UniversalRenderer(string name, Type type, object value, Action<object> setValue)
        {
            if (type.IsClass && value == null)
                return new Label($"<color=#ea0>'Null' {name} </color>")
                {
                    style = { marginLeft = 3 }
                };
            
            var foldout = new Foldout { text = name, value = false };
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach (var fieldInfo in fieldInfos)
                foldout.Add(InspectorUtility.CreateField(value, fieldInfo));
            
            return foldout;
        }
    }
}