using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Editor
{
    public class DialogueSettingsPanel : Foldout
    {
        protected EditorDialogueData editorData;
        
        public DialogueSettingsPanel(EditorDialogueData editorDialogueData)
        {
            editorData = editorDialogueData;

            text = "Settings";
            SetValueWithoutNotify(editorData.SettingsPanelOpened);
            
            Draw();
            AddManipulators();
            
            this.AddStyleSheets("Styles/FloatingSettings");
            this.AddUSSClasses("panel");

            // RegisterCallback<ChangeEvent<bool>>(OnValueChanged);
            // RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        public virtual void SetPosition(Vector2 desiredPosition)
        {
            style.left = desiredPosition.x;
            style.top = desiredPosition.y;
        }

        private void AddManipulators()
        {
            this.AddManipulator(new DragManipulator());
        }
        
        protected virtual void Draw()
        {
            AddNameInputField();
            AddSpeakersList();
            
            SetPosition(editorData.SettingsPanelPosition);
        }

        protected virtual void OnFoldoutValueChanged(ChangeEvent<bool> evt)
        {
            editorData.SettingsPanelOpened = evt.newValue;
        }
        
        protected virtual void OnGeometryChanged(GeometryChangedEvent evt)
        {
            editorData.SettingsPanelPosition = evt.newRect.position;
        }

        protected virtual void OnAttachToPanel(AttachToPanelEvent evt)
        {
            // DL.Log($"AttachToPanel: {editorData.Name}");
            RegisterCallback<ChangeEvent<bool>>(OnFoldoutValueChanged);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
        
        protected virtual void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            // DL.Log($"DetachFromPanel: {editorData.Name}");
            UnregisterCallback<ChangeEvent<bool>>(OnFoldoutValueChanged);
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

            // input.RegisterValueChangedCallback(OnNameValueChanged);
            // nameInputField.RegisterCallback<FocusInEvent>(OnFocusIn);
            // nameInputField.RegisterCallback<FocusOutEvent>(OnFocusOut);
            input.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                // DL.Log("Attach name input");
                // input.SetValueWithoutNotify(editorData.Name);
                input.RegisterValueChangedCallback(OnNameValueChanged);
            });
            input.RegisterCallback<DetachFromPanelEvent>(_ =>
            {
                // DL.Log("Detach name input");
                editorData.OnNameChanged -= OnNameChanged;
                input.UnregisterValueChangedCallback(OnNameValueChanged);
            });
            
            Add(input);

            editorData.OnNameChanged += OnNameChanged;
            return;

            void OnNameChanged(string newName)
            {
                input.SetValueWithoutNotify(newName);
            }

            async void OnNameValueChanged(ChangeEvent<string> evt)
            {
                // nameInputField.RemoveUSSClasses("dialogue-view__text-input-field--focused");

                // DL.Log("Trying to rename to: " + evt.newValue);
                string newName = evt.newValue.Trim();
                if (newName == editorData.Name)
                {
                    input.SetValueWithoutNotify(newName);
                    return;
                }

                if (!await editorData.TrySetName(newName))
                {
                    UniTalksAPI.LogError($"Failed to change name for dialogue '{editorData.Name}' with '{newName}'");
                }
                
                input.SetValueWithoutNotify(editorData.Name);
            }
            
            // void OnFocusIn(FocusInEvent evt)
            // {
            //     if (evt.target is VisualElement targetElement)
            //     {
            //         targetElement.AddUSSClasses("dialogue-view__text-input-field--focused");
            //     }
            // }
            //
            // void OnFocusOut(FocusOutEvent evt)
            // {
            //     if (evt.target is VisualElement targetElement)
            //     {
            //         targetElement.RemoveUSSClasses("dialogue-view__text-input-field--focused");
            //     }
            // }
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

            listView.itemsAdded += _ => EditorDialogueComponents.Database.SaveDialogue(editorData);
            listView.itemsRemoved += indices =>
            {
                foreach (int index in indices)
                    editorData.RuntimeData.Speakers.RemoveAt(index);
                
                EditorDialogueComponents.Database.SaveDialogue(editorData);
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
            textField.RegisterCallback<AttachToPanelEvent>(_ => textField.RegisterValueChangedCallback(OnValueChanged));
            textField.RegisterCallback<DetachFromPanelEvent>(_ => textField.UnregisterValueChangedCallback(OnValueChanged));
        }
        
        void OnValueChanged(ChangeEvent<string> evt)
        {
            if (evt.target is not TextField { userData: SpeakerData speakerData })
            {
                UniTalksAPI.LogError("Speaker not found");
                return;
            }
            
            speakerData.Name = evt.newValue;
            EditorDialogueComponents.Database.SaveDialogue(editorData);
        }
    }
}