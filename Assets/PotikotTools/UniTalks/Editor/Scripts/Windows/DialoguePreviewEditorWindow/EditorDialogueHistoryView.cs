using System.Collections.Generic;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Editor
{
    public class EditorDialogueHistoryView : VisualElement
    {
        private ScrollView _scroll;
        
        private List<NodeData> _history;
        
        public EditorDialogueHistoryView()
        {
            _history = new List<NodeData>();
            _scroll = new ScrollView();
            _scroll.contentContainer.style.flexDirection = FlexDirection.ColumnReverse;
            Add(_scroll);
            
            this.AddUSSClasses("dialogue-history-container");
        }

        public void AddNode(NodeData nodeData)
        {
            if (nodeData == null)
                return;
            
            _history.Add(nodeData);

            var option = new EditorOptionView();
            option.SetText($"{nodeData.Id} : {nodeData.Text}");
            option.OnSelected(() =>
            {
                
            });
            
            _scroll.Add(option);
        }

        // public void Clear()
        // {
        //     _history.Clear();
        //     _scroll.Clear();
        // }
    }
}