using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks
{
    public static class VisualElementExtentions
    {
        // TODO: does not work with fields that contains label
        public static TextField AddPlaceholder(this TextField tf, string text)
        {
            var placeholderLabel = new Label(text)
            {
                pickingMode = PickingMode.Ignore,
                style =
                {
                    color = new Color(0.6f, 0.6f, 0.6f, 0.75f),
                    position = Position.Absolute
                }
            };
            
            tf.Children().Last().Add(placeholderLabel);

            tf.RegisterCallback<FocusInEvent>(evt => placeholderLabel.style.display = DisplayStyle.None);
            tf.RegisterCallback<FocusOutEvent>(evt =>
            {
                if (string.IsNullOrEmpty(tf.text))
                    placeholderLabel.style.display = DisplayStyle.Flex;
            });
            
            if (!string.IsNullOrEmpty(tf.text))
                placeholderLabel.style.display = DisplayStyle.None;
            
            return tf;
        }

        public static T AddVerticalSpace<T>(this T e, float height, Color color = new()) where T : VisualElement
        {
            e.Add(new VisualElement()
            {
                style =
                {
                    height = height,
                    backgroundColor = color
                }
            });
            
            return e;
        }
        
        public static T InsertVerticalSpace<T>(this T e, int index, float height, Color color = new()) where T : VisualElement
        {
            e.Insert(index, new VisualElement()
            {
                style =
                {
                    height = height,
                    backgroundColor = color
                }
            });
            
            return e;
        }
        
        public static T AddHorizontalSpace<T>(this T e, float width, Color color = new()) where T : VisualElement
        {
            e.Add(new VisualElement()
            {
                style =
                {
                    width = width,
                    backgroundColor = color
                }
            });
            
            return e;
        }
        
        public static T InsertHorizontalSpace<T>(this T e, int index, float width, Color color = new()) where T : VisualElement
        {
            e.Insert(index, new VisualElement()
            {
                style =
                {
                    width = width,
                    backgroundColor = color
                }
            });
            
            return e;
        }
    }
}