using Morhu.Infrustructure.Installers;
using UnityEngine;

namespace Morhu.Infrustructure.States
{
    public class BootStrapState : IEnterableState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;

        public BootStrapState(GameStateMachine gameStateMachine, SceneLoader sceneLoader)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter() => 
            _sceneLoader.Load(SceneName.BootStrapperScene, () =>
                    _gameStateMachine.Enter<LoadMenuState>());
    }
}
