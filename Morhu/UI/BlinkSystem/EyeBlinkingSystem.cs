using Di;
using Morhu.Infrustructure.Services.EventBus;
using Morhu.Infrustructure.Services.EventBus.Signals;
using Morhu.Util;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Morhu.UI.BlinkSystem
{
    public class EyeBlinkingSystem : ConstructedMonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Volume _volume;

        private IEventBus _eventBus;

        private ChromaticAberration _chromaticAberration;
        private Vignette _vignette;

        private Coroutine _openingCoroutine;
        private Coroutine _closingCoroutine;

        public override void Construct(IResolvable container) => 
            _eventBus = container.Resolve<IEventBus>();

        private void Start() => 
            _eventBus.SubscribeEvent<OnCharacterWonSignal>(PerformDeathEyeBlinking);

        public void Init()
        {
            if (!_volume.profile.TryGet<ChromaticAberration>(out _chromaticAberration))
                throw new Exception("ChromaticAberration is Not Found");

            if (!_volume.profile.TryGet<Vignette>(out _vignette))
                throw new Exception("Vignette is Not Found");
        }

        public void PerformBlinking()
        {
            EnableBlurEffect();
            _animator.SetBool("isBlinking", true);
        }

        private void PerformDeathEyeBlinking(OnCharacterWonSignal signal)
        {
            gameObject.SetActive(true);
            _animator.SetTrigger("DeathBlinking");
        }

        //Animation Event
        public void DisableEffects()
        {
            StartCoroutine(PerformSmoothVignetteEffectDissolving());
            StartCoroutine(PerformSmoothBlurEffectDissolving());
        }

        //Animation Event
        public void CloseEyeUsingVignette()
        {
            CkeckCoroutinesForNull();
            _closingCoroutine = StartCoroutine(PerformSmoothEyeClosing());
        }

        //Animation Event
        public void OpenEyeUsingVignette()
        {
            CkeckCoroutinesForNull();
            _openingCoroutine = StartCoroutine(PerformSmoothEyeOpening());
        }

        //Animation Event
        public void ChangeToRoundBlur() =>
            _vignette.rounded.value = true;

        //Animation Event
        public void UnChangeRoundBlur() =>
            _vignette.rounded.value = false;

        //Animation Event
        public void OnDeathEyeBlinkingAnimationEnded() => 
            _eventBus.Invoke(new OnPlayerDefeatAnimationEnded());

        private void EnableBlurEffect()
        {
            _chromaticAberration.active = true;
            _chromaticAberration.intensity.value = 1f;
            _vignette.smoothness.value = 1f;
        }

        private IEnumerator PerformSmoothEyeOpening()
        {
            while (_vignette.intensity.value >= 0.5f)
            {
                _vignette.intensity.value -= Time.deltaTime * Constants.VALUE_INCREMENTER;
                yield return new WaitForEndOfFrame();
            }
        }          
        
        private IEnumerator PerformSmoothEyeClosing()
        {
            while (_vignette.intensity.value <= 1f)
            {
                _vignette.intensity.value += Time.deltaTime * Constants.VALUE_INCREMENTER;
                yield return new WaitForEndOfFrame();
            }
        }     
        
        private IEnumerator PerformSmoothBlurEffectDissolving()
        {
            while (_chromaticAberration.intensity.value >= 0f)
            {
                _chromaticAberration.intensity.value -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            _chromaticAberration.active = false;
        }
        
        private IEnumerator PerformSmoothVignetteEffectDissolving()
        {
            while (_vignette.intensity.value >= 0.25f)
            {
                _vignette.intensity.value -= Time.deltaTime * Constants.BIG_VALUE_INCREMENTER;
                _vignette.smoothness.value -= Time.deltaTime * Constants.BIG_VALUE_INCREMENTER;
                yield return new WaitForEndOfFrame();
            }
        }

        private void CkeckCoroutinesForNull()
        {
            if (_closingCoroutine != null)
                StopCoroutine(_closingCoroutine);

            if (_openingCoroutine != null)
                StopCoroutine(_openingCoroutine);
        }

        private void OnDestroy() => 
            _eventBus?.UnSubscribeEvent<OnCharacterWonSignal>(PerformDeathEyeBlinking);
    }
}
