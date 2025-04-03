using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem
{
    public class TimerNodeView : NodeView<TimerNodeData>
    {
        public override void Draw()
        {
            base.Draw();
            title = "Timer Node";
            
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
            
            extensionContainer.Add(timerInput);
        }
    }
}