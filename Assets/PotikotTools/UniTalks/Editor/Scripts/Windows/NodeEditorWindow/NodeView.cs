using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PotikotTools.UniTalks.Editor
{
    public abstract class NodeView<T> : Node, INodeView where T : NodeData
    {
        public event Action OnChanged;
        
        protected EditorNodeData editorData;
        protected T data;
        
        protected DialogueGraphView graphView;

        protected virtual string Title => "Dialogue Node";
        protected virtual bool CanBeZeroOptions => false;
        
        public virtual void Initialize(EditorNodeData editorData, NodeData data, DialogueGraphView graphView)
        {
            this.editorData = editorData;
            this.data = data as T;
            this.graphView = graphView;
            
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        public NodeData GetData() => data;

        public virtual void OnDelete()
        {
            data.DialogueData.RemoveNode(data);
        }

        public virtual void Draw()
        {
            if (data == null)
            {
                UniTalksAPI.LogError("NodeData is null");
                return;
            }

            SetPosition(new Rect(editorData.position, Vector2.zero));
            title = Title;

            DrawChoiceButton();
            DrawPorts();
            DrawSpeakerText();
            DrawSpeakerDropdown();
            DrawAudioField();
            DrawCommandList();

            RefreshExpandedState();
        }

        #region Draw Helpers

        protected virtual void DrawChoiceButton()
        {
            var container = new VisualElement().AddUSSClasses("node__add-choice-btn");
            container.Add(new Button(OnAddChoice) { text = "Add Choice" });
            mainContainer.Insert(1, container);
        }

        protected virtual void DrawPorts()
        {
            AddInputPort();

            foreach (var connection in data.OutputConnections)
                AddOutputPort(connection);
        }

        protected virtual void AddInputPort()
        {
            var container = new VisualElement().AddUSSClasses("node__input-port-container");
            
            container.Add(InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, null));
            container.Add(new Label("In"));

            inputContainer.Add(container);
        }

        
        // TODO: port does not connectible
        protected virtual void AddOutputPort(ConnectionData connection)
        {
            var container = new VisualElement().AddUSSClasses("node__output-container");

            // Ports Container

            var portContainer = new VisualElement().AddUSSClasses("node__output-port-container");

            var deleteBtn = new Button(() => OnRemoveChoice(data.OutputConnections.IndexOf(connection)))
            {
                text = EditorSymbols.Cross
            };

            deleteBtn.AddUSSClasses(
                "round-btn",
                "node__output-port__delete-btn"
            );
            
            if (!CanBeZeroOptions && data.OutputConnections.Count <= 1)
                deleteBtn.style.display = DisplayStyle.None;

            var textField = new TextField
            {
                value = connection.Text,
                multiline = true
            };
            textField.AddUSSClasses("node__output-port__text-field");
            textField.Children().First().AddUSSClasses("node__output-port__text-input");
            textField.RegisterValueChangedCallback(evt => connection.Text = evt.newValue);

            var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null)
                .AddUSSClasses("node__output-port");

            portContainer.Add(deleteBtn);
            portContainer.Add(textField);
            // container.Add(new VisualElement { style = { flexGrow = 1f } });
            portContainer.Add(port);

            // CMD Container

            var commandsContainer = new VisualElement().AddUSSClasses("node__output-port-cmd-container");
            commandsContainer.Add(CreateCommandList(connection.Commands));

            container.Add(portContainer);
            container.Add(commandsContainer);

            if (outputContainer.childCount > 0)
                outputContainer.AddVerticalSpace(1f, Color.black);

            outputContainer.Add(container);
        }

        protected virtual void OnAddChoice()
        {
            if (!CanBeZeroOptions && outputContainer.childCount == 1)
                outputContainer.Q<Button>().style.display = DisplayStyle.Flex;
            
            var newConnection = new ConnectionData("New Choice", data, null);
            data.OutputConnections.Add(newConnection);
            AddOutputPort(newConnection);
            
            OnChanged?.Invoke();
        }
        
        protected virtual void OnRemoveChoice(int index)
        {
            if (index < 0 || index >= data.OutputConnections.Count || !CanBeZeroOptions && data.OutputConnections.Count <= 1)
                return;
            
            int viewIndex = index * 2;

            var port = outputContainer.Query<Port>().ToList()[index];
            if (port.connected)
            {
                graphView.DeleteElements(port.connections);
                // foreach (Edge edge in edgesToRemove)
                // {
                //     edge.input.Disconnect(edge);
                //     edge.output.Disconnect(edge);
                //     edge.RemoveFromHierarchy();
                //     graphView.RemoveElement(edge);
                // }
            }

            data.OutputConnections.RemoveAt(index);
            outputContainer.RemoveAt(viewIndex);
            if (outputContainer.childCount > 0)
                outputContainer.RemoveAt(viewIndex == 0 ? 0 : viewIndex - 1);

            if (!CanBeZeroOptions && outputContainer.childCount == 1)
                outputContainer.Q<Button>().style.display = DisplayStyle.None;
        }
        
        // TODO: text field does not stretch correctly because of multiline, but it works correct in output container
        protected virtual void DrawSpeakerText()
        {
            var container = new VisualElement().AddUSSClasses("node__speaker-text-container");
            container.Add(new Label("Speaker Text"));

            var textField = new TextField
            {
                value = data.Text,
                multiline = true
            };
            
            textField.AddUSSClasses("node__speaker-text");
            textField.RegisterValueChangedCallback(evt => data.Text = evt.newValue);

            container.Add(textField);
            extensionContainer.Add(container);
        }

        protected virtual void DrawSpeakerDropdown()
        {
            if (data.DialogueData.Speakers == null)
                return;

            var popup = GeneratePopup();
            popup.RegisterValueChangedCallback(_ => data.SpeakerIndex = popup.index - 1);
            extensionContainer.Add(popup);

            PopupField<string> GeneratePopup()
            {
                var speakerNames = new List<string> { "None" };
                speakerNames.AddRange(data.DialogueData.Speakers.Select((s, i) => $"{i + 1}. {s.Name}"));

                string currentSpeaker = data.GetSpeakerName();
                return new PopupField<string>("Speaker", speakerNames,
                    string.IsNullOrEmpty(currentSpeaker) ? "None" : $"{data.SpeakerIndex + 1}. {currentSpeaker}");
            }
        }

        protected virtual void DrawAudioField()
        {
            var audioField = new ObjectField("Audio")
            {
                objectType = typeof(AudioClip),
                value = DialoguesComponents.Database.LoadResource<AudioClip>(data.AudioResourceName)
            };

            audioField.RegisterValueChangedCallback(evt => HandleAudioChange(evt, audioField));
            extensionContainer.Add(audioField);
        }

        private void HandleAudioChange(ChangeEvent<Object> evt, ObjectField audioField)
        {
            if (evt.newValue == null)
                return;

            string assetPath = AssetDatabase.GetAssetPath(evt.newValue);
            string fileName = Path.GetFileName(assetPath);

            if (!FileUtility.IsDatabaseRelativePath(assetPath))
            {
                bool moveConfirmed = EditorUtility.DisplayDialog(
                    "Move asset",
                    $"Asset '{fileName}' is not under database directory. Must move into database",
                    "Ok", "Cancel");

                if (!moveConfirmed)
                {
                    audioField.SetValueWithoutNotify(evt.previousValue);
                    return;
                }

                FileUtility.MoveAssetToDatabase(evt.newValue.GetType(), assetPath, fileName);
            }

            data.AudioResourceName = Path.GetFileNameWithoutExtension(fileName);
        }

        protected virtual void DrawCommandList()
        {
            extensionContainer.Add(CreateCommandList(data.Commands));
        }

        protected virtual VisualElement CreateCommandList(ObservableList<CommandData> commands)
        {
            var foldout = new Foldout
            {
                text = "Commands",
                value = false
            };

            var addBtn = new Button(() => 
            {
                commands.Add(new CommandData());
                foldout.Add(CreateCommandElement(commands, commands.Count - 1));

                foldout.Q("unity-checkmark").RemoveUSSClasses("node__cmd-foldout-checkmark--hidden");
                foldout.value = true;
            })
            {
                text = EditorSymbols.Plus
            };

            addBtn.AddUSSClasses("round-btn");
            
            foldout.Q<Toggle>().Add(addBtn);
            foldout.Q<Label>().AddUSSClasses("node__cmd-label");
            
            for (int i = 0; i < commands.Count; i++)
                foldout.Add(CreateCommandElement(commands, i));

            if (commands.Count == 0)
                foldout.Q("unity-checkmark").AddUSSClasses("node__cmd-foldout-checkmark--hidden");
            
            return foldout;
        }
        
        protected virtual VisualElement CreateCommandElement(ObservableList<CommandData> commands, int index)
        {
            var cmd = commands[index];
            
            var foldout = new Foldout
            {
                text = string.IsNullOrEmpty(cmd.Text) ? GetCommandName(index + 1) : cmd.Text,
                value = false
            };

            var cmdInput = new TextField { value = cmd.Text };
            cmdInput.RegisterValueChangedCallback(evt =>
            {
                cmd.Text = evt.newValue;
                foldout.text = string.IsNullOrEmpty(evt.newValue) ? GetCommandName(commands.IndexOf(cmd) + 1) : evt.newValue;
            });

            var execOrder = new EnumField("Execution Order", cmd.ExecutionOrder);
            execOrder.RegisterValueChangedCallback(evt => cmd.ExecutionOrder = (CommandExecutionOrder)evt.newValue);

            var delayField = new FloatField("Delay")
            {
                value = cmd.Delay,
                tooltip = "In seconds"
            };
            delayField.RegisterValueChangedCallback(evt =>
            {
                float newDelay = Mathf.Max(0f, evt.newValue);
                delayField.SetValueWithoutNotify(newDelay);
                cmd.Delay = newDelay;
            });

            var deleteBtn = new Button(() =>
            {
                commands.Remove(cmd);
                if (commands.Count == 0 && foldout.parent is Foldout f)
                {
                    f.value = false;
                    f.Q("unity-checkmark").AddUSSClasses("node__cmd-foldout-checkmark--hidden");
                }
                
                foldout.RemoveFromHierarchy();
            })
            {
                text = EditorSymbols.Cross
            };

            deleteBtn.AddUSSClasses("round-btn");
            
            foldout.Add(cmdInput);
            foldout.Add(execOrder);
            foldout.Add(delayField);
            foldout.Q<Toggle>().Add(deleteBtn);

            commands.OnElementRemoved += OnCommandRemoved;
            foldout.RegisterCallback<DetachFromPanelEvent>(_ => commands.OnElementRemoved -= OnCommandRemoved);
            
            return foldout;

            void OnCommandRemoved(CommandData c)
            {
                if (string.IsNullOrEmpty(cmd.Text))
                    foldout.text = GetCommandName(commands.IndexOf(cmd) + 1);
            }
            
            string GetCommandName(int i) => $"Command {i}";
        }

        #endregion

        protected virtual void OnGeometryChanged(GeometryChangedEvent evt)
        {
            editorData.position = evt.newRect.position;
        }

        protected virtual void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
    }
}