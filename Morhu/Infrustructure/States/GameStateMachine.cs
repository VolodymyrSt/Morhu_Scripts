using Morhu.Util;
using System;
using System.Collections.Generic;

namespace Morhu.Infrustructure.States
{
    public class GameStateMachine 
    {
        private readonly Dictionary<Type, IEnterableState> _states;
        private IEnterableState _currentStates;

        public GameStateMachine(LoadingCurtain loadingCurtain, SceneLoader sceneLoader,
            ICoroutineRunner coroutineRunner, Game game)
        {
            _states = new Dictionary<Type, IEnterableState>()
            {
                [typeof(BootStrapState)] = new BootStrapState(this, sceneLoader),
                [typeof(LoadMenuState)] = new LoadMenuState(this, loadingCurtain, sceneLoader, game),
                [typeof(LoadGameplayState)] = new LoadGameplayState(this, loadingCurtain, sceneLoader, game),
                [typeof(GameLoopState)] = new GameLoopState(this, coroutineRunner)
            };
        }

        public void Update()
        {
            if (_currentStates is ITickableState updatableState)
                updatableState.Tick();
        }

        public void Enter<TState>() where TState : IEnterableState 
        {
            var state = ChangeState<TState>();
            state.Enter();
        }

        private IEnterableState ChangeState<TState>()
        {
            if (_currentStates is IExitableState exitableState)
                exitableState.Exit();

            var state = _states[typeof(TState)];
            _currentStates = state;
            return state;
        }
    }
}
