using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem.Editor
{
    public class DatabaseEditorWindow : EditorWindow
    {
        private const string _crossSymbol = "\u2715";

        private VisualElement _dialoguesContainer;
        private float _spaceBetweenDialogueContainers = 10f;

        [MenuItem("Tools/DialogueSystem/Database")]
        public static void Open()
        {
            GetWindow<DatabaseEditorWindow>("Dialogue Database");
        }

        private async void CreateGUI()
        {
            var c = new VisualElement()
                .AddStyleSheets(
                    "Styles/SearchEditorWindow",
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

            inputField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return)
                    OnSearch();
            });
            
            var searchButton = new Button(OnSearch)
            {
                text = "\\U0001F50D"
            };

            searchButton.AddUSSClasses("search-bar__submit-button");
            
            c.Add(inputField);
            c.AddHorizontalSpace(1f, Color.black);
            c.Add(searchButton);
            
            return c;

            void OnSearch()
            {
                if (string.IsNullOrEmpty(inputField.text))
                {
                    foreach (var e in _dialoguesContainer.Children())
                        e.style.display = DisplayStyle.Flex;
                    
                    return;
                }

                List<DialogueData> foundDialogues;
                
                if (inputField.text.StartsWith("t:"))
                    foundDialogues = SearchDialoguesUtility.SearchDialoguesByTag(inputField.text[2..]);
                else
                    foundDialogues = SearchDialoguesUtility.SearchDialoguesByName(inputField.text);
                    
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
                    
                    if (foundDialogues.Any(d => d.Id == e.viewDataKey))
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
                DL.Log("Clicked");
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

            var dialogueDatas = await EditorDatabase.LoadAllDialoguesAsync();

            for (var i = 0; i < dialogueDatas.Count; i++)
            {
                if (i > 0)
                    _dialoguesContainer.AddVerticalSpace(_spaceBetweenDialogueContainers);

                _dialoguesContainer.Add(CreateDialoguePanel(dialogueDatas[i]));
            }
            
            return _dialoguesContainer;
        }

        private VisualElement CreateDialoguePanel(EditorDialogueData editorDialogueData)
        {
            var c = new VisualElement()
            {
                viewDataKey = editorDialogueData.Id
            };
            
            c.AddUSSClasses("dialogue-view");
            
            // name input
            
            var nameInputField = new TextField("Name")
            {
                value = editorDialogueData.Id,
                isDelayed = true
            };

            nameInputField.AddUSSClasses(
                "dialogue-view__text-input-field",
                "dialogue-view__name-input-field"
            ).AddPlaceholder("Enter name...");

            nameInputField.RegisterValueChangedCallback(OnNameValueChanged);
            nameInputField.RegisterCallback<FocusInEvent>(OnFocusIn);
            nameInputField.RegisterCallback<FocusOutEvent>(OnFocusOut);
            
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
                text = _crossSymbol
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

            async void OnNameValueChanged(ChangeEvent<string> evt)
            {
                if (!await editorDialogueData.TrySetId(evt.newValue.Trim()))
                    nameInputField.SetValueWithoutNotify(editorDialogueData.Id);

                nameInputField.RemoveUSSClasses("dialogue-view__text-input-field--focused");
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
                
                descriptionInputField.UnregisterCallback<FocusInEvent>(OnFocusIn);
                descriptionInputField.UnregisterCallback<FocusOutEvent>(OnFocusOut);
                
                c.RemoveFromHierarchy();
            }
        }

        private VisualElement CreateDialoguePanelFooter(EditorDialogueData editorDialogueData)
        {
            var c = new VisualElement()
                .AddUSSClasses("dialogue-view__footer");
            
            // edit dialogue button

            var editDialogueButton = new Button(() => NodeEditorWindow.Open(editorDialogueData))
            {
                text = "Open in node editor"
            };

            editDialogueButton.AddUSSClasses(
                "dialogue-view__button",
                "dialogue-view__edit-dialogue-button"
            );

            c.Add(CreateTagsContainer(editorDialogueData));
            c.Add(editDialogueButton);
            
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
            
            var c = new VisualElement();
            c.AddToClassList("dialogue-view__tag");
            
            // input field
            
            var inputField = new TextField()
            {
                value = tag,
                isDelayed = true
            };
            
            inputField.RegisterValueChangedCallback(async evt =>
            {
                if (tags.Contains(evt.newValue))
                {
                    inputField.SetValueWithoutNotify(evt.previousValue);
                    return;
                }
                
                int changedTagIndex = tags.IndexOf(evt.previousValue);
                if (changedTagIndex == -1) // TODO:
                {
                    DL.LogError("Tag deleted or changed from another script");
                    return;
                }
                
                tags[changedTagIndex] = evt.newValue;
                await EditorDatabase.SaveDialogueAsync(editorDialogueData);
            });
            
            // delete button

            var deleteButon = new Button(async () =>
            {
                tags.Remove(tag);
                c.RemoveFromHierarchy();
                
                await EditorDatabase.SaveDialogueAsync(editorDialogueData);
            })
            {
                text = _crossSymbol
            };

            deleteButon.AddUSSClasses(
                "dialogue-view__button",
                "dialogue-view__tag__delete-button"
            );
            
            c.Add(inputField);
            c.Add(deleteButon);
            
            return c;
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

            string newTagTemplate = "tag-";
            int i = 1;
            while (tags.Contains(newTagTemplate + i.ToString()))
                i++;
            
            tags.Add(newTagTemplate + i.ToString());
            var tagView = CreateTag(editorDialogueData, tags.Count - 1);

            tagsContainer.Insert(tagViewIndex, tagView);
            tagsContainer.InsertHorizontalSpace(tagViewIndex + 1, 10f);

            await EditorDatabase.SaveDialogueAsync(editorDialogueData);
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
            var dialogueNameInputField = new TextField()
            {
                value = "New Dialogue",
                style =
                {
                    position = Position.Absolute,
                    width = buttonWorldBound.width,
                    height = buttonWorldBound.height,
                    top = buttonWorldBound.y - buttonWorldBound.height,
                    left = buttonWorldBound.x,
                }
            };
            
            dialogueNameInputField.RegisterCallback<FocusOutEvent>(FocusOutEventCallback);
            
            rootVisualElement.Add(dialogueNameInputField);
            
            button.visible = false;
            dialogueNameInputField.Focus();
            
            void FocusOutEventCallback(FocusOutEvent evt)
            {
                if (EditorDatabase.TryCreateDialogue(dialogueNameInputField.value, out EditorDialogueData editorDialogueData))
                {
                    if (_dialoguesContainer.childCount > 0)
                        _dialoguesContainer.AddVerticalSpace(_spaceBetweenDialogueContainers);
                    
                    _dialoguesContainer.Add(CreateDialoguePanel(editorDialogueData));
                }
                
                button.visible = true;
                dialogueNameInputField.RemoveFromHierarchy();
                dialogueNameInputField.UnregisterCallback<FocusOutEvent>(FocusOutEventCallback);
            }
        }
        
        private void DeleteDialogueButtonCallback(Action onDelete, EditorDialogueData editorDialogueData)
        {
            if (EditorUtility.DisplayDialog("Delete dialogue", $"Are you really want to delete dialogue: \"{editorDialogueData.Id}\"?", "Yes", "No"))
            {
                onDelete?.Invoke();
                EditorDatabase.DeleteDialogue(editorDialogueData.Id);
            }
        }
    }
}