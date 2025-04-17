using System;
using System.Collections;
using System.Collections.Generic;
using PotikotTools.DialogueSystem;
using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    [SerializeField] private DialogueView _dialogueView;
    [SerializeField] private string _dialogueId;
    
    private void Start()
    {
        DS.StartDialogue(_dialogueId, _dialogueView);
    }
}