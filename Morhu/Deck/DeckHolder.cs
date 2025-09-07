using Di;
using Morhu.Infrustructure.Services.AudioSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Morhu.Deck
{
    public class DeckHolder : ConstructedMonoBehaviour
    {
        private IAudioService _audioService;
        private List<Card> _cards;

        public override void Construct(IResolvable container) => 
            _audioService = container.Resolve<IAudioService>();

        public void SetDeck(List<Card> cards) => 
            _cards = new List<Card>(cards);

        public Vector3 GetPosition() =>
            transform.position;

        public void GiveCard(CardHand cardHand)
        {
            if (_cards.Count == 0) return;

            if (cardHand.CanTakeCard)
            {
                var index = _cards.Count - 1;
                var card = _cards[index];
                card.MoveToHand(cardHand);
                card.SetIfCardBelongToCharacter(cardHand.IsCharacterHand());
                _cards.RemoveAt(index);
                _audioService.BuildSound().WithParent(transform).Play("GiveCard");
            }     
        }

        public bool IsEmpty() =>
            _cards.Count == 0;
    }
}
