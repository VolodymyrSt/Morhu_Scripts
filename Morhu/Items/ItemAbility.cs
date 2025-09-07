using Assets.Morhu.Logic.EnergyLogic;
using Morhu.Deck;
using Morhu.Infrustructure.AssetManagement;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Util;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace Morhu.Items
{
    [Serializable]
    public abstract class ItemAbility
    {
        public abstract IEnumerator Use(bool isPlayer, IAudioService audioService, Item item, Camera camera, EnergySystem ownEnergySystem
            , IAssetProvider assetProvider, CardHand ownCardHand, CardHand oponentCardHand, CardFactory cardFactory, Action onEnded);

        public virtual void Tick() { }
    }

    [Serializable]
    public class IgniteCandleAbility : ItemAbility
    {
        private bool _isDissolvingEffectEnded = false;
        public override IEnumerator Use(bool isPlayer, IAudioService audioService, Item item, Camera camera, EnergySystem ownEnergySystem,
            IAssetProvider assetProvider, CardHand ownCardHand, CardHand oponentCardHand, CardFactory cardFactory, Action onEnded)
        {
            item.StartCoroutine(item.TaggleDissolveEffect(() => _isDissolvingEffectEnded = true));
            item.PerformShakeAnimation();
            audioService.BuildSound().WithParent(item.transform).Play("UseMatches");

            yield return new WaitUntil(() => _isDissolvingEffectEnded);
            ownEnergySystem.IgniteOneCandle();
            yield return new WaitForSeconds(0.3f);
            ownEnergySystem.IgniteOneCandle();

            yield return new WaitForSeconds(0.3f);

            onEnded?.Invoke();
            item.DestroySelf();
        }
    }

    [Serializable]
    public class SpyAbility : ItemAbility
    {
        [SerializeField] private Material _material;
        [SerializeField] private GameObject _cardVisual;

        private Camera _camera;
        private Card _currentSelectedCard;
        private Item _item;

        private bool _isCardChosen = false;
        private bool _isPlayer;

        public override IEnumerator Use(bool isPlayer, IAudioService audioService, Item item, Camera camera, EnergySystem ownEnergySystem,
            IAssetProvider assetProvider, CardHand ownCardHand, CardHand oponentCardHand, CardFactory cardFactory, Action onEnded)
        {
            _isPlayer = isPlayer;
            if (!isPlayer)
            {
                yield return new WaitForSeconds(0.3f);
                item.StartCoroutine(item.TaggleDissolveEffect(() => onEnded?.Invoke()));
            }

            _camera = camera;
            _item = item;

            yield return new WaitUntil(() => _isCardChosen);

            yield return new WaitForSeconds(1f);

            item.StartCoroutine(item.TaggleDissolveEffect(() => { 
                onEnded?.Invoke();
                item.DestroySelf();
            }));
        }

        public override void Tick()
        {
            if (_isCardChosen) return;
            if (!_isPlayer) return;

            TryToSelectCard();
            TryToChooseCardForSpy();
        }

        public void TryToChooseCardForSpy()
        {
            if (_currentSelectedCard != null && Input.GetMouseButtonDown(0)
                && _currentSelectedCard.IsBelongToCharacter())
            {
                _isCardChosen = true; 
                _material.SetFloat("_visble_amount", 0f);
                _material.SetTexture("_FrontTexture", _currentSelectedCard.GetMainTexture());
                _item.AddDissolvingMaterial(_material);
                _cardVisual.SetActive(true);
                _currentSelectedCard = null;
            }
        }

        private void TryToSelectCard()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                if (hit.collider.TryGetComponent(out Card card))
                {
                    if (_currentSelectedCard != card)
                    {
                        TryToClearSelectedCard();

                        _currentSelectedCard = card;
                    }
                    else
                        return;
                }
            }
            else
                TryToClearSelectedCard();
        }

        private void TryToClearSelectedCard()
        {
            if (_currentSelectedCard != null)
            {
                _currentSelectedCard = null;
            }
        }
    }

    [Serializable]
    public class ChangeAbility : ItemAbility
    {
        private Camera _camera;
        private Card _currentSelectedCard;
        private IAssetProvider _assetProvider;
        private CardHand _ownCardHand;
        private CardFactory _cardFactory;

        private bool _isCardChosen = false;
        private bool _isPlayer;

        public override IEnumerator Use(bool isPlayer, IAudioService audioService, Item item, Camera camera, EnergySystem ownEnergySystem,
            IAssetProvider assetProvider, CardHand ownCardHand, CardHand oponentCardHand, CardFactory cardFactory, Action onEnded)
        {
            _isPlayer = isPlayer;
            _ownCardHand = ownCardHand;
            _assetProvider = assetProvider;
            _cardFactory = cardFactory;
            _camera = camera;

            if (!_isPlayer)
            {
                var card = _ownCardHand.GetRandomCard();
                var parent = card.transform.parent;
                var position = card.transform.position;
                _ownCardHand.RemoveCard(card);
                card.DestroySelf();

                var newCard = _cardFactory.CreateRandomUsedCard(position, parent);
                newCard.SetHand(_ownCardHand);
                newCard.SetIfCardBelongToCharacter(true);
                AssetPath.RemoveUsedCardPath(newCard.GetSelfPath());
                _ownCardHand.AddCard(newCard);
                yield return new WaitForSeconds(0.3f);
                item.StartCoroutine(item.TaggleDissolveEffect(() => {
                    item.DestroySelf();
                    onEnded?.Invoke();
                }));
            }

            yield return new WaitUntil(() => _isCardChosen);

            yield return new WaitForSeconds(1f);
            item.StartCoroutine(item.TaggleDissolveEffect(() => {
                item.DestroySelf();
                onEnded?.Invoke();
            }));
        }

        public override void Tick()
        {
            if (_isCardChosen) return;
            if (!_isPlayer) return;

            TryToSelectCard();
            TryTorChangeCard();
        }

        public void TryTorChangeCard()
        {
            if (_currentSelectedCard != null && _ownCardHand.GetForcedCard() != _currentSelectedCard 
                && Input.GetMouseButtonDown(0) && _currentSelectedCard.CanBeSelectedByPlayer())
            {
                _isCardChosen = true;
                var parent = _currentSelectedCard.transform.parent;
                var position = _currentSelectedCard.transform.position;
                _ownCardHand.RemoveCard(_currentSelectedCard);
                _currentSelectedCard.DestroySelf();
                _currentSelectedCard = null;

                var newCard = _cardFactory.CreateRandomUsedCard(position, parent);
                newCard.SetHand(_ownCardHand);
                AssetPath.RemoveUsedCardPath(newCard.GetSelfPath());
                _ownCardHand.AddCard(newCard);
            }
        }

        private void TryToSelectCard()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                if (hit.collider.TryGetComponent(out Card card))
                {
                    if (_currentSelectedCard != card)
                        _currentSelectedCard = card;
                    else
                        return;
                }
            }
            else
                TryToClearSelectedCard();
        }

        private void TryToClearSelectedCard()
        {
            if (_currentSelectedCard != null)
                _currentSelectedCard = null;
        }
    }

    [Serializable]
    public class ForceAbility : ItemAbility
    {
        private Camera _camera;
        private Card _currentSelectedCard;

        private bool _isCardChosen = false;
        private bool _isPlayer;

        public override IEnumerator Use(bool isPlayer, IAudioService audioService, Item item, Camera camera, EnergySystem ownEnergySystem,
            IAssetProvider assetProvider, CardHand ownCardHand, CardHand oponentCardHand, CardFactory cardFactory, Action onEnded)
        {
            _isPlayer = isPlayer;
            if (!_isPlayer)
            {
                var card = oponentCardHand.GetRandomCard();
                oponentCardHand.SetUpForcedCardToPlay(card);
                yield return new WaitForSeconds(0.3f);
                item.StartCoroutine(item.TaggleDissolveEffect(() => {
                    item.DestroySelf();
                    onEnded?.Invoke();
                }));
            }

            _camera = camera;  
    
            yield return new WaitUntil(() => _isCardChosen);

            yield return new WaitForSeconds(0.3f);
            item.StartCoroutine(item.TaggleDissolveEffect(() => {
                item.DestroySelf();
                onEnded?.Invoke();
            }));
        }

        public override void Tick()
        {
            if (_isCardChosen) return;
            if (!_isPlayer) return;

            TryToSelectCard();
            TryToChooseCardForSpy();
        }

        public void TryToChooseCardForSpy()
        {
            if (_currentSelectedCard != null && Input.GetMouseButtonDown(0)
                && _currentSelectedCard.IsBelongToCharacter())
            {
                _isCardChosen = true;
                var cardHand = _currentSelectedCard.GetOwnCardHand();
                cardHand.SetUpForcedCardToPlay(_currentSelectedCard);
                _currentSelectedCard = null;
            }
        }

        private void TryToSelectCard()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                if (hit.collider.TryGetComponent(out Card card))
                {
                    if (_currentSelectedCard != card)
                    {
                        TryToClearSelectedCard();

                        _currentSelectedCard = card;
                    }
                    else
                        return;
                }
            }
            else
                TryToClearSelectedCard();
        }

        private void TryToClearSelectedCard()
        {
            if (_currentSelectedCard != null)
            {
                _currentSelectedCard = null;
            }
        }
    }
}
