using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Editor
{
    public class TimerNodeView : NodeView<TimerNodeData>
    {
        protected override string Title => "Timer Node";

        public override void Initialize(EditorNodeData editorData, NodeData data, DialogueGraphView graphView)
        {
            base.Initialize(editorData, data, graphView);
            this.AddUSSClasses("choice-node");
        }
        
        public override void Draw()
        {
            base.Draw();
            CreateTimerInput();
        }

        private void CreateTimerInput()
        {
            FloatField timerInput = new("Timer")
            {
                value = data.Duration
            };

            timerInput.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue < 0f)
                {
                    timerInput.SetValueWithoutNotify(0f);
                    data.Duration = 0f;
                }
                else
                    data.Duration = evt.newValue;
            });
            
            extensionContainer.Insert(extensionContainer.childCount - 1, timerInput);
        }
    }
}