using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem
{
    public static class StyleUtility
    {
        public static VisualElement AddStyles(this VisualElement e, params string[] styles)
        {
            foreach (string style in styles)
                e.styleSheets.Add(Resources.Load<StyleSheet>(style));
            
            return e;
        }

        public static VisualElement AddClasses(this VisualElement e, params string[] classes)
        {
            foreach (string className in classes)
                e.AddToClassList(className);
            
            return e;
        }
    }
}