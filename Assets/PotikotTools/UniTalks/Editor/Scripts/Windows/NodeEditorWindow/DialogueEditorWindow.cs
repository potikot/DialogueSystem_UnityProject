using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Editor
{
    public class DialogueEditorWindowsManager : UniTalksWindowsManager<DialogueEditorWindow>
    {
        public static Dictionary<Type, (Type viewType, object[] args)> NodeTypes = new()
        {
            { typeof(SingleChoiceNodeData), (typeof(SingleChoiceNodeView), null) },
            { typeof(MultipleChoiceNodeData), (typeof(MultipleChoiceNodeView), null) },
            { typeof(TimerNodeData), (typeof(TimerNodeView), new object[] {0f} ) }
        };

        public static void AddNodeType<TD, TV>(params object[] args) where TD : NodeData where TV : Node, INodeView
        {
            var dataType = typeof(TD);
            var viewType = typeof(TV);

            if (viewType.BaseType == null)
            {
                UniTalksAPI.LogError(viewType.Name + " must be a subclass of NodeView<T>");
                return;
            }
            
            var genericArguments = viewType.BaseType.GetGenericArguments();
            if (genericArguments[0] != dataType)
            {
                UniTalksAPI.LogError(viewType.Name + " data type is not a subclass of NodeData");
                return;
            }
            
            if (!NodeTypes.TryAdd(dataType, (viewType, args)))
                UniTalksAPI.LogError(viewType.Name + " data type is already added");
        }
    }

    public class DialogueEditorWindow : BaseUniTalksEditorWindow
    {
        private DialogueGraphView _graph;
        private DialogueSettingsPanel _settngsPanel;

        protected override void OnEditorDataChanged()
        {
            AddGraphView();
            AddFloatingSettings();
        }

        // Смена названия
        protected override void ChangeTitle(string value)
        {
            titleContent = new GUIContent($"'{value}' Dialogue Editor");
        }

        private void CreateGUI()
        {
            var c = new VisualElement()
                .AddStyleSheets("Styles/NodeEditorWindow")
                .AddUSSClasses("body");

            c.Add(new Button(() =>
            {
                if (_graph == null)
                    return;

                editorData.GraphViewPosition = _graph.contentViewContainer.transform.position;
                editorData.GraphViewScale = _graph.contentViewContainer.transform.scale;

                SaveChanges();

            }) { text = "Save Dialogue", style = { alignSelf = Align.FlexEnd }});

            rootVisualElement.Add(c);
        }

        private void AddGraphView()
        {
            if (editorData == null)
                return;

            if (_graph != null)
            {
                _graph.RemoveFromHierarchy();
                _graph.OnChanged -= OnGraphChanged;
            }
            
            _graph = new DialogueGraphView(editorData);
            _graph.OnChanged += OnGraphChanged;
            _graph.StretchToParentSize();
            
            rootVisualElement.Insert(0, _graph);
        }

        private void OnGraphChanged()
        {
            hasUnsavedChanges = true;
        }

        private void AddFloatingSettings()
        {
            if (editorData == null)
                return;

            _settngsPanel?.RemoveFromHierarchy();
            _settngsPanel = new DialogueSettingsPanel(editorData);
            
            rootVisualElement.Add(_settngsPanel);
        }

        public override void DiscardChanges()
        {
            // TODO: discard changes
            
            hasUnsavedChanges = false;
        }

        public override void SaveChanges()
        {
            EditorDialogueComponents.Database.SaveDialogue(EditorData);
            hasUnsavedChanges = false;
        }
    }
}