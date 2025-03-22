using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class ChoiceView : MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void SetData(ConnectionData data)
        {
            
        }
    }
}