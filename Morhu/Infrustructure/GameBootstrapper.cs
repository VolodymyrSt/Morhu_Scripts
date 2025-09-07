using Di;
using Morhu.Infrustructure.AssetManagement;
using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.Localization;
using Morhu.Infrustructure.States;
using Morhu.Util;
using UnityEngine;

namespace Morhu.Infrustructure
{
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private LoadingCurtain _loadingCurtainPrefab;

        private Game _game;
        private ILocalizationService _localizationService;
        private IPersistanceDataService _persistanceDataService;

        private void Awake()
        {
            var loadingCurtain = Instantiate(_loadingCurtainPrefab, transform);
            var sceneLoader = new SceneLoader(this);

            InitDataPersistanceService();
            InitLocalizationService();
            InitGame(loadingCurtain, sceneLoader);

            BindDependencies();

            _game.StateMachine.Enter<BootStrapState>();
            Application.quitting += SaveData;
            DontDestroyOnLoad(gameObject);
        }

        private void Update() => _game.Tick();

        private void OnDestroy() => 
            Application.quitting -= SaveData;

        private void BindDependencies()
        {
            DI.InitMainContainer();
            DI.MainContainer.Bind(c => new AssetProvider()).AsSingle();
            DI.MainContainer.BindInstance<IPersistanceDataService>(_persistanceDataService);
            DI.MainContainer.BindInstance<ILocalizationService>(_localizationService);
            DI.MainContainer.BindInstance<Game>(_game);
        }

        private void InitGame(LoadingCurtain loadingCurtain, SceneLoader sceneLoader) => 
            _game = new Game(loadingCurtain, sceneLoader, this, _localizationService);

        private void InitDataPersistanceService()
        {
            _persistanceDataService = new PersistanceDataService();
            _persistanceDataService.Load();
        }

        private void InitLocalizationService() => 
            _localizationService = new LocalizationService(_persistanceDataService);

        private void SaveData() => 
            _persistanceDataService.Save();
    }
}
