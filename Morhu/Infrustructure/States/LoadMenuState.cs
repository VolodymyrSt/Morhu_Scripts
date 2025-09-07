using Morhu.Infrustructure.Installers;
using UnityEngine;

namespace Morhu.Infrustructure.States
{
    public class LoadMenuState : IEnterableState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly SceneLoader _sceneLoader;
        private readonly Game _game;

        public LoadMenuState(GameStateMachine gameStateMachine, LoadingCurtain loadingCurtain,
            SceneLoader sceneLoader, Game game)
        {
            _gameStateMachine = gameStateMachine;
            _loadingCurtain = loadingCurtain;
            _sceneLoader = sceneLoader;
            _game = game;
        }

        public void Enter()
        {
            _loadingCurtain.Procced(); 
            _sceneLoader.Load(SceneName.MenuScene, InitMenuScene);
        }

        public void InitMenuScene()
        {
            BindDependencies();
            _game.LocalizationService.FindAllObjectWithLanguageIDInTheScene();
            _game.LocalizationService.UpdateLanguageToCurrent();
        }

        private static void BindDependencies()
        {
            var menuInstaller = Object.FindAnyObjectByType<MenuInstaller>();
            menuInstaller.PerformBinding();
        }
    }
}
