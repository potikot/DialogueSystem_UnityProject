using System.Threading.Tasks;
using PotikotTools.DialogueSystem;
using UnityEditor;
using UnityEngine;

public class TestDialogueEditorWindow : EditorWindow
{
    private string _dialogueId = "TestDialogueSave";
    // private TextAsset _textAsset;

    [MenuItem("Tools/DialogueSystem/Test")]
    public static void ShowWindow()
    {
        GetWindow<TestDialogueEditorWindow>("Test");
    }

    private void OnGUI()
    {
        GUILayout.Label("Test Dialogue Save/Load", EditorStyles.boldLabel);

        _dialogueId = EditorGUILayout.TextField("Dialogue ID", _dialogueId);
        // _textAsset = (TextAsset)EditorGUILayout.ObjectField("Text Asset", _textAsset, typeof(TextAsset), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Save Dialogue"))
        {
            Save();
        }

        if (GUILayout.Button("Load Dialogue"))
        {
            Load();
        }
    }

    private async void Save()
    {
        DialogueData dialogueData = GenerateDialogueData();
        
        if (await EditorDatabase.SaveDialogueAsync(dialogueData))
            Debug.Log($"Dialogue {_dialogueId} saved");
        else
            Debug.LogError($"Can not save dialogue - {_dialogueId}");
    }

    private async void Load()
    {
        DialogueData dialogueData = await Components.Database.GetDialogueAsync(_dialogueId);

        if (dialogueData != null)
            Debug.Log($"Dialogue {dialogueData.Id} loaded");
        else
            Debug.LogError($"Can not load dialogue - {_dialogueId}");
    }

    private DialogueData GenerateDialogueData()
    {
        // string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_textAsset));

        DialogueData dialogueData = new DialogueData(_dialogueId);
        for (int i = 0; i < 4; i++)
            dialogueData.AddNode<SingleChoiceNodeData>().Text = $"node {i}";

        return dialogueData;
    }
}