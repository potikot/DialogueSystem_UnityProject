using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem.Editor
{
    public class SettingsPanel : Foldout
    {
        protected EditorDialogueData editorData;
        
        public SettingsPanel(EditorDialogueData editorDialogueData)
        {
            editorData = editorDialogueData;

            text = "Settings";
            SetValueWithoutNotify(editorData.SettingsPanelOpened);
            
            Draw();
            AddManipulators();
            
            this.AddStyleSheets("Styles/FloatingSettings");
            this.AddUSSClasses("panel");

            RegisterCallback<ChangeEvent<bool>>(OnValueChanged);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        public virtual void SetPosition(Vector2 desiredPosition)
        {
            style.left = desiredPosition.x;
            style.top = desiredPosition.y;
        }

        private void AddManipulators()
        {
        }
        
        protected virtual void Draw()
        {
            AddNameInputField();
            AddSpeakersList();
            
            SetPosition(editorData.SettingsPanelPosition);
        }

        protected virtual void OnValueChanged(ChangeEvent<bool> evt)
        {
            editorData.SettingsPanelOpened = evt.newValue;
        }
        
        protected virtual void OnGeometryChanged(GeometryChangedEvent evt)
        {
            editorData.SettingsPanelPosition = evt.newRect.position;
        }

        protected virtual void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            DL.Log($"DetachFromPanel: {editorData.Name}");
            UnregisterCallback<ChangeEvent<bool>>(OnValueChanged);
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
        
        protected virtual void AddNameInputField()
        {
            var input = new TextField("Name")
            {
                value = editorData.Name,
                isDelayed = true
            };

            input.AddPlaceholder("Enter name...");

            input.RegisterValueChangedCallback(OnNameValueChanged);
            // nameInputField.RegisterCallback<FocusInEvent>(OnFocusIn);
            // nameInputField.RegisterCallback<FocusOutEvent>(OnFocusOut);
            input.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                DL.Log("attaching to panel");
                input.SetValueWithoutNotify(editorData.Name);
            });
            input.RegisterCallback<DetachFromPanelEvent>(_ =>
            {
                input.UnregisterValueChangedCallback(OnNameValueChanged);
                editorData.OnNameChanged -= OnNameChanged;
            });
            
            Add(input);

            editorData.OnNameChanged += OnNameChanged;

            void OnNameChanged(string newName)
            {
                input.SetValueWithoutNotify(newName);
            }

            async void OnNameValueChanged(ChangeEvent<string> evt)
            {
                // nameInputField.RemoveUSSClasses("dialogue-view__text-input-field--focused");

                string newName = evt.newValue.Trim();
                DL.Log("Trying to rename to: " + newName);
                if (newName == editorData.Name)
                {
                    input.SetValueWithoutNotify(newName);
                    return;
                }

                if (!await editorData.TrySetName(newName))
                {
                    DL.LogError($"Failed to change name for dialogue '{editorData.Name}' with '{newName}'");
                }
                
                input.SetValueWithoutNotify(editorData.Name);
            }
            
            void OnFocusIn(FocusInEvent evt)
            {
                if (evt.target is VisualElement targetElement)
                {
                    targetElement.AddUSSClasses("dialogue-view__text-input-field--focused");
                }
            }

            void OnFocusOut(FocusOutEvent evt)
            {
                if (evt.target is VisualElement targetElement)
                {
                    targetElement.RemoveUSSClasses("dialogue-view__text-input-field--focused");
                }
            }
        }

        protected virtual void AddSpeakersList()
        {
            var speakers = new List<SpeakerData>(editorData.RuntimeData.Speakers);
            Add(CreateListView("Speakers", speakers));
        }
        
        protected virtual ListView CreateListView(string headerTitle, List<SpeakerData> source)
        {
            ListView listView = new(source)
            {
                headerTitle = headerTitle,
                showFoldoutHeader = true,
                showBorder = true,
                showAddRemoveFooter = true,
                reorderable = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                makeItem = MakeListItem,
                bindItem = BindListItem,
                unbindItem = UnbindItem
            };

            listView.itemsAdded += _ => EditorComponents.Database.SaveDialogue(editorData);
            listView.itemsRemoved += indices =>
            {
                foreach (int index in indices)
                    editorData.RuntimeData.Speakers.RemoveAt(index);
                
                EditorComponents.Database.SaveDialogue(editorData);
            };

            return listView;
        }

        private void UnbindItem(VisualElement element, int index)
        {
            element.Q<TextField>().UnregisterValueChangedCallback(OnValueChanged);
        }

        protected virtual VisualElement MakeListItem()
        {
            TextField textField = new() { isDelayed = true};
            textField.AddUSSClasses("list-item");
            textField.Children().Last().AddUSSClasses("list-item__input");
            return textField;
        }

        protected virtual void BindListItem(VisualElement element, int index)
        {
            if (!editorData.RuntimeData.TryGetSpeaker(index, out var speaker))
            {
                speaker = new SpeakerData("New Speaker");
                editorData.RuntimeData.Speakers.Add(speaker);
            }
            
            TextField textField = element.Q<TextField>();
            textField.label = $"Element {index}";
            textField.userData = speaker;
            textField.SetValueWithoutNotify(speaker.Name);
            
            textField.RegisterValueChangedCallback(OnValueChanged);
            textField.RegisterCallback<DetachFromPanelEvent>(_ => textField.UnregisterValueChangedCallback(OnValueChanged));
        }
        
        void OnValueChanged(ChangeEvent<string> evt)
        {
            if (evt.target is not TextField { userData: SpeakerData speakerData })
            {
                DL.LogError("Speaker not found");
                return;
            }
            
            speakerData.Name = evt.newValue;
            EditorComponents.Database.SaveDialogue(editorData);
        }
    }
}