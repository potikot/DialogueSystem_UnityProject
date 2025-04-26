using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem
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
            
            tf.Children().First().Add(placeholderLabel);

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

        public static VisualElement AddVerticalSpace(this VisualElement e, float height, Color color = new())
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
        
        public static VisualElement AddHorizontalSpace(this VisualElement e, float width, Color color = new())
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
    }
}