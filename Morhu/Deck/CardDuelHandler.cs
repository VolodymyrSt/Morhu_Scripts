using Assets.Morhu.Logic.EnergyLogic;
using Di;
using Morhu.Infrustructure.AssetManagement;
using Morhu.Logic.CameraLogic;
using Morhu.Logic.HealthLogic;
using Morhu.NPC;
using Morhu.Player;
using System;
using UnityEngine;

namespace Morhu.Deck
{
    public class CardDuelHandler : ConstructedMonoBehaviour
    {
        [Header("CardMovePaths")]
        [SerializeField] private Transform[] _playerCardMovePaths;
        [SerializeField] private Transform[] _characterCardMovePaths;

        [Header("CardPositions")]
        [SerializeField] private Transform _playerCardPosition;
        [SerializeField] private Transform _characterCardPosition;

        private HealthSystem _playerHealthSystem;
        private HealthSystem _characterHealthSystem;
        
        private EnergySystem _playerEnergySystem;
        private EnergySystem _characterEnergySystem;

        private CameraSwitcher _cameraSwitcher;

        private PlayerCardSelector _playerCardSelector;
        private CharacterCardSelector _characteCardSelector;

        private Card _playerCard = null;
        private Card _characterCard = null;

        public override void Construct(IResolvable container)
        {
            _playerHealthSystem = container.Resolve<HealthSystem>("Player");
            _characterHealthSystem = container.Resolve<HealthSystem>("Character");

            _playerEnergySystem = container.Resolve<EnergySystem>("Player");
            _characterEnergySystem = container.Resolve<EnergySystem>("Character");

            _cameraSwitcher = container.Resolve<CameraSwitcher>();
            _playerCardSelector = container.Resolve<PlayerCardSelector>();
            _characteCardSelector = container.Resolve<CharacterCardSelector>();
        }

        public void SetPlayerCardForDuel(Card card)
        {
            TryToClearPlayerCard();
            card.MoveOnDuel(GetPlayerCardMovePaths(), () => SetPlayerCard(card));
        }

        public void SetCharacterCardForDuel(Card card)
        {
            TryToClearCharacterCard();
            card.MoveOnDuel(GetCharacterCardMovePaths(), () => SetCharacterCard(card));
        }

        public void PerformDuel(bool isPlayerTurn, Action onDuelEnded)
        {
            if (_playerCard != null && _characterCard != null)
            {
                if (isPlayerTurn)
                    PerformCardsAbility(_playerCard, _characterCard, _playerCardSelector, _characteCardSelector, _playerHealthSystem
                        , _characterHealthSystem, _playerEnergySystem, _characterEnergySystem, onDuelEnded);
                else
                    PerformCardsAbility(_characterCard, _playerCard, _characteCardSelector, _playerCardSelector, _characterHealthSystem
                        , _playerHealthSystem, _characterEnergySystem, _playerEnergySystem, onDuelEnded);
            }
            else if (_playerCard != null && _characterCard == null)
                PerformCardAbility(_playerCard, _characterCard, _playerCardSelector, _characteCardSelector, _playerHealthSystem
                    , _characterHealthSystem, _playerEnergySystem, onDuelEnded);
            else if (_characterCard != null && _playerCard == null)
                PerformCardAbility(_characterCard, _playerCard, _characteCardSelector, _playerCardSelector, _characterHealthSystem
                    , _playerHealthSystem, _characterEnergySystem, onDuelEnded);
            else
                onDuelEnded?.Invoke();
        }

        public Vector3[] GetPlayerCardMovePaths()
        {
            Vector3[] paths = new Vector3[_playerCardMovePaths.Length];
            for (int i = 0; i < _playerCardMovePaths.Length; i++)
                paths[i] = _playerCardMovePaths[i].position;

            return paths;
        }

        public Vector3[] GetCharacterCardMovePaths()
        {
            Vector3[] paths = new Vector3[_characterCardMovePaths.Length];
            for (int i = 0; i < _characterCardMovePaths.Length; i++)
                paths[i] = _characterCardMovePaths[i].position;

            return paths;
        }

        public Card GetPlayerCard() => _playerCard;

        private void PerformCardAbility(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, EnergySystem ownEnergySystem, Action onDuelEnded)
        {
            ownCard.UseAbility(ownCard, oponentCard, ownSelector, oponentSelector, ownHealthSystem
                , oponentHealthSystem, _cameraSwitcher, ownEnergySystem, false, () =>
                {
                    onDuelEnded?.Invoke();
                    ClearCards();
                });
        }

        private void PerformCardsAbility(Card card, Card oponentCard, CardSelector cardSelector, CardSelector oponentSelector, HealthSystem cardHealthSystem,
            HealthSystem oponentHealthSystem, EnergySystem cardEnergySystem, EnergySystem oponentEnergySystem, Action onDuelEnded)
        {
            if (CheckForDiamondAbility(card, oponentCard, cardSelector, oponentSelector, cardHealthSystem,
                oponentHealthSystem, cardEnergySystem, oponentEnergySystem, onDuelEnded))
                return;  
            
            else if (CheckForDefenceAbility(card, oponentCard, cardSelector, oponentSelector, cardHealthSystem,
                oponentHealthSystem, cardEnergySystem, oponentEnergySystem, onDuelEnded))
                return;
            
            else if (CheckForCopyAbility(card, oponentCard, cardSelector, oponentSelector, cardHealthSystem,
                oponentHealthSystem, cardEnergySystem, oponentEnergySystem, onDuelEnded))
                return;

            else if (card.GetCardAbility() is AceDefenceAbility)
                DisposeCards(card, oponentCard, cardEnergySystem, onDuelEnded);
            else if (oponentCard.GetCardAbility() is AceDefenceAbility)
                DisposeCards(oponentCard, card, oponentEnergySystem, onDuelEnded);
            else
            {
                card.UseAbility(card, oponentCard, cardSelector, oponentSelector, cardHealthSystem, oponentHealthSystem, _cameraSwitcher, cardEnergySystem, true, () =>
                    PerformCardAbility(oponentCard, card, oponentSelector, cardSelector, oponentHealthSystem, cardHealthSystem, oponentEnergySystem, onDuelEnded));
            }
        }

        private bool CheckForCopyAbility(Card card, Card oponentCard, CardSelector cardSelector, CardSelector oponentSelector, HealthSystem cardHealthSystem
            , HealthSystem oponentHealthSystem, EnergySystem cardEnergySystem, EnergySystem oponentEnergySystem, Action onDuelEnded)
        {
            if (card.GetCardAbility() is CopyAbility && oponentCard.GetCardAbility() is CopyAbility)
            {
                card.UseAbility(card, null, cardSelector, oponentSelector, cardHealthSystem, oponentHealthSystem, _cameraSwitcher, cardEnergySystem, true, () =>
                    PerformCardAbility(oponentCard, null, oponentSelector, cardSelector, oponentHealthSystem, cardHealthSystem, oponentEnergySystem, onDuelEnded));
                return true;
            }
            return false;
        }

        private bool CheckForDiamondAbility(Card card, Card oponentCard, CardSelector cardSelector, CardSelector oponentSelector, HealthSystem cardHealthSystem
           , HealthSystem oponentHealthSystem, EnergySystem cardEnergySystem, EnergySystem oponentEnergySystem, Action onDuelEnded)
        {
            if (card.GetCardAbility() is AceStealingAbility && oponentCard.GetCardAbility() is CopyAbility)
            {
                oponentCard.TaggleDissolving(null);
                PerformCardAbility(card, null, cardSelector, oponentSelector, cardHealthSystem, oponentHealthSystem, cardEnergySystem, onDuelEnded);
                return true;
            }
            else if (oponentCard.GetCardAbility() is AceStealingAbility && card.GetCardAbility() is CopyAbility)
            {
                card.TaggleDissolving(null);
                PerformCardAbility(oponentCard, null, oponentSelector, cardSelector, oponentHealthSystem, cardHealthSystem, oponentEnergySystem, onDuelEnded);
                return true;
            }
            else if (card.GetCardAbility() is AceStealingAbility && oponentCard.GetCardAbility() is AceDefenceAbility)
            {
                DisposeCards(oponentCard, card, oponentEnergySystem, onDuelEnded);
                return true;
            }
            else if (oponentCard.GetCardAbility() is AceStealingAbility && card.GetCardAbility() is AceDefenceAbility)
            {
                DisposeCards(card, oponentCard, cardEnergySystem, onDuelEnded);
                return true;
            }
            else if (card.GetCardAbility() is AceStealingAbility)
            {
                oponentCard.TaggleDissolving(null);
                PerformCardAbility(card, oponentCard, cardSelector, oponentSelector, cardHealthSystem, oponentHealthSystem, cardEnergySystem, onDuelEnded);
                return true;
            }
            else if (oponentCard.GetCardAbility() is AceStealingAbility)
            {
                card.TaggleDissolving(null);
                PerformCardAbility(oponentCard, card, oponentSelector, cardSelector, oponentHealthSystem, cardHealthSystem, oponentEnergySystem, onDuelEnded);
                return true;
            }
            return false;
        }

        private bool CheckForDefenceAbility(Card card, Card oponentCard, CardSelector cardSelector, CardSelector oponentSelector, HealthSystem cardHealthSystem
          , HealthSystem oponentHealthSystem, EnergySystem cardEnergySystem, EnergySystem oponentEnergySystem, Action onDuelEnded)
        {
            if (card.GetCardAbility() is DefenceAbility && (oponentCard.GetCardAbility() is AttackAbility || oponentCard.GetCardAbility() is AceAttackAbility))
            {
                oponentCard.TaggleDissolving(null);
                PerformCardAbility(card, oponentCard, cardSelector, oponentSelector, cardHealthSystem, oponentHealthSystem, cardEnergySystem, onDuelEnded);
                return true;
            }
            else if (oponentCard.GetCardAbility() is DefenceAbility && (card.GetCardAbility() is AttackAbility || card.GetCardAbility() is AceAttackAbility))
            {
                card.TaggleDissolving(null);
                PerformCardAbility(oponentCard, card, oponentSelector, cardSelector, oponentHealthSystem, cardHealthSystem, oponentEnergySystem, onDuelEnded);
                return true;
            }
            return false;
        }

        private void DisposeCards(Card card, Card oponentCard, EnergySystem cardEnergySystem,  Action onDuelEnded)
        {
            card.PlayVFXEffect();

            if (oponentCard.CardType == CardType.BlackJocker)
                oponentCard.PlayVFXEffect();

            cardEnergySystem.BlowOneCandle();
            card.TaggleDissolving(null);
            oponentCard.TaggleDissolving(() =>
            {
                onDuelEnded?.Invoke();
                ClearCards();
            });
        }

        private void ClearCards()
        {
            TryToClearPlayerCard();
            TryToClearCharacterCard();
        }

        private void TryToClearCharacterCard()
        {
            if (_characterCard != null)
            {
                AssetPath.AddUsedCardPath(_characterCard.GetSelfPath());
                _characterCard.DestroySelf();
                _characterCard = null;
            }
        }

        private void TryToClearPlayerCard()
        {
            if (_playerCard != null)
            {
                AssetPath.AddUsedCardPath(_playerCard.GetSelfPath());
                _playerCard.DestroySelf();
                _playerCard = null;
            }
        }

        private void SetPlayerCard(Card card)
        {
            _playerCard = card;
            card.transform.SetParent(_playerCardPosition);
        }

        private void SetCharacterCard(Card card)
        {
            _characterCard = card;
            card.transform.SetParent(_characterCardPosition);
        }
    }
}
