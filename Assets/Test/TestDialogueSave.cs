using System.Collections.Generic;
using PotikotTools.DialogueSystem;
using UnityEngine;

public class TestDialogueSave : MonoBehaviour
{
    [SerializeField] private string _dialogueId = "TestDialogueSave";
    
    [ContextMenu("Save")]
    private void Save()
    {
        DialogueData dialogueData = GenerateDialogueData();
        DialogueComponents.Saver.Save(dialogueData);
        
        DL.Log($"{_dialogueId} saved");
    }

    [ContextMenu("Load")]
    private void Load()
    {
        DialogueData dialogueData = DialogueComponents.Saver.Load(_dialogueId);
        
        DL.Log($"{dialogueData.Id} loaded");
    }

    private DialogueData GenerateDialogueData()
    {
        List<NodeData> nodes = new(4);
        for (int i = 0; i < nodes.Capacity; i++)
        {
            nodes.Add(new NodeData(i)
            {
                Text = $"node {i}"
            });
        }
        
        return new DialogueData(_dialogueId)
        {
            Nodes = nodes
        };;
    }
}