using Assets.Morhu.Logic.VFX;
using DG.Tweening;
using Morhu.Factory;
using Morhu.Infrustructure.AssetManagement;
using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.Services.EventBus;
using Morhu.Infrustructure.Services.Localization;
using Morhu.UI.ToolTip;
using Morhu.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Morhu.Deck
{
    public class DeckFactory : BaseFactory<List<Card>>
    {
        private readonly ToolTipHandler _toolTipHandler;
        private readonly VFXBuilder _vFXBuilder;
        private readonly IEventBus _eventBus;
        private readonly IAudioService _audioService;
        private readonly IPersistanceDataService _persistanceDataService;

        public DeckFactory(IAssetProvider assetProvider, ToolTipHandler toolTipHandler, VFXBuilder vFXBuilder,
            IEventBus eventBus, IAudioService audioService, IPersistanceDataService persistanceDataService) : base(assetProvider)
        {
            _toolTipHandler = toolTipHandler;
            _vFXBuilder = vFXBuilder;
            _eventBus = eventBus;
            _audioService = audioService;
            _persistanceDataService = persistanceDataService;
        }

        public override List<Card> Create(Vector3 at, Transform parent) => 
            ShuffleDeck(at, parent, AssetPath.GetShuffleCardPaths());

        private List<Card> ShuffleDeck(Vector3 at, Transform parent, string[] cardPaths)
        {
            var offset = Constants.CARD_BETWEEN_OFFSET;
            var cards = new List<Card>();

            _audioService.BuildSound().WithParent(parent).Play("StuffeDeck");

            for (int i = 0; i < cardPaths.Length; i++)
            {
                var card = AssetProvider.Instantiate<Card>(cardPaths[i], at + i * offset * Vector3.up, parent);
                card.Init(_toolTipHandler, _vFXBuilder, _eventBus, cardPaths[i], _audioService, _persistanceDataService);
                card.SetInitialRotation(-i / 2);

                card.transform.DOMove(parent.transform.position + i * offset * Vector3.up, 0.4f)
                    .SetEase(Ease.Linear)
                    .Play()
                    .SetDelay(i * 0.022f);

                cards.Add(card);
            }

            return cards;
        }

        public List<Card> CreateShuffleDeck(Vector3 at, Transform parent) => Create(at, parent);
    }
}
