using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PotikotTools.UniTalks.Editor
{
    public static class InspectorUtility
    {
        private static readonly Dictionary<Type, Func<string, Type, object, Action<object>, VisualElement>> Renderers;

        private static readonly Func<VisualElement, VisualElement> DefaultRenderer = e =>
        {
            // e?.SetEnabled(false);
            return e;
        };
        
        static InspectorUtility()
        {
            Renderers = new Dictionary<Type, Func<string, Type, object, Action<object>, VisualElement>>
            {
                { typeof(string), (name, type, value, setValue) =>
                {
                    var e = new TextField(name)
                    {
                        value = (string)value,
                        isReadOnly = true
                    };
                    // e.RegisterValueChangedCallback(evt => setValue?.Invoke(evt.newValue));
                    
                    // e.labelElement.style.marginRight = 5;
                    // e.labelElement.style.minWidth = 0;
                    // e.labelElement.style.flexGrow = 0;
                    
                    return e;
                }},
                { typeof(int), (name, type, value, setValue) =>
                {
                    var e = new IntegerField(name)
                    {
                        value = (int)value,
                        isReadOnly = true,
                        labelElement =
                        {
                            pickingMode = PickingMode.Ignore
                        }
                    };
                    // e.RegisterValueChangedCallback(evt => setValue?.Invoke(evt.newValue));
                    
                    return e;
                }},
                { typeof(float), (name, type, value, setValue) =>
                {
                    var e = new FloatField(name)
                    {
                        value = (float)value,
                        isReadOnly = true,
                        labelElement =
                        {
                            pickingMode = PickingMode.Ignore
                        }
                    };
                    // e.RegisterValueChangedCallback(evt => setValue?.Invoke(evt.newValue));
                    
                    return e;
                }},
                { typeof(Enum), (name, type, value, setValue) =>
                {
                    var e = new EnumField(name, (Enum)value)
                    {
                        value = (Enum)value,
                        pickingMode = PickingMode.Ignore
                    };
                    
                    // e.RegisterValueChangedCallback(evt => setValue?.Invoke(evt.newValue));

                    return e;
                }},
                { typeof(Object), (name, type, value, setValue) =>
                {
                    var e = new ObjectField(name)
                    {
                        objectType = type,
                        value = (Object)value,
                    };
                    e.Q(null, "unity-object-field__selector").pickingMode = PickingMode.Ignore;
                    // e.RegisterValueChangedCallback(evt => setValue?.Invoke(evt.newValue));

                    return e;
                }},
                { typeof(List<>), (name, type, value, setValue) =>
                {
                    return ListViewUtility.Create(value as IList, name);
                }}
            };
        }
        
        public static VisualElement CreateInspectorWindow(object target, VisualElement root)
        {
            if (target == null || root == null)
                return null;
            
            List<FieldInfo> fields = GetAllInstanceFields(target.GetType());
            foreach (FieldInfo field in fields)
                root.Add(CreateField(target, field));
            
            return root;
        }
        
        public static VisualElement CreateInspectorWindow(object target)
        {
            return CreateInspectorWindow(target, new ScrollView(){});
        }

        public static VisualElement CreateField(object target, FieldInfo field)
        {
            if (field == null || target == null)
                return null;
            
            Type listType = typeof(List<>);
            bool isList = false;

            if (field.FieldType.IsGenericType)
            {
                Type gtd = field.FieldType.GetGenericTypeDefinition();
                isList = gtd == listType || gtd == typeof(ObservableList<>);
            }

            string fieldName = field.Name.TrimStart('_');
            if (fieldName.Length > 0)
                fieldName = char.ToUpper(fieldName[0]) + fieldName[1..];
            else
                fieldName = field.FieldType.Name;
            
            if (isList)
            {
                if (Renderers.TryGetValue(listType, out var renderer))
                    return DefaultRenderer(renderer(fieldName, field.FieldType, field.GetValue(target), null));
            }
            else
            {
                if (field.FieldType.IsEnum)
                {
                    if (Renderers.TryGetValue(typeof(Enum), out var enumRenderer))
                        return DefaultRenderer(enumRenderer(fieldName, field.FieldType, field.GetValue(target), null));
                }
                else
                {
                    Type unityObjectType = typeof(Object);
                    bool isUnityObject = field.FieldType.IsSubclassOf(unityObjectType);
                    Type rendererType = isUnityObject ? unityObjectType : field.FieldType;

                    if (Renderers.TryGetValue(rendererType, out var renderer))
                        return DefaultRenderer(renderer(fieldName, field.FieldType, field.GetValue(target), null));
                }
            }

            // Debug.LogWarning($"Cannot create field view for type '{field.FieldType.Name}'");
            return null;
        }

        public static VisualElement CreateField(string name, Type type, object value)
        {
            if (Renderers.TryGetValue(type, out var renderer))
                return DefaultRenderer(renderer(name, type, value, null));

            return null;
        }
        
        public static bool TryGetRenderer(Type type, out Func<string, Type, object, Action<object>, VisualElement> renderer) => Renderers.TryGetValue(type, out renderer);

        public static bool TryGetRenderer<T>(out Func<string, Type, object, Action<object>, VisualElement> renderer) => TryGetRenderer(typeof(T), out renderer);

        public static void RegisterRenderer(Type type, Func<string, Type, object, Action<object>, VisualElement> renderer)
        {
            if (renderer == null)
                return;
            
            Renderers[type] = renderer;
        }
        
        public static void RegisterRenderer<T>(Func<string, Type, object, Action<object>, VisualElement> renderer) => RegisterRenderer(typeof(T), renderer);
        
        public static bool UnregisterRenderer(Type type) => Renderers.Remove(type);
        
        public static bool UnregisterRenderer<T>() => UnregisterRenderer(typeof(T));
        
        public static List<FieldInfo> GetAllInstanceFields(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            var fields = new List<FieldInfo>();
            
            while (type != null && type != typeof(object))
            {
                fields.AddRange(type.GetFields(flags));
                type = type.BaseType;
            }
            
            return fields;
        }
    }
}