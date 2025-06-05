using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks
{
    public static class StyleUtility
    {
        public static T AddStyleSheets<T>(this T e, params string[] styles) where T : VisualElement
        {
            foreach (string style in styles)
                e.styleSheets.Add(Resources.Load<StyleSheet>(style));
            
            return e;
        }

        public static T RemoveStyleSheets<T>(this T e, params string[] styles) where T : VisualElement
        {
            foreach (string style in styles)
                e.styleSheets.Remove(Resources.Load<StyleSheet>(style));
            
            return e;
        }
        
        public static T AddUSSClasses<T>(this T e, params string[] classes) where T : VisualElement
        {
            foreach (string className in classes)
                e.AddToClassList(className);
            
            return e;
        }

        public static T RemoveUSSClasses<T>(this T e, params string[] classes) where T : VisualElement
        {
            foreach (string className in classes)
                e.RemoveFromClassList(className);
            
            return e;
        }
    }
}