using UnityEngine;
using Morhu.Infrustructure.Installers;

namespace Morhu.Infrustructure.States
{
    public class LoadGameplayState : IEnterableState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly SceneLoader _sceneLoader;
        private readonly Game _game;

        public LoadGameplayState(GameStateMachine gameStateMachine, LoadingCurtain loadingCurtain
            , SceneLoader sceneLoader, Game game)
        {
            _gameStateMachine = gameStateMachine;
            _loadingCurtain = loadingCurtain;
            _sceneLoader = sceneLoader;
            _game = game;
        }

        public void Enter()
        {
            _loadingCurtain.Procced();
            _sceneLoader.Load(SceneName.GameplayScene, () =>
                {
                    BindDependencies();
                    _gameStateMachine.Enter<GameLoopState>();
                    _game.LocalizationService.FindAllObjectWithLanguageIDInTheScene();
                    _game.LocalizationService.UpdateLanguageToCurrent();
                });
        }

        private static void BindDependencies()
        {
            var gamePlayInstaller = Object.FindAnyObjectByType<GamePlayInstaller>();
            gamePlayInstaller.PerformBinding();
        }
    }
}
