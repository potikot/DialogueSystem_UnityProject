using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem
{
    public abstract class NodeView<T> : Node, INodeView
        where T : NodeData
    {
        protected EditorNodeData editorData;
        protected T data;

        public T Data => data;

        public virtual void Initialize(NodeData nodeData)
        {
            data = nodeData as T;

            AddManipulators();
        }

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

            title = "Dialogue Node";

            CreatePorts();
            CreateAddButton();
            CreateSpeakerTextInput();
            CreateSpeakerIndexInput();
            CreateAudioInput();

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
            
            c.Add(new TextField()
            {
                value = connectionData.Text
            });

            c.Add(new VisualElement() { style = { flexGrow = 1f } });
            c.Add(InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null));
            
            outputContainer.Add(c);
        }
        
        #endregion
        
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