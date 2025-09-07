using Morhu.Deck;
using Morhu.Logic.HealthLogic;
using Morhu.Player;

namespace Morhu.NPC
{
    public class CharacterCardSelector : CardSelector
    {
        private readonly CardHand _characterHand;
        private readonly CardDuelHandler _cardDuelHandler;
        private readonly HealthSystem _characterHealthSystem;
        private readonly HealthSystem _playerHealthSystem;
        private readonly PlayerCardSelector _playerCardSelector;
        private readonly CharacterHandler _characterHandler;

        public CharacterCardSelector(CardHand characterHand, CardDuelHandler cardDuelHandler, HealthSystem characterHeealthSystem,
            HealthSystem playerHealthSystem, PlayerCardSelector playerCardSelector, CharacterHandler characterHandler)
        {
            _characterHand = characterHand;
            _cardDuelHandler = cardDuelHandler;
            _characterHealthSystem = characterHeealthSystem;
            _playerHealthSystem = playerHealthSystem;
            _playerCardSelector = playerCardSelector;
            _characterHandler = characterHandler;
        }

        public override void TryToChooseCardForDuel()
        {
            if (IsBlocked()) return;

            if (_characterHand.IsForcedToPlayCard())
            {
                var forcedCard = _characterHand.GetForcedCard();

                if (forcedCard != null)
                {
                    ChooseCardForDuel(forcedCard);
                    _characterHand.SetIfForcedToPlayCard(false);
                }
                return;
            }
            if (_cardDuelHandler.GetPlayerCard() != null)
            {
                var playerCard = _cardDuelHandler.GetPlayerCard();

                switch (playerCard.CardType)
                {
                    case CardType.Spade:
                        TryToDefenceSpadeCard(playerCard);
                        break;
                    case CardType.Diamond:
                        TryToDefenceDiamondCard(playerCard);
                        break;
                    case CardType.Heart:
                        TryToDefenceHeartCard();
                        break;
                    case CardType.Club:
                        TryToDefenceClubCard();
                        break;
                    case CardType.BlackJocker:
                        TryToDefenceBlackJocker();
                        break;
                    case CardType.RedJocker:
                        TryToDefenceRedJocker();
                        break;
                }
            }
            else
                TryToChooseBestOption();
        }

        private void TryToChooseBestOption()
        {
            if (_playerCardSelector.IsBlocked())
            {
                if (_playerHealthSystem.GetCountOfAllDiedHearts() == 3 &&
                _characterHand.TryToGetCardByType(CardType.Spade, out Card spadeKingCard) && spadeKingCard.CardHight == CardHight.K)
                {
                    _characterHandler.StartSpeaking("Speech2_En", "Speech2_Uk", null);
                    ChooseCardForDuel(spadeKingCard);
                }
                else if (_characterHand.TryToGetCardByType(CardType.Spade, out Card spadeCard))
                {
                    _characterHandler.StartSpeaking("Speech1_En", "Speech1_Uk", null);
                    ChooseCardForDuel(spadeCard);
                }
                else if (_characterHealthSystem.GetCountOfAllDiedHearts() > _playerHealthSystem.GetCountOfAllDiedHearts() &&
                _characterHand.TryToGetCardByType(CardType.RedJocker, out Card redJockerCard))
                {
                    _characterHandler.StartSpeaking("Speech6_En", "Speech6_Uk", null);
                    ChooseCardForDuel(redJockerCard);
                }
                else if (_characterHand.TryToGetCardByType(CardType.BlackJocker, out Card blackJockerCard))
                    ChooseCardForDuel(blackJockerCard);
                else if (_characterHealthSystem.GetCountOfAllDiedHearts() >= 2 &&
                _characterHand.TryToGetCardByType(CardType.Heart, out Card heartCard))
                    ChooseCardForDuel(heartCard);
                else if (_characterHand.TryToGetCardByType(CardType.Diamond, out Card diamondCard) && diamondCard.CardHight == CardHight.J)
                    ChooseCardForDuel(diamondCard);
                else
                    ChooseRandomLowCardForDuel();
            }
            else
            {
                if (_characterHealthSystem.GetCountOfAllDiedHearts() >= 2 &&
                _characterHand.TryToGetCardByType(CardType.Heart, out Card heartCard))
                    ChooseCardForDuel(heartCard);
                else if (_characterHand.TryToGetCardByType(CardType.Spade, out Card spadeCard))
                    ChooseCardForDuel(spadeCard);
                else if (_characterHand.TryToGetCardByType(CardType.BlackJocker, out Card blackJockerCard))
                {
                    _characterHandler.StartSpeaking("Speech4_En", "Speech4_Uk", null);
                    ChooseCardForDuel(blackJockerCard);
                }
                else if (_characterHealthSystem.GetCountOfAllDiedHearts() > _playerHealthSystem.GetCountOfAllDiedHearts() &&
                _characterHand.TryToGetCardByType(CardType.RedJocker, out Card redJockerCard))
                {
                    _characterHandler.StartSpeaking("Speech6_En", "Speech6_Uk", null);
                    ChooseCardForDuel(redJockerCard);
                }
                else if (_characterHand.TryToGetCardByType(CardType.Diamond, out Card dIamondCard))
                    ChooseCardForDuel(dIamondCard);
                else
                    ChooseRandomLowCardForDuel();
            }
        }

        private void TryToDefenceSpadeCard(Card playerCard)
        {
            if ((playerCard.CardHight == CardHight.K || playerCard.CardHight == CardHight.A) &&
            _characterHand.TryToGetCardByType(CardType.Diamond, out Card diamondCard) && diamondCard.CardHight == CardHight.A)
                ChooseCardForDuel(diamondCard);
            else if (_playerHealthSystem.GetCountOfAllDiedHearts() >= 2 && playerCard.CardHight == CardHight.Q &&
                _characterHand.TryToGetCardByType(CardType.Diamond, out Card diamondAceCard) && diamondAceCard.CardHight == CardHight.A)
                ChooseCardForDuel(diamondAceCard);
            else if (_characterHand.TryToGetCardByType(CardType.Club, out Card clubCard))
                ChooseCardForDuel(clubCard);
            else if (_characterHand.TryToGetCardByType(CardType.BlackJocker, out Card blackJockerCard))
            {
                _characterHandler.StartSpeaking("Speech4_En", "Speech4_Uk", null);
                ChooseCardForDuel(blackJockerCard);
            }
            else if (_characterHand.TryToGetCardByType(CardType.Heart, out Card heartCard))
            {
                _characterHandler.StartSpeaking("Speech5_En", "Speech5_Uk", null);
                ChooseCardForDuel(heartCard);
            }
            else if (_characterHealthSystem.GetCountOfAllDiedHearts() > _playerHealthSystem.GetCountOfAllDiedHearts() &&
            _characterHand.TryToGetCardByType(CardType.RedJocker, out Card redJockerCard))
            {
                _characterHandler.StartSpeaking("Speech4_En", "Speech4_Uk", null);
                ChooseCardForDuel(redJockerCard);
            }
            else if (_characterHand.TryToGetCardByType(CardType.Diamond, out Card dIamondCard))
            {
                _characterHandler.StartSpeaking("Speech5_En", "Speech5_Uk", null);
                ChooseCardForDuel(dIamondCard);
            }
            else
            {
                _characterHandler.StartSpeaking("Speech3_En", "Speech3_Uk", null);
                ChooseRandomLowCardForDuel();
            }
        }

        private void TryToDefenceDiamondCard(Card playerCard)
        {
            if ((playerCard.CardHight == CardHight.Q || playerCard.CardHight == CardHight.K) &&
             _characterHealthSystem.GetCountOfAllDiedHearts() <= 1 &&
             _characterHealthSystem.GetCountOfAllDiedHearts() < _playerHealthSystem.GetCountOfAllDiedHearts() &&
             _characterHand.TryToGetCardByType(CardType.Spade, out Card spadeKingCard) && spadeKingCard.CardHight == CardHight.K)
            {
                _characterHandler.StartSpeaking("Speech2_En", "Speech2_Uk", null);
                ChooseCardForDuel(spadeKingCard);
            }
            else if ((playerCard.CardHight == CardHight.Q || playerCard.CardHight == CardHight.K) &&
             _characterHealthSystem.GetCountOfAllDiedHearts() == 1 && _playerHealthSystem.GetCountOfAllDiedHearts() == 1 &&
             _characterHand.TryToGetCardByType(CardType.Spade, out Card spadeQueenCard) && spadeQueenCard.CardHight == CardHight.Q)
            {
                _characterHandler.StartSpeaking("Speech2_En", "Speech2_Uk", null);
                ChooseCardForDuel(spadeQueenCard);
            }
            else if (_characterHand.TryToGetCardByType(CardType.Club, out Card clubCard))
                ChooseCardForDuel(clubCard);
            else
                ChooseRandomLowCardForDuel();
        }

        private void TryToDefenceHeartCard()
        {
            if (_characterHealthSystem.GetCountOfAllDiedHearts() >= _playerHealthSystem.GetCountOfAllDiedHearts() &&
            _characterHand.TryToGetCardByType(CardType.RedJocker, out Card redJockerCard))
                ChooseCardForDuel(redJockerCard);
            else if (_characterHand.TryToGetCardByType(CardType.Spade, out Card spadeCard))
            {
                _characterHandler.StartSpeaking("Speech1_En", "Speech1_Uk", null);
                ChooseCardForDuel(spadeCard);
            }
            else if (_characterHand.TryToGetCardByType(CardType.BlackJocker, out Card blackJockerCard))
            {
                _characterHandler.StartSpeaking("Speech4_En", "Speech4_Uk", null);
                ChooseCardForDuel(blackJockerCard);
            }
            else if (_characterHand.TryToGetCardByType(CardType.Diamond, out Card diamondCard))
                ChooseCardForDuel(diamondCard);
            else
            {
                _characterHandler.StartSpeaking("Speech3_En", "Speech3_Uk", null);
                ChooseRandomLowCardForDuel();
            }
        }

        private void TryToDefenceClubCard()
        {
            if (_characterHealthSystem.GetCountOfAllDiedHearts() > _playerHealthSystem.GetCountOfAllDiedHearts() &&
            _characterHand.TryToGetCardByType(CardType.RedJocker, out Card redJockerCard))
                ChooseCardForDuel(redJockerCard);
            else if (_characterHealthSystem.GetCountOfAllDiedHearts() >= 1 &&
            _characterHand.TryToGetCardByType(CardType.Heart, out Card heartCard))
                ChooseCardForDuel(heartCard);
            else if (_characterHand.TryToGetCardByType(CardType.BlackJocker, out Card blackJockerCard))
            {
                _characterHandler.StartSpeaking("Speech4_En", "Speech4_Uk", null);
                ChooseCardForDuel(blackJockerCard);
            }
            else if (_characterHand.TryToGetCardByType(CardType.Diamond, out Card diamondCard) && diamondCard.CardHight == CardHight.J)
                ChooseCardForDuel(diamondCard);
            else
                ChooseRandomLowCardForDuel();
        }

        private void TryToDefenceBlackJocker()
        {
            if (_characterHand.TryToGetCardByType(CardType.Club, out Card clubCard))
                ChooseCardForDuel(clubCard);
            else if (_characterHealthSystem.GetCountOfAllDiedHearts() > _playerHealthSystem.GetCountOfAllDiedHearts() &&
             _characterHand.TryToGetCardByType(CardType.RedJocker, out Card redJockerCard))
            {
                _characterHandler.StartSpeaking("Speech4_En", "Speech4_Uk", null);
                ChooseCardForDuel(redJockerCard);
            }
            else if (_characterHand.TryToGetCardByType(CardType.Heart, out Card heartCard))
                ChooseCardForDuel(heartCard);
            else if (_characterHand.TryToGetCardByType(CardType.Diamond, out Card diamondCard))
                ChooseCardForDuel(diamondCard);
            else
                ChooseRandomLowCardForDuel();
        }

        private void TryToDefenceRedJocker()
        {
            if (_characterHealthSystem.GetCountOfAllDiedHearts() == 3 && _characterHand.TryToGetCardByType(CardType.Spade,
             out Card spadeKingCard) && spadeKingCard.CardHight == CardHight.K)
            {
                _characterHandler.StartSpeaking("Speech2_En", "Speech2_Uk", null);
                ChooseCardForDuel(spadeKingCard);
            }
            else if (_characterHealthSystem.GetCountOfAllDiedHearts() == 2 && _characterHand.TryToGetCardByType(CardType.Spade,
             out Card spadeQueenOrAceCard) && (spadeQueenOrAceCard.CardHight == CardHight.A || spadeQueenOrAceCard.CardHight == CardHight.Q))
            {
                _characterHandler.StartSpeaking("Speech2_En", "Speech2_Uk", null);
                ChooseCardForDuel(spadeQueenOrAceCard);
            }
            else if (_characterHealthSystem.GetCountOfAllDiedHearts() > _playerHealthSystem.GetCountOfAllDiedHearts() &&
             _characterHand.TryToGetCardByType(CardType.Club, out Card clubCard))
                ChooseCardForDuel(clubCard);
            else if (_characterHand.TryToGetCardByType(CardType.Heart, out Card heartCard))
                ChooseCardForDuel(heartCard);
            else if (_characterHand.TryToGetCardByType(CardType.Diamond, out Card diamondCard))
                ChooseCardForDuel(diamondCard);
            else
            {
                _characterHandler.StartSpeaking("Speech3_En", "Speech3_Uk", null);
                ChooseRandomLowCardForDuel();
            }
        }

        private void ChooseRandomLowCardForDuel()
        {
            Card card = null;
            if ((_characterHand.TryToGetCardByType(CardType.Spade, out card) && card.CardHight == CardHight.J) ||
                (_characterHand.TryToGetCardByType(CardType.Club, out card) && card.CardHight == CardHight.J) ||
                (_characterHand.TryToGetCardByType(CardType.Heart, out card) && card.CardHight == CardHight.J) ||
                (_characterHand.TryToGetCardByType(CardType.Diamond, out card) && card.CardHight == CardHight.J))
              ChooseCardForDuel(card);
            else if ((_characterHand.TryToGetCardByType(CardType.Spade, out card) && card.CardHight == CardHight.Q) ||
                (_characterHand.TryToGetCardByType(CardType.Club, out card) && card.CardHight == CardHight.Q) ||
                (_characterHand.TryToGetCardByType(CardType.Heart, out card) && card.CardHight == CardHight.Q) ||
                (_characterHand.TryToGetCardByType(CardType.Diamond, out card) && card.CardHight == CardHight.Q))
              ChooseCardForDuel(card);
            else if ((_characterHand.TryToGetCardByType(CardType.Spade, out card) && card.CardHight == CardHight.K) ||
                (_characterHand.TryToGetCardByType(CardType.Club, out card) && card.CardHight == CardHight.K) ||
                (_characterHand.TryToGetCardByType(CardType.Heart, out card) && card.CardHight == CardHight.K) ||
                (_characterHand.TryToGetCardByType(CardType.Diamond, out card) && card.CardHight == CardHight.K))
              ChooseCardForDuel(card);
            else
                ChooseRandomCardForDuel();
        }

        private void ChooseRandomCardForDuel()
        {
            var randomCard = _characterHand.GetRandomCardAI();
            ChooseCardForDuel(randomCard);
        }

        private void ChooseCardForDuel(Card card)
        {
            card.SetIfCardBelongToCharacter(false);
            _cardDuelHandler.SetCharacterCardForDuel(card);
        }
    }
}
