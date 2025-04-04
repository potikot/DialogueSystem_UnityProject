using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using PotikotTools.DialogueSystem;
using UnityEditor;
using UnityEngine;

public class TestDialogueSave : MonoBehaviour
{
    [SerializeField] private string _dialogueId = "TestDialogueSave";
    [SerializeField] private TextAsset _textAsset;

    [ContextMenu("Save")]
    private void Save()
    {
        DialogueData dialogueData = GenerateDialogueData();
        Components.Saver.Save(dialogueData);
        
        DL.Log($"{_dialogueId} saved");
    }

    [ContextMenu("Load")]
    private void Load()
    {
        DialogueData dialogueData = Components.Saver.Load(_dialogueId);
        DL.Log($"{dialogueData.Id} loaded");
    }

    private DialogueData GenerateDialogueData()
    {
        string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_textAsset));

        List<NodeData> nodes = new(4);
        for (int i = 0; i < nodes.Capacity; i++)
        {
            nodes.Add(new NodeData(i)
            {
                Text = $"node {i}",
            });
        }
        
        return new DialogueData(_dialogueId)
        {
        };;
    }
}