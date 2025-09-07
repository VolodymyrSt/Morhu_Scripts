using DG.Tweening;
using Di;
using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.Services.EventBus;
using Morhu.Infrustructure.Services.EventBus.Signals;
using System;
using System.Collections;
using UnityEngine;

namespace Morhu.NPC
{
    public class CharacterHandler : ConstructedMonoBehaviour
    {
        private const float CHRACTER_DEFEAT_Z_POSITION = 3f;
        private readonly int SPEAKING_HASH = Animator.StringToHash("isSpeaking");

        [SerializeField] private Animator _animator;
        private IEventBus _eventBus;
        private IAudioService _audioService;
        private IPersistanceDataService _persistanceDataService;

        private Coroutine _startSpeakingRoutine;
        private Coroutine _speakingRoutine;
        private Tween _speakingTween;
        private Tween _idleTween;

        private void OnValidate()
        {
            if (_animator == null) 
                _animator = GetComponent<Animator>();
        }

        public override void Construct(IResolvable container)
        {
            _eventBus = container.Resolve<IEventBus>();
            _audioService = container.Resolve<IAudioService>();
            _persistanceDataService = container.Resolve<IPersistanceDataService>();
        }

        private void Awake() => StartIdleAnimation();

        private void Start()
        {
            _eventBus.SubscribeEvent<OnPlayerWonSignal>(PerformDefeatAnimation);
            _eventBus.SubscribeEvent<OnCharacterWonSignal>(PerformVictorySpeech);
        }

        public void StartSpeaking(string clipIdEn, string clipIdUk, Action onEnded)
        {
            if (_startSpeakingRoutine != null)
            {
                StopCoroutine(_startSpeakingRoutine);
                _startSpeakingRoutine = null;
            }
            _startSpeakingRoutine = StartCoroutine(PerformSpeaking(clipIdEn, clipIdUk, onEnded));
        }

        private IEnumerator PerformSpeaking(string clipIdEn, string clipIdUk, Action onEnded)
        {
            var duraction = default(float);

            if (_persistanceDataService.Data.Language == Infrustructure.Services.Localization.Language.En)
                _audioService.BuildSound().WithParent(transform).Play(clipIdEn, out duraction);
            else
                _audioService.BuildSound().WithParent(transform).Play(clipIdUk, out duraction);

            StartSpeakingAnimation();
            yield return new WaitForSeconds(duraction);
            StartIdleAnimation();
            onEnded?.Invoke();
        }

        private void StartSpeakingAnimation()
        {
            StopCurrentTween();
            _animator.SetBool(SPEAKING_HASH, true);
            _speakingRoutine = StartCoroutine(SpeakingAnimationRoutine());
        }

        private void StartIdleAnimation()
        {
            StopCurrentTween();

            _idleTween = DOTween.Sequence()
                .Append(transform.DOLocalMoveY(transform.localPosition.y + 0.03f, 0.5f)
                    .SetEase(Ease.InOutQuad))
                .Join(transform.DOScale(new Vector3(1.02f, 0.98f, 1.02f), 0.5f)
                    .SetEase(Ease.InOutQuad))
                .Append(transform.DOLocalMoveY(transform.localPosition.y, 0.5f)
                    .SetEase(Ease.InOutQuad))
                .Join(transform.DOScale(Vector3.one, 0.5f)
                    .SetEase(Ease.InOutQuad))
                .SetLoops(-1)
                .Play();
        }

        private IEnumerator SpeakingAnimationRoutine()
        {
            var basePos = transform.position;
            var baseScale = Vector3.one;
            var baseRot = transform.eulerAngles;

            while (true)
            {
                float yOffset = UnityEngine.Random.Range(0.015f, 0.035f);
                float xOffset = UnityEngine.Random.Range(-0.015f, 0.015f);
                float duration = UnityEngine.Random.Range(0.2f, 0.35f);
                float scaleFactor = UnityEngine.Random.Range(0.98f, 1.03f);

                _speakingTween = DOTween.Sequence()
                    .Append(transform.DOLocalMove(basePos + new Vector3(xOffset, yOffset, 0), duration).SetEase(Ease.InOutSine))
                    .Join(transform.DOScale(baseScale * scaleFactor, duration).SetEase(Ease.InOutSine))
                    .Append(transform.DOLocalMove(basePos, duration).SetEase(Ease.InOutSine))
                    .Join(transform.DOScale(baseScale, duration).SetEase(Ease.InOutSine))
                    .Join(transform.DOLocalRotate(baseRot, duration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine))
                    .Play();

                yield return _speakingTween.WaitForCompletion();
            }
        }

        private void PerformDefeatAnimation(OnPlayerWonSignal signal)
        {
            StartSpeaking("CharacterLost_En", "CharacterLost_Uk", () =>
            {
                transform.DOLocalMoveZ(CHRACTER_DEFEAT_Z_POSITION, 2f)
                    .SetEase(Ease.Linear)
                    .Play()
                    .OnComplete(() => _eventBus.Invoke(new OnCharacterDefeatAnimationEnded()));
            });
        }

        private void PerformVictorySpeech(OnCharacterWonSignal signal) =>
            StartSpeaking("PlayerLost_En", "PlayerLost_Uk", null);

        private void StopCurrentTween()
        {
            _speakingTween?.Kill();
            _idleTween?.Kill();
            _speakingTween = null;
            _animator.SetBool(SPEAKING_HASH, false);

            if (_speakingRoutine != null)
            {
                StopCoroutine(_speakingRoutine);
                _speakingRoutine = null;
            }
        }

        private void OnDestroy()
        {
            _eventBus?.UnSubscribeEvent<OnPlayerWonSignal>(PerformDefeatAnimation);
            _eventBus?.UnSubscribeEvent<OnCharacterWonSignal>(PerformVictorySpeech);

            if (_startSpeakingRoutine != null)
                StopCoroutine(_startSpeakingRoutine);
        }
    }
}
