using Assets.Morhu.Logic.VFX;
using Morhu.Factory;
using Morhu.Infrustructure.AssetManagement;
using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.Services.EventBus;
using Morhu.Infrustructure.Services.Localization;
using Morhu.UI.ToolTip;
using UnityEngine;

namespace Morhu.Deck
{
    public class CardFactory : BaseFactory<Card>
    {
        private readonly ToolTipHandler _toolTipHandler;
        private readonly VFXBuilder _vFXBuilder;
        private readonly IEventBus _eventBus;
        private readonly IAudioService _audioService;
        private readonly IPersistanceDataService _persistanceDataService;

        public CardFactory(IAssetProvider assetProvider, ToolTipHandler toolTipHandler,
            VFXBuilder vFXBuilder, IEventBus eventBus, IAudioService audioService, IPersistanceDataService persistanceDataService) : base(assetProvider)
        {
            _toolTipHandler = toolTipHandler;
            _vFXBuilder = vFXBuilder;
            _eventBus = eventBus;
            _audioService = audioService;
            _persistanceDataService = persistanceDataService;
        }

        public override Card Create(Vector3 at, Transform parent)
        {
            var path = AssetPath.GetRandomUsedCardPath();

            var card = AssetProvider.Instantiate<Card>(path, at, parent);
            card.Init(_toolTipHandler, _vFXBuilder, _eventBus, path, _audioService, _persistanceDataService);
            return card;
        }

        public Card CreateRandomUsedCard(Vector3 at, Transform parent) => 
            Create(at, parent);
    }
}
