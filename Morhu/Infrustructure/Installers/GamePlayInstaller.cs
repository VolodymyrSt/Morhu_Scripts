using Di;
using Morhu.NPC;
using Morhu.Deck;
using Morhu.Infrustructure.AssetManagement;
using Morhu.Logic.CameraLogic;
using Morhu.Logic.HealthLogic;
using Morhu.Player;
using UnityEngine;
using Morhu.Infrustructure.Services.EventBus;
using Morhu.UI.ToolTip;
using Assets.Morhu.Logic.VFX;
using Morhu.Items;
using Assets.Morhu.Logic.EnergyLogic;
using Morhu.UI.GameResultMenu;
using Morhu.UI.BlinkSystem;
using Morhu.UI.GameplaySetting;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.Services.AudioSystem.ScriptableObjects;
using Morhu.Infrustructure.Services.Localization;
using Morhu.Infrustructure.Data;

namespace Morhu.Infrustructure.Installers
{
    public class GamePlayInstaller : LocalInstaller
    {
        [Header("Camera")]
        [SerializeField] private CameraSwitcher _camera;

        [Header("Deck")]
        [SerializeField] private DeckHolder _deckHolder;
        [SerializeField] private CardHand _playerHand;
        [SerializeField] private CardHand _characterHand;
        [SerializeField] private CardDuelHandler _cardDuelHandler;

        [Header("Health")]
        [SerializeField] private HealthSystem _playerHealthSystem;
        [SerializeField] private HealthSystem _characterHealthSystem;

        [Header("Energy")]
        [SerializeField] private EnergySystem _playerEnergySystem;
        [SerializeField] private EnergySystem _characterEnergySystem;

        [Header("Items")]
        [SerializeField] private ItemPresenter _playerItemPresenter;
        [SerializeField] private ItemPresenter _characterItemPresenter;

        [SerializeField] private ItemTray _playerItemTray;
        [SerializeField] private ItemTray _characterItemTray;

        [Header("UI")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private ToolTipView _toolTipView;
        [SerializeField] private GameResultMenuView _resultMenuView;
        [SerializeField] private GameplaySettingView _gameplaySettingView;

        [Header("Sounds")]
        [SerializeField] private CompositionsHolderSO _compositionsHolderSO;

        [Header("Other")]
        [SerializeField] private EyeBlinkingSystem _eyeBlinkingSystem;
        [SerializeField] private CharacterHandler _characterHandler;

        protected override void InstallBindings()
        {
            BindCamera();
            BindEventBus();
            BindVFXBuilder();
            BindAudioSystem();
            BindToolTip();
            BindCharacter();
            BindDeck();
            BindHands();
            BindHealthSystems();
            BindEnergySystems();
            BindCardDuelHandler();
            BindItems();
            BindCardSelectors();
            BindGameResultMenuHandler();
            BindEyeBlinkingSystem();
            BindGameplaySetting();
        }

        private void BindCharacter() => 
            Container.BindInstance(_characterHandler).NeedToBeConstructed();

        private void BindAudioSystem()
        {
            Container.Bind<IAudioService>(c => new AudioService(_compositionsHolderSO
                , c.Resolve<AssetProvider>(), c.Resolve<IPersistanceDataService>())).AsSingle();
        }

        private void BindGameplaySetting()
        {
            Container.BindInstance(_gameplaySettingView).NeedToBeConstructed();
            Container.Bind(c => new GameplaySettingHandler(c.Resolve<GameplaySettingView>(),
                c.Resolve<IAudioService>(), c.Resolve<Game>())).AsSingle().NotLazy();
        }

        private void BindGameResultMenuHandler() => 
            Container.Bind(c => new GameResultMenuHandler(_resultMenuView,c.Resolve<IEventBus>(),
                c.Resolve<Game>(), c.Resolve<IAudioService>(), c.Resolve<IPersistanceDataService>())).AsSingle().NotLazy();

        private void BindEyeBlinkingSystem() =>
            Container.BindInstance(_eyeBlinkingSystem).NeedToBeConstructed();

        private void BindCardDuelHandler() => 
            Container.BindInstance<CardDuelHandler>(_cardDuelHandler).NeedToBeConstructed();

        private void BindEnergySystems()
        {
            Container.BindInstance<EnergySystem>(_playerEnergySystem, "Player");
            Container.BindInstance<EnergySystem>(_characterEnergySystem, "Character");

            _playerEnergySystem.Init(Container.Resolve<HealthSystem>("Player"), Container.Resolve<IAudioService>());
            _characterEnergySystem.Init(Container.Resolve<HealthSystem>("Character"), Container.Resolve<IAudioService>());
        }

        private void BindHealthSystems()
        {
            Container.BindInstance<HealthSystem>(_playerHealthSystem, "Player").NeedToBeConstructed();
            Container.BindInstance<HealthSystem>(_characterHealthSystem, "Character").NeedToBeConstructed();
        }

        private void BindHands()
        {
            Container.BindInstance<CardHand>(_playerHand, "Player");
            Container.BindInstance<CardHand>(_characterHand, "Character");
        }

        private void BindDeck()
        {
            Container.Bind(c => new DeckFactory(c.Resolve<AssetProvider>(), c.Resolve<ToolTipHandler>()
                , c.Resolve<VFXBuilder>(), c.Resolve<IEventBus>(), c.Resolve<IAudioService>()
                , c.Resolve<IPersistanceDataService>())).AsSingle();
            Container.Bind(c => new CardFactory(c.Resolve<AssetProvider>(), c.Resolve<ToolTipHandler>()
                , c.Resolve<VFXBuilder>(), c.Resolve<IEventBus>(), c.Resolve<IAudioService>()
                , c.Resolve<IPersistanceDataService>())).AsSingle();

            Container.BindInstance<DeckHolder>(_deckHolder).NeedToBeConstructed();
        }

        private void BindVFXBuilder() => 
            Container.Bind(c => new VFXBuilder(c.Resolve<AssetProvider>(), c.Resolve<IAudioService>())).AsSingle();

        private void BindEventBus() => 
            Container.Bind<IEventBus>(c => new EventBus()).AsSingle();

        private void BindItems()
        {
            Container.Bind(c => new ItemFactory(c.Resolve<AssetProvider>())).AsSingle();

            Container.BindInstance<ItemPresenter>(_playerItemPresenter, "Player").NeedToBeConstructed();
            Container.BindInstance<ItemPresenter>(_characterItemPresenter, "Character").NeedToBeConstructed();

            Container.BindInstance<ItemTray>(_playerItemTray, "Player");
            Container.BindInstance<ItemTray>(_characterItemTray, "Character");

            Container.Bind(c => new PlayerItemSelector(Container.Resolve<Camera>()
                , Container.Resolve<ItemTray>("Player"), Container.Resolve<ItemPresenter>("Player"),
                Container.Resolve<EnergySystem>("Player"), c.Resolve<AssetProvider>(), _playerHand, _characterHand
                , Container.Resolve<CardFactory>())).AsSingle();

            Container.Bind(c => new CharacterItemSelector(Container.Resolve<Camera>(), Container.Resolve<ItemTray>("Character"), DI.Container.Resolve<ItemPresenter>("Character")
                , Container.Resolve<EnergySystem>("Character"), c.Resolve<AssetProvider>(), _characterHand, _playerHand
                , Container.Resolve<CardFactory>())).AsSingle();
        }

        private void BindToolTip()
        {
            Container.BindInstance<ToolTipView>(_toolTipView);
            Container.BindInstance<Canvas>(_canvas);
            var toolTipHandler = new ToolTipHandler(Container.Resolve<ToolTipView>(), Container.Resolve<Canvas>());
            Container.BindInstance<ToolTipHandler>(toolTipHandler);
        }

        private void BindCardSelectors()
        {
            var playerSelector = new PlayerCardSelector(Container.Resolve<Camera>()
                , Container.Resolve<CardDuelHandler>(), _playerHand);
            Container.BindInstance<PlayerCardSelector>(playerSelector);

            var characterSelector = new CharacterCardSelector(Container.Resolve<CardHand>("Character")
                , Container.Resolve<CardDuelHandler>(), _characterHealthSystem, _playerHealthSystem, 
                Container.Resolve<PlayerCardSelector>(), Container.Resolve<CharacterHandler>());
            Container.BindInstance<CharacterCardSelector>(characterSelector);
        }

        private void BindCamera()
        {
            Container.BindInstance<CameraSwitcher>(_camera).NeedToBeConstructed();
            Container.BindInstance<Camera>(_camera.GetComponentInChildren<Camera>());
        }
    }
}
