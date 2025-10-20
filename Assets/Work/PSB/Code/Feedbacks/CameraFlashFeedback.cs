using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Work.PSB.Code.Feedbacks
{
    public class CameraFlashFeedback : Feedback
    {
        [Header("Flash Settings")]
        [SerializeField] private Image flashImage;
        [SerializeField] private float maxAlpha = 1f;
        [SerializeField] private float fadeInTime = 0.05f;
        [SerializeField] private float fadeOutTime = 0.4f;
        [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Coroutine flashCoroutine;

        public override void CreateFeedback()
        {
            if (flashImage == null)
            {
                Debug.LogWarning("Flash Image가 할당되지 않았습니다.");
                return;
            }

            if (flashCoroutine != null)
                flashImage.StopCoroutine(flashCoroutine);

            flashCoroutine = flashImage.StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            yield return LerpAlpha(0f, maxAlpha, fadeInTime);
            yield return LerpAlpha(maxAlpha, 0f, fadeOutTime);
            flashCoroutine = null;
        }

        private IEnumerator LerpAlpha(float from, float to, float time)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / time;
                float v = curve.Evaluate(t);
                SetAlpha(Mathf.Lerp(from, to, v));
                yield return null;
            }
            SetAlpha(to);
        }

        private void SetAlpha(float a)
        {
            Color c = flashImage.color;
            c.a = a;
            flashImage.color = c;
        }

        public override void StopFeedback()
        {
        }
        
        
    }
}