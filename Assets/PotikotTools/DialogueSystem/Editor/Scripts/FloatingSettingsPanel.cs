using System.Collections.Generic;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem.Editor
{
    public class FloatingSettingsPanel : VisualElement
    {
        protected EditorDialogueData editorData;
        
        public FloatingSettingsPanel(EditorDialogueData editorDialogueData)
        {
            editorData = editorDialogueData;

            AddSettings();
            
            this.AddManipulator(new DragManipulator());
            
            this.AddStyleSheets("Styles/FloatingSettings");
            this.AddUSSClasses("panel");
        }

        private void AddSettings()
        {
            // AddNameInputField();
            AddSpeakersList();
        }

        private void AddNameInputField()
        {
            var input = new TextField("Name")
            {
                value = editorData.Id,
                isDelayed = true
            };

            input.AddPlaceholder("Enter name...");

            input.RegisterValueChangedCallback(OnNameValueChanged);
            // nameInputField.RegisterCallback<FocusInEvent>(OnFocusIn);
            // nameInputField.RegisterCallback<FocusOutEvent>(OnFocusOut);
            input.RegisterCallback<DetachFromPanelEvent>(_ =>
            {
                input.UnregisterValueChangedCallback(OnNameValueChanged);
            });
            
            Add(input);

            editorData.OnNameChanged += OnNameChanged;

            void OnNameChanged(string newName)
            {
                
            }
                
            async void OnNameValueChanged(ChangeEvent<string> evt)
            {
                // nameInputField.RemoveUSSClasses("dialogue-view__text-input-field--focused");

                string newName = evt.newValue.Trim();
                DL.Log("Trying to rename to: " + newName);
                if (newName == editorData.Id)
                {
                    input.SetValueWithoutNotify(newName);
                    return;
                }

                if (!await editorData.TrySetName(newName))
                {
                    DL.LogError($"Failed to change name for dialogue '{editorData.Id}' with '{newName}'");
                }
                
                input.SetValueWithoutNotify(editorData.Id);
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

        private void AddSpeakersList()
        {
            var speakers = new List<SpeakerData>(editorData.RuntimeData.Speakers);
            Add(CreateListView("Speakers", speakers));
        }
        
        private ListView CreateListView(string headerTitle, List<SpeakerData> source)
        {
            ListView listView = new(source)
            {
                headerTitle = headerTitle,
                showFoldoutHeader = true,
                showBorder = true,
                showAddRemoveFooter = true,
                // reorderable = true,
                // reorderMode = ListViewReorderMode.Animated,
                makeItem = MakeListItem,
                bindItem = BindListItem
            };

            listView.itemsAdded += async _ => await EditorComponents.Database.SaveDialogueAsync(editorData);
            listView.itemsRemoved += async _ => await EditorComponents.Database.SaveDialogueAsync(editorData);

            return listView;
        }
        
        private VisualElement MakeListItem()
        {
            TextField textField = new() { isDelayed = true };
            return textField;
        }

        private void BindListItem(VisualElement element, int index)
        {
            if (!editorData.RuntimeData.TryGetSpeaker(index, out var speaker))
            {
                speaker = new SpeakerData("New Speaker");
                editorData.RuntimeData.Speakers.Add(speaker);
            }
            
            TextField textField = element.Q<TextField>();
            textField.label = $"Element {index}";
            textField.value = speaker.Name; // TODO: index out of range on reset empty list
            
            textField.RegisterValueChangedCallback(OnValueChanged);
            textField.RegisterCallback<DetachFromPanelEvent>(_ => textField.UnregisterValueChangedCallback(OnValueChanged));

            async void OnValueChanged(ChangeEvent<string> evt)
            {
                editorData.RuntimeData.Speakers[index].Name = evt.newValue;
                await EditorComponents.Database.SaveDialogueAsync(editorData);
            }
        }
    }
}