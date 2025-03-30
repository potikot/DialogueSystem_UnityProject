using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem
{
    public abstract class NodeView<T> : Node where T : NodeData
    {
        protected EditorNodeData editorData;
        protected T data;

        public T Data => data;
        
        public virtual void Initialize(NodeData nodeData)
        {
            data = nodeData as T;

            AddManipulators();
        }

        public virtual void Draw()
        {
            if (data == null)
            {
                DL.LogError("NodeData is null");
                return;
            }

            title = "Dialogue Node";

            AddPort("Input", Direction.Input);

            for (int i = 0; i < data.OutputConnections.Count; i++)
                AddPort(data.OutputConnections[i].Text, Direction.Output);
            
            mainContainer.Insert(1, CreateButtons());

            VisualElement speakerTextContainer = new();
            speakerTextContainer.Add(new Label("Speaker Text"));
            TextField speakerTextInput = new()
            {
                value = data.Text
            };
            speakerTextInput.RegisterValueChangedCallback(evt =>
            {
                data.Text = evt.newValue;
            });

            speakerTextContainer.Add(speakerTextInput);
            extensionContainer.Add(speakerTextContainer);

            IntegerField speakerIdInput = new("Speaker ID");
            speakerIdInput.RegisterValueChangedCallback(evt =>
            {
                if (data.DialogueData.HasSpeaker(evt.newValue))
                    data.SpeakerId = evt.newValue;
                else
                    speakerIdInput.SetValueWithoutNotify(data.SpeakerId);
            });
            
            extensionContainer.Add(speakerIdInput);
            
            ObjectField audioInput = new()
            {
                objectType = typeof(AudioClip)
            };

            if (data.AudioAssetReference != null)
                audioInput.SetValueWithoutNotify(data.AudioAssetReference.Asset);

            audioInput.RegisterValueChangedCallback(evt =>
            {
                // TODO: 
                if (evt.newValue == null)
                    data.AudioAssetReference = null;
                else if (evt.newValue.IsAddressable())
                    data.AudioAssetReference = (evt.newValue as AudioClip).AsAddressable();
                else if (data.AudioAssetReference != null)
                    audioInput.SetValueWithoutNotify(data.AudioAssetReference.Asset);
                else
                    audioInput.SetValueWithoutNotify(null);
            });
            
            extensionContainer.Add(audioInput);

            extensionContainer.style.backgroundColor = new Color(0.2470588f, 0.2470588f, 0.2470588f, 0.8039216f);
            RefreshExpandedState();
        }

        protected virtual VisualElement CreateButtons()
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
            
            return c;
        }

        protected void AddPort(string text, Direction direction)
        {
            if (direction == Direction.Input)
                AddInputPort(text);
            else
                AddOutputPort(text);
        }

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
                    flexDirection = FlexDirection.RowReverse
                }
            };

            c.Add(InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null));
            c.Add(new TextField()
            {
                value = text
            });
            
            c.Add(new Button(() => RemoveButtonCallback(outputContainer.childCount - 1))
            {
                text = "x"
            });
            
            outputContainer.Add(c);
        }
        
        protected virtual void AddButtonCallback()
        {
            AddPort("New Choice", Direction.Output);
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