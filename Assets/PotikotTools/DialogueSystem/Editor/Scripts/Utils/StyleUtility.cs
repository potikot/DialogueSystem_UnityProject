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

        // border width
        
        public static void BorderWidth(this VisualElement e, float top, float right, float bottom, float left)
        {
            e.style.borderTopWidth = top;
            e.style.borderRightWidth = right;
            e.style.borderBottomWidth = bottom;
            e.style.borderLeftWidth = left;
        }

        public static void BorderWidth(this VisualElement e, float vertical, float horizontal)
        {
            e.style.borderTopWidth = vertical;
            e.style.borderBottomWidth = vertical;

            e.style.borderRightWidth = horizontal;
            e.style.borderLeftWidth = horizontal;
        }
        
        public static void BorderWidth(this VisualElement e, float value)
        {
            e.style.borderTopWidth = value;
            e.style.borderRightWidth = value;
            e.style.borderBottomWidth = value;
            e.style.borderLeftWidth = value;
        }
        
        // border radius
        
        public static void BorderRadius(this VisualElement e, float topLeft, float topRight, float bottomleft, float bottomRight)
        {
            e.style.borderTopLeftRadius = topLeft;
            e.style.borderTopRightRadius = topRight;
            e.style.borderBottomLeftRadius = bottomleft;
            e.style.borderBottomRightRadius = bottomRight;
        }
        
        public static void BorderRadius(this VisualElement e, float left, float right)
        {
            e.style.borderTopLeftRadius = left;
            e.style.borderBottomLeftRadius = left;
            
            e.style.borderTopRightRadius = right;
            e.style.borderBottomRightRadius = right;
        }
        
        public static void BorderRadius(this VisualElement e, float value)
        {
            e.style.borderTopLeftRadius = value;
            e.style.borderTopRightRadius = value;
            e.style.borderBottomLeftRadius = value;
            e.style.borderBottomRightRadius = value;
        }
        
        // border color
        
        public static void BorderColor(this VisualElement e, Color top, Color right, Color bottom, Color left)
        {
            e.style.borderTopColor = top;
            e.style.borderRightColor = right;
            e.style.borderBottomColor = bottom;
            e.style.borderLeftColor = left;
        }

        public static void BorderColor(this VisualElement e, Color vertical, Color horizontal)
        {
            e.style.borderTopColor = vertical;
            e.style.borderBottomColor = vertical;

            e.style.borderRightColor = horizontal;
            e.style.borderLeftColor = horizontal;
        }
        
        public static void BorderColor(this VisualElement e, Color value)
        {
            e.style.borderTopColor = value;
            e.style.borderRightColor = value;
            e.style.borderBottomColor = value;
            e.style.borderLeftColor = value;
        }
        
        // margin
        
        public static void Margin(this VisualElement e, float top, float right, float bottom, float left)
        {
            e.style.marginTop = top;
            e.style.marginRight = right;
            e.style.marginBottom = bottom;
            e.style.marginLeft = left;
        }

        public static void Margin(this VisualElement e, float vertical, float horizontal)
        {
            e.style.marginTop = vertical;
            e.style.marginBottom = vertical;

            e.style.marginRight = horizontal;
            e.style.marginLeft = horizontal;
        }
        
        public static void Margin(this VisualElement e, float value)
        {
            e.style.marginTop = value;
            e.style.marginRight = value;
            e.style.marginBottom = value;
            e.style.marginLeft = value;
        }
        
        // padding
        
        public static void Padding(this VisualElement e, float top, float right, float bottom, float left)
        {
            e.style.paddingTop = top;
            e.style.paddingRight = right;
            e.style.paddingBottom = bottom;
            e.style.paddingLeft = left;
        }

        public static void Padding(this VisualElement e, float vertical, float horizontal)
        {
            e.style.paddingTop = vertical;
            e.style.paddingBottom = vertical;

            e.style.paddingRight = horizontal;
            e.style.paddingLeft = horizontal;
        }
        
        public static void Padding(this VisualElement e, float value)
        {
            e.style.paddingTop = value;
            e.style.paddingRight = value;
            e.style.paddingBottom = value;
            e.style.paddingLeft = value;
        }
    }
}