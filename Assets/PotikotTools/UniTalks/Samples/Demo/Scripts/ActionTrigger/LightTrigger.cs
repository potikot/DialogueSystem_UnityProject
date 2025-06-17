using UnityEngine;

namespace PotikotTools.UniTalks.Demo
{
    [ExecuteAlways]
    public class LightTrigger : MonoBehaviour, IActionTrigger
    {
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        [Header("Light")]
        public Light flickerLight;
        public float minIntensity = 0f;
        public float maxIntensity = 1.2f;

        [Header("Emission")]
        public Material emissionMat;
        public Color emissionColor = Color.white;
        public float minEmission = 0f;
        public float maxEmission = 3f;

        [Header("Flicker Settings")]
        public float flickerChance = 0.2f;
        public float flickerSpeed = 0.05f;

        [Header("Activation")]
        public float flickerDuration = 5f;
        private float _activeUntil = -1f;

        private float _nextFlickerTime = 0f;
        private float _defaultIntensity;
        private Color _defaultEmissionColor;
        
        private void Awake()
        {
            _defaultIntensity = flickerLight.intensity;
            G.LightActionTriggerList.Add(this);

            if (emissionMat)
                _defaultEmissionColor = emissionMat.GetColor(EmissionColor);
        }

        private void Update()
        {
            if (Time.time <= _activeUntil)
            {
                if (Time.time >= _nextFlickerTime)
                {
                    float intensity = (Random.value < flickerChance) ?
                        Random.Range(minIntensity, 0.3f) :
                        Random.Range(0.8f * maxIntensity, maxIntensity);

                    float emission = Mathf.Lerp(minEmission, maxEmission, intensity / maxIntensity);

                    flickerLight.intensity = intensity;
                    
                    if (emissionMat)
                        emissionMat.SetColor(EmissionColor, emissionColor * emission);

                    _nextFlickerTime = Time.time + Random.Range(flickerSpeed, flickerSpeed * 3f);
                }
            }
            else
            {
                flickerLight.intensity = _defaultIntensity;
                
                if (emissionMat)
                    emissionMat.SetColor(EmissionColor, _defaultEmissionColor);
            }
        }
        
        [ContextMenu(nameof(Trigger))]
        public void Trigger()
        {
            _activeUntil = Time.time + flickerDuration;
        }
    }
}