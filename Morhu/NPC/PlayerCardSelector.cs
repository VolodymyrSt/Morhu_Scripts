using Assets.Morhu.Logic.EnergyLogic;
using Morhu.Deck;
using UnityEngine;

namespace Morhu.Player
{
    public class PlayerCardSelector : CardSelector
    {
        private readonly Camera _camera;
        private readonly CardDuelHandler _cardDuelHandler;
        private readonly CardHand _cardHand;

        private Card _currentSelectedCard;
        private bool _isPlayerCardOnDuel;
        public bool IsCardOnDuel { get => _isPlayerCardOnDuel; set => _isPlayerCardOnDuel = value; }

        public PlayerCardSelector(Camera camera, CardDuelHandler cardDuelHandler, CardHand cardHand)
        {
            _camera = camera;
            _cardDuelHandler = cardDuelHandler;
            _cardHand = cardHand;
        }

        public void Update()
        {
            if (!IsCardOnDuel)
            {
                TryToSelectCard();
                TryToChooseCardForDuel();
            }
        }

        public override void TryToChooseCardForDuel()
        {
            if (_currentSelectedCard != null && Input.GetMouseButtonDown(0))
            {
                _currentSelectedCard.DisHighLight();
                _currentSelectedCard.SetIfCardCanBeSelectedByPlayer(false);
                _cardDuelHandler.SetPlayerCardForDuel(_currentSelectedCard);
                _cardHand.SetIfForcedToPlayCard(false);
                _currentSelectedCard = null;
                IsCardOnDuel = true;
            }
        }

        private void TryToSelectCard()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                if (hit.collider.TryGetComponent(out Card card))
                {
                    if (!card.CanBeSelectedByPlayer()) return;

                    if (_cardHand.IsForcedToPlayCard())
                    {
                        if (_cardHand.GetForcedCard() == card)
                        {
                            if (_currentSelectedCard != card)
                                SelectCard(card);
                        }
                        else
                            TryToClearSelectedCard();
                    }
                    else
                    {
                        if (_currentSelectedCard != card)
                            SelectCard(card);
                        else
                            return;
                    }
                }
            }
            else
                TryToClearSelectedCard();
        }

        private void SelectCard(Card card)
        {
            TryToClearSelectedCard();
            _currentSelectedCard = card;
            card.HighLight();
        }

        private void TryToClearSelectedCard()
        {
            if (_currentSelectedCard != null)
            {
                _currentSelectedCard.DisHighLight();
                _currentSelectedCard = null;
            }
        }
    }
}
