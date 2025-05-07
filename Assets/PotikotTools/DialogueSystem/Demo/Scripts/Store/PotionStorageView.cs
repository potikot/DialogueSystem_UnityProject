using System.Collections.Generic;
using UnityEngine;

namespace PotikotTools.DialogueSystem.Demo
{
    public class PotionStorageView : MonoBehaviour
    {
        [SerializeField] private RectTransform _potionViewsContainer;
        [SerializeField] private PotionView _potionViewTemplate;
        
        private List<PotionView> _potionViews = new();

        public void Initialize(PotionStorageConfig config)
        {
            foreach (Potion potion in config.Potions)
            {
                var potionView = Instantiate(_potionViewTemplate, _potionViewsContainer);
                potionView.SetData(potion);
                
                _potionViews.Add(potionView);
            }
        }
    }
}