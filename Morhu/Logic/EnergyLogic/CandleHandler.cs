using DG.Tweening;
using Morhu.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Morhu.Logic.EnergyLogic
{
    public class CandleHandler : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private List<GameObject> _candleLightParts;
        [SerializeField] private Light _light;

        private bool _isBlew = false;

        private void OnEnable() => 
            StartCoroutine(PerformLighMovement());

        public void BlowSelf()
        {
            _isBlew = true;
            StartCoroutine(SetCandleBlowState(0f, false));
        }

        public void IgniteSelf()
        {
            _isBlew = false;
            StartCoroutine(SetCandleBlowState(1f, true));
        }

        private IEnumerator SetCandleBlowState(float scaleValue, bool value)
        {
            foreach (var part in _candleLightParts)
            {
                if (value)
                    part.SetActive(value);

                part.transform.DOScale(scaleValue, Constants.LONG_ANIMATION_DURATION)
                    .SetEase(Ease.Linear)
                    .Play()
                    .OnComplete(() => part.SetActive(value));

                yield return new WaitForSeconds(Constants.LONG_ANIMATION_DURATION);
            }
        }

        private IEnumerator PerformLighMovement()
        {
            foreach (var part in _candleLightParts)
            {
                part.transform.DOScale(1.1f, Constants.LONG_ANIMATION_DURATION)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Yoyo)
                    .Play();

                yield return new WaitForSeconds(Constants.LONG_ANIMATION_DURATION);
            }
        }    

        public bool IsBlew() => _isBlew;
    }
}
