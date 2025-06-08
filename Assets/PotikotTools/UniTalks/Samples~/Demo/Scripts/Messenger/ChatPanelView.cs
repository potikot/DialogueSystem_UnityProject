using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PotikotTools.UniTalks.Demo
{
    public class ChatPanelView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TextMeshProUGUI _nameLabel;

        private ChatPanelData _panelData;
        
        public void SetData(ChatPanelData panelData)
        {
            _panelData = panelData;
            UpdateView();
        }

        public void UpdateView()
        {
            _avatarImage.sprite = _panelData.Avatar;
            _nameLabel.text = _panelData.Name;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            G.Hud.MessengerWindow.OpenChat(_panelData.DialogueName);
        }
    }
}
