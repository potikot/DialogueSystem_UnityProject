using TMPro;
using UnityEngine;

namespace PotikotTools.UniTalks.Demo
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _headerLabel;
        [SerializeField] private TextMeshProUGUI _contentLabel;

        public void Show(string header, string content, Vector3 position)
        {
            _headerLabel.text = header;
            _contentLabel.text = content;
            
            transform.position = position;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}