using System.Collections.Generic;
using System.Linq;
using PotikotTools.UniTalks.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Demo.Editor
{
    public class ChatNodeView : NodeView<ChatNodeData>
    {
        private VisualElement _chainedOutputPortContainer;
        
        protected override string Title => "Chat Node";
        protected override bool CanBeZeroOptions => true;

        public override void Initialize(EditorNodeData editorData, NodeData data, DialogueGraphView graphView)
        {
            base.Initialize(editorData, data, graphView);
            this.AddUSSClasses("choice-node");
        }

        public override void OnConnected(Edge edge)
        {
            if (edge.output.node is ChatNodeView outputNode
                && edge.input.node is INodeView inputNode)
            {
                if (outputNode.data.NextChainedNode == null)
                    outputNode.data.NextChainedNode ??= new ConnectionData("$_$", outputNode.data, inputNode.GetData());
                else
                {
                    outputNode.data.NextChainedNode.From = outputNode.data;
                    outputNode.data.NextChainedNode.To = inputNode.GetData();
                }
            }
        }

        public override void OnDisconnected(Edge edge)
        {
            if (edge.output.node is ChatNodeView outputNode)
            {
                outputNode.data.NextChainedNode = null;
            }
        }

        public override void DrawEdges(UQueryState<Node> nodes)
        {
            if (data.NextChainedNode == null || data.HasOutputConnections)
                return;
            
            var node = nodes.FirstOrDefault(Predicate);
            if (node == null)
                return;

            Port toPort = node.inputContainer.Q<Port>();
            Port fromPort = outputContainer.Q<Port>();
            graphView.AddElement(fromPort.ConnectTo(toPort));

            bool Predicate(Node node)
            {
                if (node is INodeView nodeView)
                    return nodeView.GetData().Id == data.NextChainedNode.To.Id;
                
                return false;
            }
        }
        
        public override void Draw()
        {
            base.Draw();
            DrawDelayField();

            if (data.OutputConnections.Count == 0)
                AddChainedOutputPort();
            
            RefreshExpandedState();
        }

        private void AddChainedOutputPort()
        {
            _chainedOutputPortContainer = new VisualElement().AddUSSClasses("node__output-container");
            _chainedOutputPortContainer.style.alignItems = Align.FlexEnd;
            var portContainer = new VisualElement().AddUSSClasses("node__output-port-container");
            
            var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null)
                .AddUSSClasses("node__output-port");
            port.Q<Label>().style.display = DisplayStyle.Flex;
            port.portName = "Out";
            port.viewDataKey = "Chained";
            
            portContainer.Add(port);
            _chainedOutputPortContainer.Add(portContainer);
            outputContainer.Add(_chainedOutputPortContainer);
        }
        
        private void DrawDelayField()
        {
            var field = new FloatField("Delay")
            {
                value = data.Delay
            };

            field.RegisterValueChangedCallback(OnValueChanged);
            field.RegisterCallback<DetachFromPanelEvent>(_ => field.UnregisterValueChangedCallback(OnValueChanged));
            extensionContainer.Insert(1, field);
            void OnValueChanged(ChangeEvent<float> evt) => data.Delay = evt.newValue;
        }
        
        protected override void DrawSpeakerDropdown()
        {
            if (data.DialogueData.Speakers == null)
                return;

            var popup = GeneratePopup();
            popup.RegisterValueChangedCallback(OnValueChanged);
            popup.RegisterCallback<DetachFromPanelEvent>(_ => popup.UnregisterValueChangedCallback(OnValueChanged));
            data.DialogueData.Speakers.CollectionChanged += (_, _) => RegeneratePopup();
            foreach (var speaker in data.DialogueData.Speakers)
                speaker.OnNameChanged += _ => RegeneratePopup();
            
            extensionContainer.Add(popup);

            PopupField<string> GeneratePopup()
            {
                var speakerNames = new List<string> { "None" };
                speakerNames.AddRange(data.DialogueData.Speakers.Select((s, i) => $"{i + 1}. {s.Name}"));

                string currentSpeaker = data.GetSpeakerName();
                return new PopupField<string>("Speaker", speakerNames,
                    string.IsNullOrEmpty(currentSpeaker) ? "None" : $"{data.SpeakerIndex + 1}. {currentSpeaker}");
            }

            void OnValueChanged(ChangeEvent<string> newValue)
            {
                data.SpeakerIndex = popup.index - 1;
            }

            void RegeneratePopup()
            {
                popup.RemoveFromHierarchy();
                popup = GeneratePopup();
                popup.RegisterValueChangedCallback(OnValueChanged);
                popup.RegisterCallback<DetachFromPanelEvent>(_ => popup.UnregisterValueChangedCallback(OnValueChanged));
                extensionContainer.Insert(2, popup);
                RefreshExpandedState();
            }
        }

        protected override void OnAddChoice()
        {
            base.OnAddChoice();

            if (_chainedOutputPortContainer != null)
            {
                _chainedOutputPortContainer.RemoveFromHierarchy();
                _chainedOutputPortContainer = null;
                data.NextChainedNode = null;
            }
        }

        protected override void OnRemoveChoice(int index)
        {
            base.OnRemoveChoice(index);

            if (data.HasOutputConnections)
                AddChainedOutputPort();
        }
    }
}
