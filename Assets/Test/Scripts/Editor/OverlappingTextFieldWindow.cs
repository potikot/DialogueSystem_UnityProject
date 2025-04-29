using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class OverlappingTextFieldWindow : EditorWindow
{
    [MenuItem("Window/Overlapping TextField Example")]
    public static void ShowWindow()
    {
        GetWindow<OverlappingTextFieldWindow>("TextField Over Button");
    }

    private void OnEnable()
    {
        // Create and configure the button
        Button mainButton = new Button();
        mainButton.text = "Click to Add TextField";
        mainButton.style.marginTop = 20;
        mainButton.style.width = 200;
        mainButton.style.height = 30;
        
        // Add click handler
        mainButton.clicked += () => CreateOverlappingTextField(mainButton);
        
        // Add button to root
        rootVisualElement.Add(mainButton);
    }

    private void CreateOverlappingTextField(Button targetButton)
    {
        // Remove any existing text fields first
        RemoveExistingTextField();

        // Create new TextField with absolute positioning
        TextField textField = new TextField();
        textField.value = "Type here...";
        
        // Position directly above the button
        Rect buttonRect = targetButton.worldBound;
        textField.style.position = Position.Absolute;
        textField.style.top = buttonRect.y; // 30 pixels above button
        textField.style.left = buttonRect.x;
        textField.style.width = buttonRect.width;

        // Auto-select all text when created
        textField.Q<TextElement>().RegisterCallback<MouseDownEvent>(evt => {
            textField.SelectAll();
        });

        // Handle focus loss and Enter key
        textField.RegisterCallback<FocusOutEvent>(evt => RemoveTextField(textField));
        // textField.RegisterCallback<SubmitEvent>(evt => RemoveTextField(textField));

        // Add to root visual element
        rootVisualElement.Add(textField);
        
        // Set focus immediately
        textField.Focus();
    }

    private void RemoveExistingTextField()
    {
        // Remove any existing text fields
        var existingFields = rootVisualElement.Query<TextField>().ToList();
        foreach (var field in existingFields)
        {
            rootVisualElement.Remove(field);
        }
    }

    private void RemoveTextField(TextField field)
    {
        if (rootVisualElement.Contains(field))
        {
            rootVisualElement.Remove(field);
            
            // Optional: Do something with the input value
            if (!string.IsNullOrEmpty(field.value))
            {
                Debug.Log($"User entered: {field.value}");
            }
        }
    }
}