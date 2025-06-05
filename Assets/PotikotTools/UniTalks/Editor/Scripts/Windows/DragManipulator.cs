using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Editor
{
    public class DragManipulator : MouseManipulator
    {
        private Vector2 startMousePosition;
        private Vector2 startPanelPosition;
        private bool dragging;

        public DragManipulator()
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (CanStartManipulation(evt))
            {
                startMousePosition = evt.mousePosition;
                startPanelPosition = target.resolvedStyle.position == Position.Absolute
                    ? new Vector2(target.resolvedStyle.left, target.resolvedStyle.top)
                    : target.transform.position;

                dragging = true;
                target.CaptureMouse();
                evt.StopPropagation();
            }
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (!dragging || !target.HasMouseCapture()) return;

            Vector2 delta = evt.mousePosition - startMousePosition;
            Vector2 newPos = startPanelPosition + delta;

            // Clamp to parent (usually rootVisualElement)
            var parent = target.parent;
            if (parent != null)
            {
                float panelWidth = target.resolvedStyle.width;
                float panelHeight = target.resolvedStyle.height;
                float parentWidth = parent.resolvedStyle.width;
                float parentHeight = parent.resolvedStyle.height;

                // Clamp X and Y
                newPos.x = Mathf.Clamp(newPos.x, 0, parentWidth - panelWidth);
                newPos.y = Mathf.Clamp(newPos.y, 0, parentHeight - panelHeight);
            }

            target.style.left = newPos.x;
            target.style.top = newPos.y;

            evt.StopPropagation();
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            if (dragging && CanStopManipulation(evt))
            {
                dragging = false;
                target.ReleaseMouse();
                evt.StopPropagation();
            }
        }
    }
}