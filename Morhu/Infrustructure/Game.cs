
using Morhu.Infrustructure.Services.Localization;
using Morhu.Infrustructure.States;
using Morhu.Util;

namespace Morhu.Infrustructure
{
    public class Game
    {
        public GameStateMachine StateMachine { get; private set; }
        public ILocalizationService LocalizationService { get; private set; }

        public Game(LoadingCurtain loadingCurtain, SceneLoader sceneLoader, ICoroutineRunner coroutineRunner,
            ILocalizationService localizationService)
        {
            StateMachine = new GameStateMachine(loadingCurtain, sceneLoader, coroutineRunner, this);
            LocalizationService = localizationService;
        }

        public void Tick() =>
            StateMachine.Update();
    }
}
