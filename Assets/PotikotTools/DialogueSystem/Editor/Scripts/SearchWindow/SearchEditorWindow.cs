using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PotikotTools.DialogueSystem.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem
{
    public class SearchEditorWindow : EditorWindow
    {
        [MenuItem("Tools/DialogueSystem/Search Window")]
        public static void Open()
        {
            GetWindow<SearchEditorWindow>("Dialogue Database");
        }
        
        private void CreateGUI()
        {
            var c = new VisualElement();
            c.Margin(5f, 10f);

            c.Add(CreateHeader());
            c.Add(CreateBody());
            
            rootVisualElement.Add(c);
        }

        #region Header

        private VisualElement CreateHeader()
        {
            var c = new VisualElement();
            
            c.Add(CreateSearchBar());
            c.AddVerticalSpace(10f);
            c.Add(CreateControlsBar());
            c.AddVerticalSpace(10f);
            
            return c;
        }
        
        private VisualElement CreateSearchBar()
        {
            var c = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };

            c.BorderRadius(5f);
            c.BorderWidth(1f);
            c.BorderColor(Color.black);

            var inputField = new TextField
            {
                style =
                {
                    flexGrow = 1f,
                    borderLeftColor = Color.clear
                }
            };
            
            inputField.AddPlaceholder("Enter search text...");
            inputField.Margin(0f);
            inputField.BorderWidth(0f);
            inputField.BorderRadius(5f, 0f);
            
            var textInput = inputField.Children().First();
            textInput.BorderRadius(5f, 0f);
            textInput.BorderColor(Color.clear);
            
            var searchButton = new Button(() => DL.Log("Search"))
            {
                text = "\\U0001F50D"
            };
            
            searchButton.Margin(0f);
            searchButton.BorderWidth(0f);
            searchButton.BorderRadius(0f, 5f);
            
            c.Add(inputField);
            c.AddHorizontalSpace(1f, Color.black);
            c.Add(searchButton);
            
            return c;
        }

        private VisualElement CreateControlsBar() // TODO: naming
        {
            var c = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween
                }
            };

            // dialogue panel view options
            
            var dialoguePanelViewOptionsContainer = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };

            dialoguePanelViewOptionsContainer.BorderRadius(5f);
            dialoguePanelViewOptionsContainer.BorderWidth(1f);
            dialoguePanelViewOptionsContainer.BorderColor(Color.black);
            
            // panel view button
            var panelViewOptionButton = new Button(() => DL.Log("Panel"))
            {
                text = "Panel",
                style =
                {
                    borderTopRightRadius = 0f,
                    borderBottomRightRadius = 0f,
                }
            };
            
            panelViewOptionButton.BorderWidth(0f);
            panelViewOptionButton.Margin(0f);

            // stroke view button
            var strokeViewOptionButton = new Button(() => DL.Log("Stroke"))
            {
                text = "Stroke",
                style =
                {
                    borderTopLeftRadius = 0f,
                    borderBottomLeftRadius = 0f,
                }
            };
            
            strokeViewOptionButton.BorderWidth(0f);
            strokeViewOptionButton.Margin(0f);

            dialoguePanelViewOptionsContainer.Add(panelViewOptionButton);
            dialoguePanelViewOptionsContainer.AddHorizontalSpace(1f, Color.black);
            dialoguePanelViewOptionsContainer.Add(strokeViewOptionButton);
            
            // create dialogue button

            var createDialogueButton = new Button(() => DL.Log("Create Dialogue"))
            {
                text = "Create Dialogue"
            };
            
            createDialogueButton.BorderRadius(5f);
            createDialogueButton.BorderWidth(1f);
            createDialogueButton.BorderColor(Color.black);
            createDialogueButton.Margin(0f);
            
            c.Add(dialoguePanelViewOptionsContainer);
            c.Add(createDialogueButton);
            return c;
        }

        #endregion

        #region Body

        private VisualElement CreateBody()
        {
            var c = new VisualElement();

            c.Add(CreateDialoguePanel(new EditorDialogueData(new DialogueData("Test"))));
            
            return c;
        }

        private VisualElement CreateDialoguePanel(EditorDialogueData editorData)
        {
            var c = new VisualElement();
            
            c.BorderRadius(5f);
            c.BorderWidth(1f);
            c.BorderColor(Color.black);

            // delete dialogue button
            
            var deleteButton = new Button(() => DL.Log($"Delete Dialogue: {editorData.RuntimeData.Id}"))
            {
                text = "x",
            };
            
            deleteButton.style.alignSelf = Align.FlexEnd;

            // name input
            
            var nameInputField = new TextField()
            {
                value = editorData.RuntimeData.Id
            };
            
            nameInputField.AddPlaceholder("Enter name...");
            
            // description input
            
            var descriptionInputField = new TextField()
            {
                value = "",
                multiline = true
            };
            
            descriptionInputField.AddPlaceholder("Enter description...");
            
            c.Add(deleteButton);
            c.Add(nameInputField);
            c.Add(descriptionInputField);
            c.Add(CreateDialoguePanelFooter());
            
            return c;
        }

        private VisualElement CreateDialoguePanelFooter()
        {
            var c = new VisualElement()
            {
                style =
                {
                    height = 100f,
                }
            };

            var tagsContainer = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                }
            };
            
            tagsContainer.Add(CreateTag("Tag 1"));
            tagsContainer.AddHorizontalSpace(10f);
            tagsContainer.Add(CreateTag("Tag 2"));
            tagsContainer.AddHorizontalSpace(10f);
            tagsContainer.Add(CreateTag("Tag 3"));

            c.Add(tagsContainer);
            
            return c;
        }
        
        private VisualElement CreateTag(string text)
        {
            var c = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };
            
            c.BorderRadius(5f);
            c.BorderWidth(1f);
            c.BorderColor(Color.black);

            // input field
            
            var inputField = new TextField()
            {
                value = text
            };
            
            inputField.Margin(0f);
            inputField.BorderWidth(0f);
            inputField.BorderRadius(5f, 0f);
            
            var textInput = inputField.Children().First();
            textInput.BorderRadius(5f, 0f);
            textInput.BorderColor(Color.clear);
            textInput.style.backgroundColor = Color.clear;
            
            // delete button

            var deleteButon = new Button(() => DL.Log($"Delete Tag: {text}"))
            {
                text = "x",
                style =
                {
                    display = DisplayStyle.None,
                    backgroundColor = Color.clear
                }
            };
            
            deleteButon.BorderColor(Color.clear);
            
            c.Add(inputField);
            c.Add(deleteButon);
            
            c.RegisterCallback<PointerEnterEvent>(evt => deleteButon.style.display = DisplayStyle.Flex);
            c.RegisterCallback<PointerLeaveEvent>(evt => deleteButon.style.display = DisplayStyle.None);
            
            return c;
        }
        
        #endregion
    }
}