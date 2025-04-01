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
            AddInputPort("In");

            for (int i = 0; i < data.OutputConnections.Count; i++)
                AddOutputPort(data.OutputConnections[i].Text);
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

        #endregion
        
        protected virtual void AddInputPort(string text)
        {
            VisualElement c = new()
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };

            c.Add(InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, null));
            c.Add(new Label(text)
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            });

            inputContainer.Add(c);
        }
        
        protected virtual void AddOutputPort(string text)
        {
            VisualElement c = new()
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };

            c.Add(new Button(() => RemoveButtonCallback(outputContainer.childCount - 1))
            {
                text = "x"
            });
            
            c.Add(new TextField()
            {
                value = text
            });

            c.Add(new VisualElement() { style = { flexGrow = 1f } });
            c.Add(InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null));
            
            outputContainer.Add(c);
        }
        
        protected virtual void AddButtonCallback()
        {
            AddOutputPort("New Choice");
        }

        protected virtual void RemoveButtonCallback(int index)
        {
            if (outputContainer.childCount <= 1)
                return;

            outputContainer.RemoveAt(index);
        }
        
        protected virtual void AddManipulators()
        {
            this.AddManipulator(new Dragger());
        }
    }
}