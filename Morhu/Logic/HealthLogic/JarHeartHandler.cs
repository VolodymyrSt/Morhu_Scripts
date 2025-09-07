using DG.Tweening;
using Morhu.Util;
using System;
using System.Collections;
using UnityEngine;

namespace Morhu.Logic.HealthLogic
{
    public class JarHeartHandler : MonoBehaviour
    {
        private const float MAX_LIGHT_INTENSITY = 0.1f;
        private const float Y_INTERVAL = 0.05f;

        [Header("Base")]
        [SerializeField] private GameObject _heart;
        [SerializeField] private Light _light;

        private Tween _heartBeatTween;

        private void Start() =>
            PerformHeartBeatAnimation();

        private bool _isAlive = true;

        public void KillSelf(Action onKilled = null)
        {
            _isAlive = false;

            _heartBeatTween?.Kill();

            StartCoroutine(PerformLightTrobbing(0));
            _heart.transform.DOMoveY(_heart.transform.position.y - Y_INTERVAL, 2f)
                .SetEase(Ease.Linear)
                .Play()
                .OnComplete(() => onKilled?.Invoke());
        }


        public void HealSelf(Action onHealled = null)
        {
            _isAlive = true;

            StartCoroutine(PerformLightTrobbing(MAX_LIGHT_INTENSITY));
            _heart.transform.DOMoveY(_heart.transform.position.y + Y_INTERVAL, 2f)
                .SetEase(Ease.Linear)
                .Play()
                .OnComplete(() => {
                    PerformHeartBeatAnimation();
                    onHealled?.Invoke();
                });
        }

        public bool IsAlive() => _isAlive;

        private IEnumerator PerformLightTrobbing(float endedValue)
        {
            yield return new WaitForSeconds(0.05f);
            _light.intensity = 0;
            yield return new WaitForSeconds(0.05f);
            _light.intensity = MAX_LIGHT_INTENSITY;
            yield return new WaitForSeconds(0.05f);
            _light.intensity = 0;
            yield return new WaitForSeconds(0.05f);
            _light.intensity = MAX_LIGHT_INTENSITY;
            yield return new WaitForSeconds(0.05f);
            _light.intensity = 0;
            yield return new WaitForSeconds(0.05f);
            _light.intensity = MAX_LIGHT_INTENSITY;
            yield return new WaitForSeconds(0.05f);
            _light.intensity = 0;
            yield return new WaitForSeconds(0.05f);
            _light.intensity = MAX_LIGHT_INTENSITY;
            yield return new WaitForSeconds(0.05f);
            _light.intensity = 0;
            yield return new WaitForSeconds(0.05f);
            _light.intensity = endedValue;
        }

        private void PerformHeartBeatAnimation()
        {
            _heartBeatTween?.Kill();
            _heart.transform.localScale = Vector3.one;

            _heartBeatTween = _heart.transform
                .DOScale(1.1f, Constants.LONG_ANIMATION_DURATION)
                .SetEase(Ease.InOutQuad)
                .SetLoops(-1, LoopType.Yoyo)
                .Play();
        }
    }
}
