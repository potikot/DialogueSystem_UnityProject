using PotikotTools.UniTalks;
using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    [SerializeField] private DialogueView _dialogueView;
    [SerializeField] private string _dialogueId;
    
    private async void Start()
    {
        await UniTalksAPI.LoadDialogueGroupAsync("fefee");
        
        await UniTalksAPI.StartDialogueAsync(_dialogueId, _dialogueView);
    }
}