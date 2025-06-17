using System.Collections;
using UnityEngine;

namespace PotikotTools.UniTalks.Demo
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var runnerGO = new GameObject("[CoroutineRunner]");
                    _instance = runnerGO.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(runnerGO);
                }
                
                return _instance;
            }
        }

        public static Coroutine Run(IEnumerator coroutine)
        {
            if (!Application.isPlaying)
                return null;
            
            return Instance.StartCoroutine(coroutine);
        }

        public static void Stop(Coroutine coroutine)
        {
            if (!Application.isPlaying)
                return;
            
            if (coroutine != null && Instance != null)
                Instance.StopCoroutine(coroutine);
        }
    }
}
