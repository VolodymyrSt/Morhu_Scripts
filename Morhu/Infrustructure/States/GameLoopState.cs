using UnityEngine;
using Di;
using Morhu.Deck;
using System.Collections;
using Morhu.Util;
using Morhu.Logic.CameraLogic;
using Morhu.Player;
using Morhu.NPC;
using Morhu.UI.ToolTip;
using Morhu.Items;
using Assets.Morhu.Logic.EnergyLogic;
using Morhu.Infrustructure.AssetManagement;
using Morhu.Infrustructure.Services.EventBus;
using Morhu.Infrustructure.Services.EventBus.Signals;
using Morhu.UI.BlinkSystem;
using Morhu.UI.GameplaySetting;
using Morhu.Infrustructure.Services.AudioSystem;

namespace Morhu.Infrustructure.States
{
    public class GameLoopState : ITickableState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly ICoroutineRunner _coroutineRunner;

        //DECK
        private DeckFactory _deckFactory;
        private DeckHolder _deckHolder;
        private CardHand _playerHand;
        private CardHand _characterHand;
        private CardDuelHandler _cardDuelHandler;

        private PlayerCardSelector _playerSelector;
        private CharacterCardSelector _characterSelector;

        //ITEM
        private ItemPresenter _playerItemPresenter;
        private ItemPresenter _characterItemPresenter;

        private ItemTray _playerItemTray;
        private ItemTray _characterItemTray;

        private CharacterItemSelector _characterItemSelector;
        private PlayerItemSelector _playerItemSelector;

        //OTHER
        private CameraSwitcher _cameraSwitcher;
        private IEventBus _eventBus;
        private IAudioService _audioService;
        private ToolTipHandler _toolTipHandler;

        private EnergySystem _playerEnergySystem;
        private EnergySystem _characterEnergySystem;

        private CharacterHandler _characterHandler;
        private EyeBlinkingSystem _eyeBlinkingSystem;
        private GameplaySettingHandler _gameplaySettingHandler;

        private Coroutine _startGameCoroutine;

        private bool _isPlayerTurn;
        private bool _shouldChangeTurn;
        private bool _isDuelEnded;
        private bool _isGameEnded;
        private bool _isAwakeCutSceneEnded;
        private bool _isCharacterIntroSpeakingEnded;

        private int _countOfPlayedDuels;
        private bool _isItemSelectionStarted;

        public GameLoopState(GameStateMachine gameStateMachine, ICoroutineRunner coroutineRunner)
        {
            _gameStateMachine = gameStateMachine;
            _coroutineRunner = coroutineRunner;
        }

        public void Enter()
        {
            _cameraSwitcher = DI.Container.Resolve<CameraSwitcher>();
            _eventBus = DI.Container.Resolve<IEventBus>();

            _deckFactory = DI.Container.Resolve<DeckFactory>();
            _deckHolder = DI.Container.Resolve<DeckHolder>();

            _playerHand = DI.Container.Resolve<CardHand>("Player");
            _characterHand = DI.Container.Resolve<CardHand>("Character");
            _cardDuelHandler = DI.Container.Resolve<CardDuelHandler>();

            _playerSelector = DI.Container.Resolve<PlayerCardSelector>();
            _characterSelector = DI.Container.Resolve<CharacterCardSelector>();
            _toolTipHandler = DI.Container.Resolve<ToolTipHandler>();

            _playerItemPresenter = DI.Container.Resolve<ItemPresenter>("Player");
            _characterItemPresenter = DI.Container.Resolve<ItemPresenter>("Character");

            _playerItemTray = DI.Container.Resolve<ItemTray>("Player");
            _characterItemTray = DI.Container.Resolve<ItemTray>("Character");

            _playerItemSelector = DI.Container.Resolve<PlayerItemSelector>();
            _characterItemSelector = DI.Container.Resolve<CharacterItemSelector>();

            _playerEnergySystem = DI.Container.Resolve<EnergySystem>("Player");
            _characterEnergySystem = DI.Container.Resolve<EnergySystem>("Character");

            _eyeBlinkingSystem = DI.Container.Resolve<EyeBlinkingSystem>();
            _gameplaySettingHandler = DI.Container.Resolve<GameplaySettingHandler>();
            _audioService = DI.Container.Resolve<IAudioService>();

            _characterHandler = DI.Container.Resolve<CharacterHandler>();

            _eventBus.SubscribeEvent<OnPlayerWonSignal>(StopGame);
            _eventBus.SubscribeEvent<OnCharacterWonSignal>(StopGame);

            InitStats();

            _startGameCoroutine = _coroutineRunner.StartCoroutine(StartGame());
        }

        public void Tick()
        {
            if (_isGameEnded) return;

            if (!_playerItemSelector.IsPlayerUsingItem())
                _playerSelector.Update();

            _toolTipHandler.Tick();
            _playerItemSelector.Update();

            if (_isAwakeCutSceneEnded && _isCharacterIntroSpeakingEnded)
                _gameplaySettingHandler.Update();
        }

        public void Exit()
        {
            AssetPath.ClearUsedCardPaths();
            _eventBus?.UnSubscribeEvent<OnPlayerWonSignal>(StopGame);
            _eventBus?.UnSubscribeEvent<OnCharacterWonSignal>(StopGame);
        }

        private IEnumerator StartGame()
        {
            _audioService.BuildSound().WithParent(_cameraSwitcher.transform).Play("GameMusic");

            yield return _coroutineRunner.StartCoroutine(StartBeginningPart());

            while (true)
            {
                if (!IsAllSelectorsBlocked())
                {
                    yield return _coroutineRunner.StartCoroutine(CheckIfAllCandlesAreBlow());

                    if (IsTimeToSelectItems())
                        yield return _coroutineRunner.StartCoroutine(PerformItemSelection());

                    yield return new WaitUntil(() => !_isItemSelectionStarted);

                    yield return _coroutineRunner.StartCoroutine(PerformDuelCardsSelection());

                    yield return new WaitForSeconds(Constants.DELAY);

                    _cameraSwitcher.SwitchTo(SwitchedCameraType.DuelCamera);
                    yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                    UnblockSelectors();

                    _cardDuelHandler.PerformDuel(_isPlayerTurn, () => {
                        _isDuelEnded = true;
                        _cameraSwitcher.SwitchTo(SwitchedCameraType.PlayerCamera);
                    });

                    _toolTipHandler.ResetToolTipPerformance();
                    yield return new WaitUntil(() => _isDuelEnded);
                    yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                    yield return _coroutineRunner.StartCoroutine(CheckIfDeckIsEmpty());

                    yield return new WaitForSeconds(Constants.DELAY);
                    RestartSetting();
                }
                else
                    UnblockSelectors();
            }
        }

        private bool IsTimeToSelectItems() => 
            _countOfPlayedDuels != 0 && _countOfPlayedDuels % 3 == 0;

        private void CreateShuffleDeckAndSetToDeck()
        {
            var deck = _deckFactory.CreateShuffleDeck(_deckHolder.GetPosition() + Vector3.up, _deckHolder.transform);
            _deckHolder.SetDeck(deck);
            AssetPath.ClearUsedCardPaths();
        }

        private bool IsAllSelectorsBlocked() => 
            _playerSelector.IsBlocked() && _characterSelector.IsBlocked();

        private void UnblockSelectors()
        {
            _playerSelector.Unblock();
            _characterSelector.Unblock();
        }

        private void RestartSetting()
        {
            _isDuelEnded = false;
            _countOfPlayedDuels++;

            if (_shouldChangeTurn)
            {
                _isPlayerTurn = !_isPlayerTurn;
                _shouldChangeTurn = false;
            }
        }

        private void InitStats()
        {
            _isPlayerTurn = true;
            _shouldChangeTurn = false;
            _isDuelEnded = false;
            _isAwakeCutSceneEnded = false;
            _isCharacterIntroSpeakingEnded = false;

            _countOfPlayedDuels = 0;
            _isItemSelectionStarted = false;
            _isGameEnded = false;
        }

        private IEnumerator StartBeginningPart()
        {
            _eyeBlinkingSystem.Init();
            _eyeBlinkingSystem.PerformBlinking();
            _cameraSwitcher.SetPlayerCameraNoise(amplitude: 6f, frequency: 6f);

            _characterHandler.StartSpeaking("Intro_En", "Intro_Uk",() => { 
                _isCharacterIntroSpeakingEnded = true;
            });

            _cameraSwitcher.PerformPlayerAwake(() => {
                _isAwakeCutSceneEnded = true;
            });

            yield return new WaitUntil(() => _isAwakeCutSceneEnded && _isCharacterIntroSpeakingEnded);

            _eyeBlinkingSystem.SetActive(false);
            _cameraSwitcher.SetPlayerCameraNoise(amplitude: 3f, frequency: 3f);

            CreateShuffleDeckAndSetToDeck();

            yield return new WaitForSeconds(Constants.DELAY_FOR_DECK_STUFFLE);
            _deckHolder.GiveCard(_playerHand);
            yield return new WaitForSeconds(Constants.WAIT_TIME_TO_GIVE_CARD);
            _deckHolder.GiveCard(_playerHand);
            yield return new WaitForSeconds(Constants.WAIT_TIME_TO_GIVE_CARD);
            _deckHolder.GiveCard(_characterHand);
            yield return new WaitForSeconds(Constants.WAIT_TIME_TO_GIVE_CARD);
            _deckHolder.GiveCard(_characterHand);
        }

        private IEnumerator CheckIfAllCandlesAreBlow()
        {
            if (_playerEnergySystem.IsAllCandlesAreBlew())
            {
                var isAllCandlesAreIgnited = false;
                _cameraSwitcher.SwitchTo(SwitchedCameraType.PlayerHealthCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                _coroutineRunner.StartCoroutine(_playerEnergySystem.TryToIgniteAllCandles(() => {
                    isAllCandlesAreIgnited = true;
                }));

                yield return new WaitUntil(() => isAllCandlesAreIgnited);

                if (!_characterEnergySystem.IsAllCandlesAreBlew())
                {
                    _cameraSwitcher.SwitchTo(SwitchedCameraType.PlayerCamera);
                    yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);
                }
            }
            if (_characterEnergySystem.IsAllCandlesAreBlew())
            {
                var isAllCandlesAreIgnited = false;
                _cameraSwitcher.SwitchTo(SwitchedCameraType.CharacterHealthCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                _coroutineRunner.StartCoroutine(_characterEnergySystem.TryToIgniteAllCandles(() => {
                    isAllCandlesAreIgnited = true;
                }));

                yield return new WaitUntil(() => isAllCandlesAreIgnited);

                _cameraSwitcher.SwitchTo(SwitchedCameraType.PlayerCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);
            }
        }

        private IEnumerator PerformItemSelection()
        {
            _playerItemSelector.SetIfPlayerCanUseItems(false);

            if (_playerItemTray.CanAddItemToTray() && _characterItemTray.CanAddItemToTray())
            {
                _isItemSelectionStarted = true;
                var isCharacterSelectedItems = false;

                _cameraSwitcher.SwitchTo(SwitchedCameraType.PlayerItemPresenter);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                _playerItemSelector.RestartSelector();
                _playerItemPresenter.OpenAndPresentItems(() => {
                    _playerItemSelector.SetIfPlayerCanSelectItems(true);
                });

                _characterItemPresenter.OpenAndPresentItems(() => {
                    _coroutineRunner.StartCoroutine(_characterItemSelector
                        .TryToSelectTwoItems(() => isCharacterSelectedItems = true));
                });

                yield return new WaitUntil(() => isCharacterSelectedItems && _playerItemSelector.IsPlayerSelectedItems());

                _playerItemPresenter.Close();
                _characterItemPresenter.Close();

                _cameraSwitcher.SwitchTo(SwitchedCameraType.PlayerCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                _playerItemSelector.SetIfPlayerCanSelectItems(false);
                _isItemSelectionStarted = false;
            }
            else if (_playerItemTray.CanAddItemToTray() && !_characterItemTray.CanAddItemToTray())
            {
                _isItemSelectionStarted = true;

                _cameraSwitcher.SwitchTo(SwitchedCameraType.PlayerItemPresenter);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                _playerItemSelector.RestartSelector();
                _playerItemPresenter.OpenAndPresentItems(() => {
                    _playerItemSelector.SetIfPlayerCanSelectItems(true);
                });

                yield return new WaitUntil(() => _playerItemSelector.IsPlayerSelectedItems());
                _playerItemPresenter.Close();

                _cameraSwitcher.SwitchTo(SwitchedCameraType.PlayerCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                _playerItemSelector.SetIfPlayerCanSelectItems(false);
                _isItemSelectionStarted = false;
            }
            else if (_characterItemTray.CanAddItemToTray() && !_playerItemTray.CanAddItemToTray())
            {
                _isItemSelectionStarted = true;
                var isCharacterSelectedItems = false;

                _characterItemPresenter.OpenAndPresentItems(() => {
                    _coroutineRunner.StartCoroutine(_characterItemSelector
                        .TryToSelectTwoItems(() => isCharacterSelectedItems = true));
                });

                yield return new WaitUntil(() => isCharacterSelectedItems);
                _characterItemPresenter.Close();
                _isItemSelectionStarted = false;
            }
        }

        private IEnumerator PerformDuelCardsSelection()
        {
            if (_isPlayerTurn)
            {
                if (!_playerSelector.IsBlocked())
                {
                    _playerSelector.IsCardOnDuel = false;
                    _playerItemSelector.SetIfPlayerCanUseItems(true);
                    yield return new WaitUntil(() => _playerSelector.IsCardOnDuel);
                    _playerItemSelector.SetIfPlayerCanUseItems(false);
                    _toolTipHandler.StopToolTipPerformance();
                    _shouldChangeTurn = true;
                }

                yield return new WaitForSeconds(Constants.DELAY);

                _characterSelector.TryToChooseCardForDuel();
            }
            else
            {
                yield return new WaitForSeconds(Constants.DELAY);

                var isCharacterEndedItemSelection = false;

                if (!_characterSelector.IsBlocked())
                {
                    _characterItemSelector.TryToUseItemAbility(() => isCharacterEndedItemSelection = true);
                    _shouldChangeTurn = true;
                }
                else
                    isCharacterEndedItemSelection = true;

                yield return new WaitUntil(() => isCharacterEndedItemSelection);
                yield return new WaitForSeconds(Constants.ANIMATION_DURATION);

                _characterSelector.TryToChooseCardForDuel();

                if (!_playerSelector.IsBlocked())
                {
                    yield return new WaitForSeconds(Constants.DELAY);
                    _playerSelector.IsCardOnDuel = false;
                    yield return new WaitUntil(() => _playerSelector.IsCardOnDuel);
                    _toolTipHandler.StopToolTipPerformance();
                }
            }
        }

        private IEnumerator CheckIfDeckIsEmpty()
        {
            if (_deckHolder.IsEmpty())
            {
                CreateShuffleDeckAndSetToDeck();

                yield return new WaitForSeconds(Constants.DELAY_FOR_DECK_STUFFLE);
                _deckHolder.GiveCard(_playerHand);
                yield return new WaitForSeconds(Constants.WAIT_TIME_TO_GIVE_CARD);
                _deckHolder.GiveCard(_characterHand);
            }
            else
            {
                _deckHolder.GiveCard(_playerHand);
                yield return new WaitForSeconds(Constants.WAIT_TIME_TO_GIVE_CARD);

                if (_deckHolder.IsEmpty())
                {
                    CreateShuffleDeckAndSetToDeck();

                    yield return new WaitForSeconds(Constants.DELAY_FOR_DECK_STUFFLE);
                    _deckHolder.GiveCard(_characterHand);
                }
                else
                    _deckHolder.GiveCard(_characterHand);
            }
        }

        private void StopGame(OnPlayerWonSignal signal)
        {
            _isGameEnded = true;

            if (_startGameCoroutine != null)
                _coroutineRunner.StopCoroutine(_startGameCoroutine);
        }
        
        private void StopGame(OnCharacterWonSignal signal)
        {
            _isGameEnded = true;

            if (_startGameCoroutine != null)
                _coroutineRunner.StopCoroutine(_startGameCoroutine);
        }
    }
}
