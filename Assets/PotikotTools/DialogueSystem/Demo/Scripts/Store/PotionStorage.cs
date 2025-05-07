using System;
using UnityEngine;

namespace PotikotTools.DialogueSystem.Demo
{
    public class PotionStorage : MonoBehaviour
    {
        [SerializeField] private PotionStorageConfig _config;
        [SerializeField] private PotionStorageView _view;

        public void Start()
        {
            _view.Initialize(_config);
        }
    }
}