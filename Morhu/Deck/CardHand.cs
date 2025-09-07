using DG.Tweening;
using Morhu.Items;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Morhu.Deck
{
    public class CardHand : MonoBehaviour
    {
        [Header("CardMovePaths")]
        [SerializeField] private Transform[] _cardMovePaths;

        [Header("Selection")]
        [SerializeField] private bool _canCardsBeSelectedByPlayer = false;
        [SerializeField] private bool _isCharacterHand = false;

        [Header("Layout Settings")]
        [SerializeField] private float _maxHandWidth = 5f;
        [SerializeField] private float _cardSpacing = 0.5f; 
        [SerializeField] private float _cardTilt = 300f;   
        [SerializeField] private Vector3 _positionOffset = new Vector3(30f, 0f, 0f);   
        [SerializeField] private float _fanAngle = 20f;  
        [SerializeField] private float _heightCurve = 0.3f; 
        [SerializeField] private float _startPositionOffset = 0.1f; 

        [Header("Animation Settings")]
        [SerializeField] private float _cardMoveDuration = 0.3f;
        [SerializeField] private Ease _moveEase = Ease.OutQuad;

        private readonly List<Card> _cards = new();
        private Card _forcedCardToPlay = null;
        private bool _isForcedToPlayCard = false;

        public bool CanTakeCard => _cards.Count < 2;

        public Vector3[] GetPaths()
        {
            Vector3[] paths = new Vector3[_cardMovePaths.Length];
            for (int i = 0; i < _cardMovePaths.Length; i++)
                paths[i] = _cardMovePaths[i].position;

            return paths;
        }

        public void AddCard(Card card)
        {
            _cards.Add(card);
            card.transform.SetParent(transform, false);
            card.SetIfCardCanBeSelectedByPlayer(_canCardsBeSelectedByPlayer);
        }

        public void ArrangeCards()
        {
            if (_cards.Count == 0)
                return;

            var totalWidth = Mathf.Min(_maxHandWidth, _cardSpacing * (_cards.Count - 1));
            float startX = -totalWidth / 2f + _startPositionOffset;

            for (int i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];

                var normalizedPos = (_cards.Count > 1) ? (float)i / (_cards.Count - 1) : 0.5f;
                float xPos = Mathf.Lerp(startX, startX + totalWidth, normalizedPos);

                float zOffset = -Mathf.Pow(normalizedPos * 2 - 1, 2) * _heightCurve;
                float yRotation = Mathf.Lerp(-_fanAngle / 2, _fanAngle / 2, normalizedPos);

                Vector3 targetPosition = new(xPos, i * _positionOffset.y, zOffset + i * _positionOffset.z);
                Quaternion targetRotation = Quaternion.Euler(_cardTilt, yRotation, 0f);

                card.transform.DOKill();
                card.transform.DOLocalMove(targetPosition, _cardMoveDuration)
                    .SetEase(_moveEase)
                    .Play();

                card.transform.DOLocalRotateQuaternion(targetRotation, _cardMoveDuration)
                    .SetEase(_moveEase)
                    .Play();
            }
        }

        //AI
        public bool TryToGetCardByType(CardType type, out Card searchCard)
        {
            searchCard = null;
            if (_cards.Count == 0 && _cards == null) return false;

            foreach (var card in _cards)
            {
                if (card.CardType == type)
                {
                    searchCard = card;
                    return true;
                }
            }

            return false;
        }
        
        //AI
        public bool IsCardWithTypeFound(CardType type)
        {
            if (_cards.Count == 0 && _cards == null) return false;

            foreach (var card in _cards)
            {
                if (card.CardType == type)
                    return true;
            }
            return false;
        }

        //AI
        public Card GetRandomCardAI()
        {
            if (_cards.Count == 0)
                return null;

            var random = Random.Range(0, _cards.Count);
            return _cards[random];
        }

        //AI
        public bool AreAllCardHaveSameType()
        {
            if (_cards.Count == 0) return false;

            for (int i = 0; i < _cards.Count;)
            {
                var firstCard = _cards[i];
                var secondCard = _cards[i + 1];

                if (firstCard != null && secondCard != null)
                {
                    if (_cards[i].CardType == _cards[i + 1].CardType)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            return false;
        }

        public Card GetRandomCard()
        {
            if (_cards.Count == 0)
                return null;

            var random = Random.Range(0, _cards.Count);
            return _cards[random];
        }

        public void RemoveCard(Card card)
        {
            if (_cards.Remove(card))
                ArrangeCards();
        }

        public void SetUpForcedCardToPlay(Card card)
        {
            SetIfForcedToPlayCard(true);
            _forcedCardToPlay = card;
        }

        public Card GetForcedCard() => _forcedCardToPlay;
        public bool IsForcedToPlayCard() => _isForcedToPlayCard;
        public void SetIfForcedToPlayCard(bool value) => _isForcedToPlayCard = value;

        public bool IsCharacterHand() => _isCharacterHand;
    }
}
