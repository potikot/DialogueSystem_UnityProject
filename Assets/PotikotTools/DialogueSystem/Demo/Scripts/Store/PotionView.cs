using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PotikotTools.DialogueSystem.Demo
{
    public class PotionView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _image;

        private Potion _data;

        public void SetData(Potion potion)
        {
            _data = potion;
            UpdateView();
        }

        public void UpdateView()
        {
            _image.sprite = _data.Sprite;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            Vector3 position = transform.position;
            position.x += ((RectTransform)transform).sizeDelta.x * 1.75f;
            
            G.TooltipManager.Show(_data.Name, _data.Description + "\n" + _data.SideEffect, position);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            G.TooltipManager.Hide();
        }
    }
}