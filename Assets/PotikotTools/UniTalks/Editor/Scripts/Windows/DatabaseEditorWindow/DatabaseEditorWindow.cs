using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Editor
{
    public class DatabaseEditorWindow : EditorWindow
    {
        private const string NewTagTemplate = "tag-";
        private const float SpaceBetweenDialogueContainers = 10f;

        private VisualElement _dialoguesContainer;

        [MenuItem("Tools/PotikotTools/UniTalks Database", priority = 1)]
        public static void Open()
        {
            GetWindow<DatabaseEditorWindow>("Dialogue Database");
        }

        private async void CreateGUI()
        {
            var c = new VisualElement()
                .AddStyleSheets(
                    "Styles/DatabaseEditorWindow",
                    "Styles/Variables"
                ).AddUSSClasses("root-container");

            c.Add(CreateHeader());
            c.Add(await CreateBody());
            
            rootVisualElement.Add(c);
        }

        #region Header

        private VisualElement CreateHeader()
        {
            var c = new VisualElement()
                .AddUSSClasses("header");
            
            c.Add(CreateSearchBar());
            c.AddVerticalSpace(10f);
            c.Add(CreateControlsBar());
            c.AddVerticalSpace(10f);
            
            return c;
        }

        private VisualElement CreateSearchBar()
        {
            var c = new VisualElement()
                .AddUSSClasses("search-bar");
            
            var inputField = new TextField()
                .AddUSSClasses("search-bar__input-field")
                .AddPlaceholder("Dialogue name or t:tag");

            // inputField.RegisterCallback<KeyDownEvent>(evt =>
            // {
            //     if (evt.keyCode == KeyCode.Return)
            //         OnSearch();
            // });
            inputField.RegisterValueChangedCallback(_ => OnSearch());
            
            // var searchButton = new Button(OnSearch)
            //     .AddUSSClasses("search-bar__submit-button");
            //
            // searchButton.Add(new VisualElement()
            // {
            //     style =
            //     {
            //         backgroundImage = new StyleBackground(EditorGUIUtility.IconContent("d_Search Icon").image as Texture2D)
            //     }
            // }.AddUSSClasses("search-bar__submit-button-image"));
            
            c.Add(inputField);
            // c.AddHorizontalSpace(1f, Color.black);
            // c.Add(searchButton);
            
            return c;

            void OnSearch()
            {
                string text = inputField.text.Trim();
                
                if (string.IsNullOrEmpty(text))
                {
                    foreach (var e in _dialoguesContainer.Children())
                        e.style.display = DisplayStyle.Flex;
                    
                    return;
                }

                List<DialogueData> foundDialogues = text.StartsWith("t:")
                    ? SearchDialoguesUtility.SearchDialoguesByTag(text[2..])
                    : SearchDialoguesUtility.SearchDialoguesByName(text);

                if (foundDialogues == null)
                {
                    foreach (var e in _dialoguesContainer.Children())
                        e.style.display = DisplayStyle.Flex;
                    
                    return;
                }
                
                bool removeNextSpace = false;
                
                foreach (var e in _dialoguesContainer.Children())
                {
                    if (string.IsNullOrEmpty(e.viewDataKey))
                    {
                        e.style.display = removeNextSpace ? DisplayStyle.None : DisplayStyle.Flex;
                        continue;
                    }
                    
                    if (foundDialogues.Any(d => d.Name == e.viewDataKey))
                    {
                        e.style.display = DisplayStyle.Flex;
                        removeNextSpace = false;
                    }
                    else
                    {
                        e.style.display = DisplayStyle.None;
                        removeNextSpace = true;
                    }
                }
            }
        }

        private VisualElement CreateControlsBar() // TODO: naming
        {
            var c = new VisualElement()
                .AddUSSClasses("controls-bar");
            
            // create dialogue button

            var createDialogueButton = new Button()
            {
                text = "Create Dialogue"
            };
            createDialogueButton.clicked += () => CreateDialogueButtonCallback(createDialogueButton);
            createDialogueButton.AddUSSClasses("create-dialogue-button");
            
            c.Add(CreateDialogueViewOptionSelector());
            c.Add(createDialogueButton);
            
            return c;
        }

        // TODO: selector functionality
        private VisualElement CreateDialogueViewOptionSelector()
        {
            var c = new VisualElement()
                .AddUSSClasses("dialogue-view-options-selector");

            var buttons = new List<Button>(2);
            Action onClick = () =>
            {
                foreach (var button in buttons)
                    button.RemoveUSSClasses("dialogue-view-options-selector__button--selected");
            };

            // panel view button
            var panelViewOptionButton = CreateDialogueViewOptionButton(onClick, "dialogue-view-options-selector__panel-button");
            panelViewOptionButton.text = "Panel";

            // stroke view button
            var strokeViewOptionButton = CreateDialogueViewOptionButton(onClick, "dialogue-view-options-selector__stroke-button");
            strokeViewOptionButton.text = "Stroke";

            buttons.Add(panelViewOptionButton);
            buttons.Add(strokeViewOptionButton);

            c.Add(panelViewOptionButton);
            c.AddHorizontalSpace(1f, Color.black);
            c.Add(strokeViewOptionButton);

            return new VisualElement();
        }
        
        private Button CreateDialogueViewOptionButton(Action onClick, string classSelector)
        {
            var button = new Button();
            
            button.clicked += () =>
            {
                UniTalksAPI.Log("Clicked");
                onClick?.Invoke();
                DialogueViewOptionButtonCallback(button, 0);
            };
            
            return button.AddUSSClasses(
                "dialogue-view-options-selector__button",
                classSelector
            );
        }
        
        #endregion

        #region Body

        private async Task<VisualElement> CreateBody()
        {
            _dialoguesContainer = new ScrollView()
                .AddUSSClasses("dialogue-views-container");

            var dialogueDatas = await EditorDialogueComponents.Database.LoadAllDialoguesAsync();

            for (var i = 0; i < dialogueDatas.Count; i++)
            {
                if (i > 0)
                    _dialoguesContainer.AddVerticalSpace(SpaceBetweenDialogueContainers);

                _dialoguesContainer.Add(CreateDialoguePanel(dialogueDatas[i]));
            }
            
            return _dialoguesContainer;
        }

        private VisualElement CreateDialoguePanel(EditorDialogueData editorDialogueData)
        {
            var c = new VisualElement()
            {
                viewDataKey = editorDialogueData.Name
            };
            
            c.AddUSSClasses("dialogue-view");
            
            // name input
            
            var nameInputField = new TextField("Name")
            {
                value = editorDialogueData.Name,
                isDelayed = true
            };

            nameInputField.AddUSSClasses(
                "dialogue-view__text-input-field",
                "dialogue-view__name-input-field"
            ).AddPlaceholder("Enter name...");

            nameInputField.RegisterValueChangedCallback(OnNameValueChanged);
            nameInputField.RegisterCallback<FocusInEvent>(OnFocusIn);
            nameInputField.RegisterCallback<FocusOutEvent>(OnFocusOut);

            editorDialogueData.OnNameChanged += OnNameValueChangedOutside;
            
            // description input
            
            var descriptionInputField = new TextField("Description")
            {
                value = editorDialogueData.Description,
                // multiline = true
            };

            descriptionInputField.AddUSSClasses(
                "dialogue-view__text-input-field",
                "dialogue-view__description-input-field"
            ).AddPlaceholder("Enter description...");
            
            descriptionInputField.RegisterCallback<FocusInEvent>(OnFocusIn);
            descriptionInputField.RegisterCallback<FocusOutEvent>(OnFocusOut);
            
            // delete dialogue button
            
            var deleteButton = new Button(() => DeleteDialogueButtonCallback(OnDelete, editorDialogueData))
            {
                text = EditorSymbols.Cross
            };
            
            deleteButton.AddUSSClasses(
                "dialogue-view__button",
                "dialogue-view__delete-button"
            );
            
            // header
            
            var header = new VisualElement()
                .AddUSSClasses("dialogue-view__header");
            
            header.Add(nameInputField);
            header.Add(deleteButton);
            
            c.Add(header);
            c.Add(descriptionInputField);
            c.Add(CreateDialoguePanelFooter(editorDialogueData));
            
            return c;

            void OnNameValueChangedOutside(string value)
            {
                nameInputField.SetValueWithoutNotify(value);
            }
            
            async void OnNameValueChanged(ChangeEvent<string> evt)
            {
                nameInputField.RemoveUSSClasses("dialogue-view__text-input-field--focused");

                string newName = evt.newValue.Trim();
                UniTalksAPI.Log("Trying to rename to: " + newName);
                if (newName == editorDialogueData.Name)
                {
                    nameInputField.SetValueWithoutNotify(newName);
                    return;
                }

                if (!await editorDialogueData.TrySetName(newName))
                {
                    UniTalksAPI.LogError($"Failed to change name for dialogue '{editorDialogueData.Name}' with '{newName}'");
                }
                
                nameInputField.SetValueWithoutNotify(editorDialogueData.Name);
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

            void OnDelete()
            {
                nameInputField.UnregisterValueChangedCallback(OnNameValueChanged);
                nameInputField.UnregisterCallback<FocusInEvent>(OnFocusIn);
                nameInputField.UnregisterCallback<FocusOutEvent>(OnFocusOut);
                editorDialogueData.OnNameChanged -= OnNameValueChangedOutside;
                
                descriptionInputField.UnregisterCallback<FocusInEvent>(OnFocusIn);
                descriptionInputField.UnregisterCallback<FocusOutEvent>(OnFocusOut);

                int index = _dialoguesContainer.IndexOf(c);
                if (index == 0)
                {
                    if (_dialoguesContainer.childCount > 1)
                        _dialoguesContainer.RemoveAt(1);
                }
                else
                    _dialoguesContainer.RemoveAt(index - 1);
                
                c.RemoveFromHierarchy();
            }
        }

        private VisualElement CreateDialoguePanelFooter(EditorDialogueData editorDialogueData)
        {
            var c = new VisualElement()
                .AddUSSClasses("dialogue-view__footer");

            var buttonContainer = new VisualElement()
                .AddUSSClasses("dialogue-view__footer__button-container");

            // edit dialogue button

            var editDialogueButton = new Button(() => EditorDialogueComponents.DialogueEditorWM.Open(editorDialogueData))
            {
                text = "Edit"
            };

            editDialogueButton.AddUSSClasses(
                "dialogue-view__button",
                "dialogue-view__footer-button"
            );

            // test dialogue button
            
            var testDialogueButton = new Button(() => EditorDialogueComponents.DialoguePreviewWM.Open(editorDialogueData))
            {
                text = "Test"
            };

            testDialogueButton.AddUSSClasses(
                "dialogue-view__button",
                "dialogue-view__footer-button"
            );
            
            buttonContainer.Add(testDialogueButton);
            buttonContainer.AddHorizontalSpace(5f);
            buttonContainer.Add(editDialogueButton);
            
            c.Add(CreateTagsContainer(editorDialogueData));
            c.Add(buttonContainer);
            
            return c;
        }

        private VisualElement CreateTagsContainer(EditorDialogueData editorDialogueData)
        {
            // tags container
            
            var c = new VisualElement()
                .AddUSSClasses("dialogue-view__tags-container");
            
            var tags = editorDialogueData.RuntimeData.Tags;
            if (tags != null)
            {
                for (var i = 0; i < tags.Count; i++)
                {
                    c.Add(CreateTag(editorDialogueData, i));
                    c.AddHorizontalSpace(10f);
                }
            }

            c.Add(CreateAddTagButton(c, editorDialogueData));
            
            return c;
        }
        
        private VisualElement CreateTag(EditorDialogueData editorDialogueData, int tagIndex)
        {
            var tags = editorDialogueData.RuntimeData.Tags;
            string tag = tags[tagIndex];
            
            var c = new VisualElement()
                .AddUSSClasses("dialogue-view__tag");
            
            // input field
            
            var inputField = new TextField
            {
                value = tag,
                isDelayed = true
            };
            
            inputField.RegisterValueChangedCallback(OnTagChanged);
            
            // delete button

            var deleteButton = new Button(OnDelete)
            {
                text = EditorSymbols.Cross
            };

            deleteButton.AddUSSClasses(
                "dialogue-view__button",
                "dialogue-view__tag__delete-button"
            );
            
            c.Add(inputField);
            c.Add(deleteButton);
            
            return c;

            async void OnTagChanged(ChangeEvent<string> evt)
            {
                if (tags.Contains(evt.newValue))
                {
                    inputField.SetValueWithoutNotify(evt.previousValue);
                    return;
                }
                
                int changedTagIndex = tags.IndexOf(evt.previousValue);
                if (changedTagIndex == -1) // TODO:
                {
                    UniTalksAPI.LogError("Tag deleted or changed from another script");
                    return;
                }

                tags[changedTagIndex] = evt.newValue;
                await EditorDialogueComponents.Database.SaveDialogueAsync(editorDialogueData);
            }
            
            async void OnDelete()
            {
                tags.Remove(tag);
                
                int index = c.parent.IndexOf(c);
                if (index == 0)
                {
                    if (c.parent.childCount > 1)
                        c.parent.RemoveAt(1);
                }
                else
                    c.parent.RemoveAt(index - 1);
                
                c.RemoveFromHierarchy();
                
                await EditorDialogueComponents.Database.SaveDialogueAsync(editorDialogueData);
            }
        }

        private VisualElement CreateAddTagButton(VisualElement tagsContainer, EditorDialogueData editorDialogueData)
        {
            var button = new Button(async () => await AddTag(tagsContainer, editorDialogueData))
            {
                text = "+"
            };
            
            button.AddUSSClasses(
                "dialogue-view__button",
                "dialogue-view__tags-container__add-button"
            );

            return button;
        }
        
        private async Task AddTag(VisualElement tagsContainer, EditorDialogueData editorDialogueData)
        {
            var tags = editorDialogueData.RuntimeData.Tags;
            int tagViewIndex = tagsContainer.childCount - 1;

            int i = 1;
            while (tags.Contains(NewTagTemplate + i))
                i++;
            
            tags.Add(NewTagTemplate + i);
            var tagView = CreateTag(editorDialogueData, tags.Count - 1);

            tagsContainer.Insert(tagViewIndex, tagView);
            tagsContainer.InsertHorizontalSpace(tagViewIndex + 1, 10f);

            await EditorDialogueComponents.Database.SaveDialogueAsync(editorDialogueData);
            tagView.Q<TextField>().Focus();
        }
        
        #endregion
        
        // callbacks

        private void DialogueViewOptionButtonCallback(Button button, int option)
        {
            button.AddUSSClasses("dialogue-view-options-selector__button--selected");
        }
        
        private void CreateDialogueButtonCallback(Button button) // TODO:
        {
            Rect buttonWorldBound = button.worldBound;
            
            var c = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    position = Position.Absolute,
                    width = buttonWorldBound.width,
                    height = buttonWorldBound.height,
                    top = buttonWorldBound.y - buttonWorldBound.height,
                    left = buttonWorldBound.x,
                }
            };

            bool canceled = false;
            var dialogueNameInputField = new TextField()
            {
                value = "New Dialogue"
            };
            
            dialogueNameInputField.RegisterCallback<FocusOutEvent>(OnFocusOut);
            dialogueNameInputField.RegisterCallback<KeyDownEvent>(OnKeyDown);
            
            c.Add(dialogueNameInputField);
            
            rootVisualElement.Add(c);
            
            button.visible = false;
            dialogueNameInputField.Focus();

            void OnKeyDown(KeyDownEvent evt)
            {
                if (evt.keyCode == KeyCode.Escape)
                {
                    canceled = true;
                    OnFocusOut(null);
                }
            }

            async void OnFocusOut(FocusOutEvent evt)
            {
                if (!canceled)
                {
                    var editorDialogueData = await EditorDialogueComponents.Database.CreateDialogue(dialogueNameInputField.value);
                    if (editorDialogueData != null)
                    {
                        if (_dialoguesContainer.childCount > 0)
                            _dialoguesContainer.AddVerticalSpace(SpaceBetweenDialogueContainers);

                        _dialoguesContainer.Add(CreateDialoguePanel(editorDialogueData));
                    }
                }
                
                button.visible = true;
                dialogueNameInputField.UnregisterCallback<FocusOutEvent>(OnFocusOut);
                dialogueNameInputField.UnregisterCallback<KeyDownEvent>(OnKeyDown);
                
                c.RemoveFromHierarchy();
            }
        }
        
        private void DeleteDialogueButtonCallback(Action onDelete, EditorDialogueData editorDialogueData)
        {
            if (EditorUtility.DisplayDialog("Delete dialogue", $"Are you really want to delete dialogue: \"{editorDialogueData.Name}\"?", "Yes", "No"))
            {
                onDelete?.Invoke();
                EditorDialogueComponents.Database.DeleteDialogue(editorDialogueData);
            }
        }
    }
}