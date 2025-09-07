using System;
using System.Collections;
using UnityEngine;

namespace Morhu.Infrustructure
{
    public class LoadingCurtain : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _curtain;
        [SerializeField] private float _fadeDuration = 1f;
        [SerializeField] private float _durationBeforeFadeIn = 2f;

        public void Procced()
        {
            gameObject.SetActive(true);

            _curtain.alpha = 1f;
            StartCoroutine(WaitAndFadeIn(_durationBeforeFadeIn));
        }

        private IEnumerator WaitAndFadeIn(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            StartFade(() => gameObject.SetActive(false));
        }

        private void StartFade(Action onComplete) => 
            StartCoroutine(FadeRoutine(onComplete));

        private IEnumerator FadeRoutine(Action onComplete)
        {
            float elapsed = 0f;

            while (_fadeDuration > elapsed)
            {
                _curtain.alpha -= 0.01f;
                elapsed += 0.01f;
                yield return new WaitForEndOfFrame();
            }

            onComplete?.Invoke();
        }
    }
}
