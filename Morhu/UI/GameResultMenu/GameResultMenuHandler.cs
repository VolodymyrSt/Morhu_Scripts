using Morhu.Infrustructure;
using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.Services.EventBus;
using Morhu.Infrustructure.Services.EventBus.Signals;
using Morhu.Infrustructure.Services.Localization;
using System;
using UnityEngine;

namespace Morhu.UI.GameResultMenu
{
    public class GameResultMenuHandler : IDisposable
    {
        private readonly GameResultMenuView _view;
        private readonly IEventBus _eventBus;
        private readonly Game _game;
        private readonly IAudioService _audioService;
        private readonly IPersistanceDataService _persistanceDataService;

        public GameResultMenuHandler(GameResultMenuView view, IEventBus eventBus, Game game, IAudioService audioService,
            IPersistanceDataService persistanceDataService)
        {
            _view = view;
            _eventBus = eventBus;
            _game = game;
            _audioService = audioService;
            _persistanceDataService = persistanceDataService;

            _eventBus.SubscribeEvent<OnCharacterDefeatAnimationEnded>(ShowPlayerWonResult);
            _eventBus.SubscribeEvent<OnPlayerDefeatAnimationEnded>(ShowCharacterWonResult);

            _view.Init(_game, audioService);
        }

        public void ShowPlayerWonResult(OnCharacterDefeatAnimationEnded signal)
        {
            if (_persistanceDataService.Data.Language == Language.En)
                _view.ShowResult(ResultLocalizationSprite.YouWonEn);
            else
                _view.ShowResult(ResultLocalizationSprite.YouWonUk);

            _audioService.BuildSound().WithParent(_view.transform).Play("Victory");
        }

        public void ShowCharacterWonResult(OnPlayerDefeatAnimationEnded signal)
        {
            if (_persistanceDataService.Data.Language == Language.En)
                _view.ShowResult(ResultLocalizationSprite.YouLostEn);
            else
                _view.ShowResult(ResultLocalizationSprite.YouLostUk);

            _audioService.BuildSound().WithParent(_view.transform).Play("Lost");
        }

        public void Dispose()
        {
            _eventBus?.UnSubscribeEvent<OnCharacterDefeatAnimationEnded>(() => {
                _view.ShowResult(ResultLocalizationSprite.YouWonEn);
            });

            _eventBus?.UnSubscribeEvent<OnPlayerDefeatAnimationEnded>(() => {
                _view.ShowResult(ResultLocalizationSprite.YouLostEn);
            });
        }
    }
}
