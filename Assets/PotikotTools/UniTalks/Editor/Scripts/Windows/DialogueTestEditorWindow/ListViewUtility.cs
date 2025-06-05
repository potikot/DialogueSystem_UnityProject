using System.Collections;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Editor
{
    public static class ListViewUtility
    {
        public static Foldout Create(IList source, string title = null)
        {
            var foldout = new Foldout { text = title, value = false };

            int count = source.Count;

            if (count > 0)
            {
                var elementType = source.GetType().GetGenericArguments()[0];
                for (int i = 0; i < count; i++)
                    foldout.Add(InspectorUtility.CreateField($"Element {i}", elementType, source[i]));
            }
            else
            {
                foldout.Q("unity-checkmark").style.display = DisplayStyle.None;
                foldout.Q<Label>().style.marginLeft = 0f;
                foldout.text = $"<color=#ea0>'Empty' {title} </color>";
            }
            
            return foldout;
        }
    }
}