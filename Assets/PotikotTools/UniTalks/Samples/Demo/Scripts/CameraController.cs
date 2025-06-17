using System;
using System.Collections;
using UnityEngine;

namespace PotikotTools.UniTalks.Demo
{
    public class CameraController : MonoBehaviour
    {
        private static Transform CameraTransform;
        private static Quaternion _initialRotation;

        private static Coroutine _idleRoutine;
        
        private void Awake()
        {
            CameraTransform = Camera.main.transform;
            _initialRotation = CameraTransform.rotation;
            G.CameraController = this;
        }

        private void Start()
        {
            _idleRoutine = CoroutineRunner.Run(HeadSwayRoutine(10000f));
        }

        public static void Look(Vector3 position, float speed, float time) => CoroutineRunner.Run(LookRoutine(position, speed, time));

        private static IEnumerator LookRoutine(Vector3 targetPosition, float speed, float time)
        {
            CoroutineRunner.Stop(_idleRoutine);
            
            Quaternion targetRotation = Quaternion.LookRotation(CameraTransform.position - targetPosition);
            yield return SmoothRotate(CameraTransform, targetRotation, speed);
            yield return CoroutineRunner.Run(HeadSwayRoutine(time));
            yield return SmoothRotate(CameraTransform, _initialRotation, speed);
            
            _idleRoutine = CoroutineRunner.Run(HeadSwayRoutine(10000f));
        }

        private static IEnumerator SmoothRotate(Transform transform, Quaternion targetRotation, float speed)
        {
            Quaternion startRotation = transform.rotation;
            float angle = Quaternion.Angle(startRotation, targetRotation);
            float duration = angle / speed;

            float time = 0f;
            while (time < duration)
            {
                float t = time / duration;
                float smoothedT = Mathf.SmoothStep(0f, 1f, t);
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, smoothedT);

                time += time / 100 + Time.deltaTime;
                yield return null;
            }

            transform.rotation = targetRotation;
        }
        
        private static IEnumerator HeadSwayRoutine(float duration)
        {
            float elapsed = 0f;

            Vector3 baseEuler = CameraTransform.localEulerAngles;
            if (baseEuler.x > 180f) baseEuler.x -= 360f;
            if (baseEuler.y > 180f) baseEuler.y -= 360f;
            if (baseEuler.z > 180f) baseEuler.z -= 360f;

            while (elapsed < duration)
            {
                float swayX = Mathf.Sin(elapsed * 2.0f) * 0.3f;
                float swayY = Mathf.Sin(elapsed * 1.5f) * 0.2f;

                Quaternion swayRotation = Quaternion.Euler(baseEuler.x + swayX, baseEuler.y + swayY, baseEuler.z);
                CameraTransform.localRotation = swayRotation;

                elapsed += Time.deltaTime;
                yield return null;
            }

            CameraTransform.localRotation = Quaternion.Euler(baseEuler);
        }
    }
}
