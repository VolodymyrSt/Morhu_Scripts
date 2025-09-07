using Assets.Morhu.Logic.EnergyLogic;
using Assets.Morhu.Logic.VFX;
using DG.Tweening;
using Morhu.Deck.ScriptableObjects;
using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.Services.EventBus;
using Morhu.Infrustructure.Services.EventBus.Signals;
using Morhu.Infrustructure.Services.Localization;
using Morhu.Logic.CameraLogic;
using Morhu.Logic.HealthLogic;
using Morhu.Player;
using Morhu.UI.ToolTip;
using Morhu.Util;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Morhu.Deck
{
    public enum CardType { Spade, Club, Heart, Diamond, RedJocker, BlackJocker }
    public enum CardHight { J, Q, K, A, Joker }

    public class Card : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private CardDescriptionSO _cardDescription;
        [SerializeField] private GameObject _selectionPart;
        [SerializeField] protected MeshRenderer MeshRenderer;
        [SerializeField] protected Texture _mainTexture;

        [Header("VFX")]
        [SerializeField] private Transform _effectPoint;

        [Header("Ability")]
        [SerializeField] private CardType _cardType;
        [SerializeField] private CardHight _cardHight;
        [SerializeReference, SubclassSelector] private CardAbility _ability;

        private CardHand _currentHand;
        private bool _canBeSelectedByPlayer = false;
        private bool _isCardBelongToCharacter = false;
        private Material[] _materials;
        private ToolTipTrigger _toolTipTrigger;
        private VFXBuilder _vFXBuilder;
        private IEventBus _eventBus;
        private IAudioService _audioService;
        private Coroutine _useCardAbilityCoroutine;
        private string _selfPath;
        private bool _isUsed;

        public CardType CardType => _cardType;
        public CardHight CardHight => _cardHight;

        public virtual void Init(ToolTipHandler toolTipHandler, VFXBuilder vFXBuilder,
            IEventBus eventBus, string cardPath, IAudioService audioService, IPersistanceDataService persistanceDataService)
        {
            _selectionPart.SetActive(false);
            _materials = MeshRenderer.materials;
            _vFXBuilder = vFXBuilder;
            _eventBus = eventBus;
            _audioService = audioService;
            _selfPath = cardPath;
            _isUsed = false;

            var cardName = _cardDescription.GetDataByLanguage(persistanceDataService.Data.Language).Name;
            var cardAbility = _cardDescription.GetDataByLanguage(persistanceDataService.Data.Language).AbilityDescription;
            var cardAditional = _cardDescription.GetDataByLanguage(persistanceDataService.Data.Language).Aditional;

            _toolTipTrigger = new ToolTipTrigger(toolTipHandler, cardName, cardAbility, cardAditional,
                ToolTipPosition.BottomRight);

            _eventBus?.SubscribeEvent<OnPlayerWonSignal>(StopCard);
            _eventBus?.SubscribeEvent<OnCharacterWonSignal>(StopCard);

            if (_ability is BlackJockerAbility blackJoker)
                blackJoker.InitJoker(this);
        }

        public void SetInitialRotation(float offset) =>
            transform.rotation = Quaternion.Euler(180f, 180f + offset, 0f);

        public void MoveToHand(CardHand hand)
        {
            float duration = 0.2f;

            SetHand(hand);
            hand.AddCard(this);

            transform.DOPath(hand.GetPaths(), duration)
                .SetEase(Ease.Linear)
                .Play()
                .OnComplete(() => hand.ArrangeCards());
        }

        public virtual void MoveOnDuel(Vector3[] paths, Action onComplete)
        {
            float duration = 0.35f;

            _audioService.BuildSound().WithParent(transform).Play("ThrowCard");
            _currentHand.RemoveCard(this);

            transform.DOPath(paths, duration)
                .SetEase(Ease.Linear)
                .Play()
                .OnComplete(() => { 
                    onComplete.Invoke();
                });

            var targetRotation = Quaternion.Euler(Vector3.zero);
            transform.DORotateQuaternion(targetRotation, 0.2f)
                .SetEase(Ease.Linear)
                .Play()
                .SetDelay(0.05f);
        }

        public void UseAbility(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem, bool needToBeSwitchedToDuelCamera, Action onEnded)
        {
            PlayVFXEffect();
            TaggleDissolving(() =>
            {
                _useCardAbilityCoroutine = StartCoroutine(_ability.Use(ownCard, oponentCard, ownSelector, oponentSelector, ownHealthSystem, oponentHealthSystem, cameraSwitcher,
                    ownEnergySystem, needToBeSwitchedToDuelCamera, onEnded));
            });
        }

        public void UseAbilityWithoutDissoleEffect(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem, bool needToBeSwitchedToDuelCamera, Action onEnded)
        {
            _useCardAbilityCoroutine = StartCoroutine(_ability.Use(ownCard, oponentCard, ownSelector, oponentSelector, ownHealthSystem, oponentHealthSystem, cameraSwitcher,
                   ownEnergySystem, needToBeSwitchedToDuelCamera, onEnded));
        }

        public void PlayVFXEffect() => 
            _vFXBuilder.BuildForCard(_cardType, _effectPoint.position, _effectPoint, _ability);

        public void HighLight()
        {
            _toolTipTrigger.Show();
            _selectionPart.SetActive(true);
            transform.DOScale(1.05f, 0.1f)
                .SetEase(Ease.Linear)
                .Play().OnComplete(() => {
                    _audioService.BuildSound().WithParent(transform).Play("CardSelection");
                });
        }

        public void DisHighLight()
        {
            _toolTipTrigger.Hide();
            _selectionPart.SetActive(false);
            transform.DOKill();
            transform.DOScale(1f, 0.1f)
                .SetEase(Ease.Linear)
                .Play();
        }

        public void SetIfCardCanBeSelectedByPlayer(bool value) =>
            _canBeSelectedByPlayer = value;  
        
        public void SetIfCardBelongToCharacter(bool value) =>
            _isCardBelongToCharacter = value;

        public void TaggleDissolving(Action onDissolved) => 
            StartCoroutine(TaggleDissolveEffect(onDissolved));

        public CardAbility GetCardAbility() => _ability;
        public Texture GetMainTexture() => _mainTexture;
        public string GetSelfPath() => _selfPath;
        public CardHand GetOwnCardHand() => _currentHand;

        public void SetHand(CardHand cardHand) => _currentHand = cardHand;
        public void SetCardAbility(CardAbility cardAbility) => _ability = cardAbility;
        public void SetCardToBeUsed() => _isUsed = true;

        public bool CanBeSelectedByPlayer() => _canBeSelectedByPlayer;
        public bool IsBelongToCharacter() => _isCardBelongToCharacter;
        public bool IsUsed() => _isUsed;

        public void DestroySelf()
        {
            _eventBus?.UnSubscribeEvent<OnPlayerWonSignal>(StopCard);
            _eventBus?.UnSubscribeEvent<OnCharacterWonSignal>(StopCard);

            Destroy(gameObject);
        }

        private IEnumerator TaggleDissolveEffect(Action onDissolved)
        {
            var waitTime = 0.3f;
            var visibleAmountMultiplayer = 1f;
            var currentVisibleAmount = 0f;
            var limitVisibleAmount = 1f;

            _audioService.BuildSound().WithParent(transform).Play("DissolveCard");

            while (currentVisibleAmount <= limitVisibleAmount)
            {
                foreach (var material in _materials)
                {
                    currentVisibleAmount += visibleAmountMultiplayer * Time.deltaTime;
                    material.SetFloat("_visble_amount", currentVisibleAmount);
                    yield return null;
                }
            }

            yield return new WaitForSeconds(waitTime);

            onDissolved?.Invoke();
        }


        private void StopCard(OnPlayerWonSignal signal)
        {
            if (_useCardAbilityCoroutine != null)
                StopCoroutine(_useCardAbilityCoroutine);
        }

        private void StopCard(OnCharacterWonSignal signal)
        {
            if (_useCardAbilityCoroutine != null)
                StopCoroutine(_useCardAbilityCoroutine);
        }
    }
}
