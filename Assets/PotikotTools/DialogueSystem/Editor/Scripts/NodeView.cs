using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem.Editor
{
    public abstract class NodeView<T> : Node, INodeView
        where T : NodeData
    {
        protected EditorNodeData editorData;
        protected T data;

        public T Data => data;

        public virtual void Initialize(EditorNodeData editorData, NodeData data)
        {
            this.editorData = editorData;
            this.data = data as T;

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            
            AddManipulators();
        }

        public NodeData GetData() => data;
        
        public virtual void Delete()
        {
            data.DialogueData.RemoveNode(data);
        }

        #region Draw

        public virtual void Draw()
        {
            if (data == null)
            {
                DL.LogError("NodeData is null");
                return;
            }

            SetPosition(new Rect(editorData.position, Vector2.zero));
            
            title = "Dialogue Node";

            CreatePorts();
            CreateAddButton();
            CreateSpeakerTextInput();
            CreateSpeakerIndexInput();
            CreateAudioInput();
            CreateCommandsInput();

            extensionContainer.style.backgroundColor = new Color(0.2470588f, 0.2470588f, 0.2470588f, 0.8039216f);
            RefreshExpandedState();
        }

        protected virtual void CreatePorts()
        {
            AddInputPort();

            for (int i = 0; i < data.OutputConnections.Count; i++)
                AddOutputPort(data.OutputConnections[i]);
        }

        protected virtual void CreateSpeakerTextInput()
        {
            VisualElement speakerTextContainer = new();
            speakerTextContainer.Add(new Label("Speaker Text"));
            TextField speakerTextInput = new()
            {
                value = data.Text
            };
            speakerTextInput.RegisterValueChangedCallback(evt => data.Text = evt.newValue);
            speakerTextContainer.Add(speakerTextInput);
            
            extensionContainer.Add(speakerTextContainer);
        }

        protected virtual void CreateSpeakerIndexInput()
        {
            if (data.DialogueData.Speakers == null)
                return;
            
            List<string> speakerNames = new(data.DialogueData.Speakers.Count + 1) { "None" };
            speakerNames.AddRange(data.DialogueData.Speakers.Select(s => s.Name));
        
            PopupField<string> speakerIndexInput = new
            (
                "Speaker",
                speakerNames,
                data.GetSpeakerName() ?? "None"
                
            );
            
            speakerIndexInput.RegisterValueChangedCallback(evt => data.SpeakerIndex = speakerIndexInput.index - 1);
        
            extensionContainer.Add(speakerIndexInput);
        }

        protected virtual void CreateAudioInput()
        {
            TextField audioNameInput = new("Audio Name")
            {
                value = data.AudioResourceName
            };

            audioNameInput.RegisterValueChangedCallback(evt =>
            {
                data.AudioResourceName = evt.newValue;
            });
            
            extensionContainer.Add(audioNameInput);
        }

        protected virtual void CreateCommandsInput()
        {
            Foldout foldout = new()
            {
                text = "Commands",
                value = false
            };

            foldout.Add(new Button(() =>
            {
                CommandData commandData = new();
                data.Commands.Add(commandData);
                int index = data.Commands.Count - 1;

                foldout.Add(CreateListElement(data.Commands, index, () =>
                {
                    int index = data.Commands.IndexOf(commandData);
                    
                    data.Commands.RemoveAt(index);
                    foldout.RemoveAt(index + 1);
                }));
            })
            {
                text = "Add Command"
            });

            for (int i = 0; i < data.Commands.Count; i++)
            {
                VisualElement el = CreateListElement(data.Commands, i, () =>
                {
                    data.Commands.RemoveAt(i);
                    foldout.RemoveAt(i + 1);
                });
                foldout.Add(el);
            }

            extensionContainer.Add(foldout);
        }

        private VisualElement CreateListElement(List<CommandData> commands, int index, Action deleteAction)
        {
            CommandData commandData = commands[index];

            Foldout foldout = new()
            {
                text = commandData.Command ?? $"Command {index + 1}",
                value = false
            };
            
            TextField commandInput = new()
            {
                value = commandData.Command
            };

            commandInput.RegisterValueChangedCallback(evt =>
            {
                commandData.Command = evt.newValue;

                if (string.IsNullOrEmpty(evt.newValue))
                    foldout.text = $"Command {index + 1}";
                else
                    foldout.text = evt.newValue;
            });

            EnumField executionOrderInput = new("Execution Order", commandData.ExecutionOrder);
            executionOrderInput.RegisterValueChangedCallback(evt => commandData.ExecutionOrder = (CommandExecutionOrder) evt.newValue);

            FloatField delayInput = new("Delay")
            {
                value = commandData.Delay
            };
            
            delayInput.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue < 0f)
                    delayInput.SetValueWithoutNotify(0f);

                commandData.Delay = delayInput.value;
            });

            foldout.Add(commandInput);
            foldout.Add(executionOrderInput);
            foldout.Add(delayInput);
            foldout.Q<Toggle>().Add(new Button(deleteAction)
            {
                text = "x",
                style =
                {
                    paddingLeft = 5f,
                    paddingRight = 5f
                }
            });

            return foldout;
        }
        
        protected virtual void CreateAddButton()
        {
            VisualElement c = new()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    borderTopWidth = 1f,
                    borderTopColor = new Color(0.2f, 0.2f, 0.2f),
                    backgroundColor = new Color(0.2470588f, 0.2470588f, 0.2470588f, 0.8039216f)
                }
            };

            c.Add(new Button(AddButtonCallback)
            {
                text = "Add",
                style = { flexGrow = 1f }
            });
            
            mainContainer.Insert(1, c);
        }

        protected virtual void AddInputPort()
        {
            VisualElement c = new()
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };

            c.Add(InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, null));
            c.Add(new Label("In")
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            });

            inputContainer.Add(c);
        }
        
        protected virtual void AddOutputPort(ConnectionData connectionData)
        {
            VisualElement c = new()
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };

            c.Add(new Button(() => RemoveButtonCallback(data.OutputConnections.IndexOf(connectionData)))
            {
                text = "x"
            });

            TextField textField = new()
            {
                value = connectionData.Text
            };
            
            textField.RegisterValueChangedCallback(evt => connectionData.Text = evt.newValue);
            
            c.Add(textField);

            c.Add(new VisualElement() { style = { flexGrow = 1f } });
            c.Add(InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null));
            
            outputContainer.Add(c);
        }
        
        #endregion

        protected virtual void OnGeometryChanged(GeometryChangedEvent evt)
        {
            editorData.position = evt.newRect.position;
        }
        
        protected virtual void AddButtonCallback()
        {
            ConnectionData connectionData = new("New Choice", data, null);
            data.OutputConnections.Add(connectionData);

            AddOutputPort(connectionData);
        }

        protected virtual void RemoveButtonCallback(int index)
        {
            if (index < 0 || index >= data.OutputConnections.Count || data.OutputConnections.Count <= 1)
                return;

            data.OutputConnections.RemoveAt(index);
            outputContainer.RemoveAt(index);
        }
        
        protected virtual void AddManipulators()
        {
            this.AddManipulator(new Dragger());
        }
    }
}