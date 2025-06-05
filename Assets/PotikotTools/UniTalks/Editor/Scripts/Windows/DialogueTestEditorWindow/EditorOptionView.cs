using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Editor
{
    public class EditorOptionView : VisualElement
    {
        private Label _label;
        private Action _onSelected;

        private bool _isMouseClicked;
        
        public EditorOptionView()
        {
            this.AddUSSClasses("option-view");
            
            _label = new Label().AddUSSClasses("option-view__label");
            Add(_label);
            
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                RegisterCallback<MouseDownEvent>(MouseDownEventCallback);
                RegisterCallback<MouseUpEvent>(MouseUpEventCallback);
                // RegisterCallback<MouseCaptureOutEvent>(MouseCaptureOutEventCallback);
            });
            
            RegisterCallback<DetachFromPanelEvent>(_ =>
            {
                UnregisterCallback<MouseDownEvent>(MouseDownEventCallback);
                UnregisterCallback<MouseUpEvent>(MouseUpEventCallback);
                // UnregisterCallback<MouseCaptureOutEvent>(MouseCaptureOutEventCallback);
            });
        }
        
        public void Show() => style.display = DisplayStyle.Flex;
        public void Hide() => style.display = DisplayStyle.None;
        
        public void SetText(string text)
        {
            _label.text = text;
        }

        public void OnSelected(Action onSelected)
        {
            _onSelected = onSelected;
        }

        private void MouseDownEventCallback(MouseDownEvent evt)
        {
            if (evt.button != 0)
                return;

            _isMouseClicked = true;
            this.CaptureMouse();
            this.AddUSSClasses("option-view--pressed");
        }
        
        private void MouseUpEventCallback(MouseUpEvent evt)
        {
            if (evt.button != 0 || !_isMouseClicked)
                return;

            _isMouseClicked = false;
            
            this.ReleaseMouse();
            this.RemoveUSSClasses("option-view--pressed");

            Vector2 globalMousePosition = this.LocalToWorld(evt.localMousePosition);
            if (!worldBound.Contains(globalMousePosition))
                return;
            
            _onSelected?.Invoke();
        }
    }
}