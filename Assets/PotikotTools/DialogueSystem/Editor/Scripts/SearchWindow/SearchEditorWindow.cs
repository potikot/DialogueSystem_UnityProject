using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PotikotTools.DialogueSystem.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace PotikotTools.DialogueSystem
{
    public class SearchEditorWindow : EditorWindow
    {
        private const string _crossSymbol = "\u2715";

        [MenuItem("Tools/DialogueSystem/Database")]
        public static void Open()
        {
            GetWindow<SearchEditorWindow>("Dialogue Database");
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
                .AddPlaceholder("Enter search text...");
            
            var searchButton = new Button(() => DL.Log("Search"))
            {
                text = "\\U0001F50D"
            };

            searchButton.AddUSSClasses("search-bar__submit-button");
            
            c.Add(inputField);
            c.AddHorizontalSpace(1f, Color.black);
            c.Add(searchButton);
            
            return c;
        }

        private VisualElement CreateControlsBar() // TODO: naming
        {
            var c = new VisualElement()
                .AddUSSClasses("controls-bar");
            
            // create dialogue button

            var createDialogueButton = new Button(CreateDialogueButtonCallback)
            {
                text = "Create Dialogue"
            };
            
            createDialogueButton.AddUSSClasses("create-dialogue-button");
            
            c.Add(CreateDialogueViewOptionSelector());
            c.Add(createDialogueButton);
            
            return c;
        }

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

            return c;
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
            var c = new ScrollView()
                .AddUSSClasses("dialogue-views-container");

            var dialogues = await EditorDatabase.LoadAllDialoguesAsync();

            foreach (var dialogue in dialogues)
            {
                c.Add(CreateDialoguePanel(dialogue));
                c.AddVerticalSpace(10f);
            }
            
            return c;
        }

        private VisualElement CreateDialoguePanel(EditorDialogueData editorDialogueData)
        {
            var c = new VisualElement()
                .AddUSSClasses("dialogue-view");

            // delete dialogue button
            
            var deleteButton = new Button(() => DeleteDialogueButtonCallback(editorDialogueData))
            {
                text = _crossSymbol
            };
            
            deleteButton.AddUSSClasses(
                "dialogue-view__button",
                "dialogue-view__delete-button"
            );
            
            // name input
            
            var nameInputField = new TextField("Name")
            {
                value = editorDialogueData.Id,
                isDelayed = true
            };
            
            nameInputField.AddUSSClasses(
                "dialogue-view__input-field",
                "dialogue-view__name-input-field"
            ).AddPlaceholder("Enter name...");

            nameInputField.RegisterValueChangedCallback(async evt =>
            {
                if (!await editorDialogueData.TrySetId(evt.newValue.Trim()))
                    nameInputField.SetValueWithoutNotify(editorDialogueData.Id);

                nameInputField.RemoveUSSClasses("dialogue-view__input-field--focused");
            });
            
            nameInputField.RegisterCallback<FocusInEvent>(evt =>
            {
                nameInputField.AddUSSClasses("dialogue-view__input-field--focused");
            });
            nameInputField.RegisterCallback<FocusOutEvent>(evt =>
            {
                nameInputField.RemoveUSSClasses("dialogue-view__input-field--focused");
            });
            
            // description input
            
            var descriptionInputField = new TextField("Description")
            {
                value = editorDialogueData.Description,
                // multiline = true
            };

            descriptionInputField.AddUSSClasses(
                "dialogue-view__input-field",
                "dialogue-view__description-input-field"
            ).AddPlaceholder("Enter description...");
            
            descriptionInputField.RegisterCallback<FocusInEvent>(evt =>
                descriptionInputField.AddUSSClasses("dialogue-view__input-field--focused"));
            descriptionInputField.RegisterCallback<FocusOutEvent>(evt =>
                descriptionInputField.RemoveUSSClasses("dialogue-view__input-field--focused"));
            
            var header = new VisualElement()
                .AddUSSClasses("dialogue-view__header");
            
            header.Add(nameInputField);
            header.Add(deleteButton);
            
            c.Add(header);
            c.Add(descriptionInputField);
            c.Add(CreateDialoguePanelFooter(editorDialogueData));
            
            return c;
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

            c.Add(CreateTagsContainer(editorDialogueData.RuntimeData.Tags));
            c.Add(editDialogueButton);
            
            return c;
        }

        private VisualElement CreateTagsContainer(string[] tags)
        {
            // tags container
            
            var c = new VisualElement()
                .AddUSSClasses("dialogue-view__tags-container");
            
            float spaceWidth = 10f;

            if (tags != null)
            {
                for (var i = 0; i < tags.Length; i++)
                {
                    c.Add(CreateTag(tags[i]));
                    c.AddHorizontalSpace(spaceWidth);
                }
            }

            c.Add(CreateAddTagButton(c));
            
            return c;
        }
        
        private VisualElement CreateTag(string text)
        {
            var c = new VisualElement();
            c.AddToClassList("dialogue-view__tag");
            
            // input field
            
            var inputField = new TextField()
            {
                value = text
            };
            
            // delete button

            var deleteButon = new Button(() => DL.Log($"Delete Tag: {text}"))
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

        private VisualElement CreateAddTagButton(VisualElement tagsContainer)
        {
            var button = new Button(() => AddTag(tagsContainer))
            {
                text = "+"
            };
            
            button.AddUSSClasses(
                "dialogue-view__button",
                "dialogue-view__tags-container__add-button"
            );
            
            return button;
        }
        
        private void AddTag(VisualElement tagsContainer)
        {
            int index = tagsContainer.childCount - 1;
            
            var tag = CreateTag("New Tag");
            tagsContainer.Insert(index, tag);
            
            tagsContainer.Insert(index + 1, new VisualElement()
            {
                style =
                {
                    width = 10f
                }
            });

            tag.Q<TextField>().Focus();
        }
        
        #endregion
        
        // callbacks

        private void DialogueViewOptionButtonCallback(Button button, int option)
        {
            button.AddUSSClasses("dialogue-view-options-selector__button--selected");
        }
        
        private void CreateDialogueButtonCallback()
        {
            EditorDatabase.CreateDialogue("New Dialogue");
        }
        
        private void DeleteDialogueButtonCallback(EditorDialogueData editorDialogueData)
        {
            if (EditorUtility.DisplayDialog($"Delete dialogue", $"Are you really want to delete dialogue: \"{editorDialogueData.Id}\"?", "Yes", "No"))
                EditorDatabase.DeleteDialogue(editorDialogueData.Id);
        }
    }
}