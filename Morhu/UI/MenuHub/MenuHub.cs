using Di;
using Morhu.Infrustructure;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.States;
using Morhu.UI.GameplaySetting;
using Morhu.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Morhu.UI.MenuHUB
{
    public class MenuHub : ConstructedMonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _cardsLiblaryButton;
        [SerializeField] private Button _gameRuleButton;
        [SerializeField] private Button _backToFirstLevelContainerFromSecondButton;
        [SerializeField] private Button _backToFirstLevelContainerFromThirdButton;
        [SerializeField] private Button _backToFirstLevelContainerFromForthButton;
        [SerializeField] private List<ButtonAnimator>  _menuButtonAnimators = new();

        [Header("Containers")]
        [SerializeField] private RectTransform _firstLevelContainer;
        [SerializeField] private RectTransform _secondLevelContainer;
        [SerializeField] private RectTransform _thirdLevelContainer;
        [SerializeField] private RectTransform _forthLevelContainer;

        private Game _game;
        private IAudioService _audioService;

        public override void Construct(IResolvable container)
        {
            _game = container.Resolve<Game>();
            _audioService = container.Resolve<IAudioService>();
        }

        private void Awake()
        {
            InitButtons();
            EnableAllContainers();
        }

        private void Start()
        {
            InitButtonAnimators();
            PlayBackgroundMusic();
            DisableAllContainers();
        }

        private void PlayBackgroundMusic() => 
            _audioService.BuildSound().WithParent(transform).Play("GameMusic");

        private void InitButtonAnimators()
        {
            foreach (var animator in _menuButtonAnimators)
                animator.Init(_audioService);
        }

        private void InitButtons()
        {
            _playButton.onClick.AddListener(() => {
                _game.StateMachine.Enter<LoadGameplayState>();
                _audioService.BuildSound().WithParent(transform).Play("ClickButton");
            });
            _exitButton.onClick.AddListener(() => {
                Application.Quit();
                _audioService.BuildSound().WithParent(transform).Play("ClickButton");
            });
            _settingButton.onClick.AddListener(() => {
                GoToContainer(_secondLevelContainer);
                _audioService.BuildSound().WithParent(transform).Play("ClickButton");
            });
            _cardsLiblaryButton.onClick.AddListener(() => {
                GoToContainer(_thirdLevelContainer);
                _audioService.BuildSound().WithParent(transform).Play("ClickButton");
            });
            _gameRuleButton.onClick.AddListener(() => {
                GoToContainer(_forthLevelContainer);
                _audioService.BuildSound().WithParent(transform).Play("ClickButton");
            });

            _backToFirstLevelContainerFromSecondButton.onClick.AddListener(() =>
            {
                GoBackToFirstLevelContainerFrom(_secondLevelContainer);
                _audioService.BuildSound().WithParent(transform).Play("ClickButton");
            });
            _backToFirstLevelContainerFromThirdButton.onClick.AddListener(() => {
                GoBackToFirstLevelContainerFrom(_thirdLevelContainer);
                _audioService.BuildSound().WithParent(transform).Play("ClickButton");
            });
            _backToFirstLevelContainerFromForthButton.onClick.AddListener(() => {
                GoBackToFirstLevelContainerFrom(_forthLevelContainer);
                _audioService.BuildSound().WithParent(transform).Play("ClickButton");
            });
        }

        private void GoToContainer(RectTransform toContainer)
        {
            _firstLevelContainer.SetActive(false);
            toContainer.SetActive(true);
        }

        private void GoBackToFirstLevelContainerFrom(RectTransform activeContainer)
        {
            activeContainer.SetActive(false);
            _firstLevelContainer.SetActive(true);
        }

        private void EnableAllContainers()
        {
            _firstLevelContainer.SetActive(true);
            _secondLevelContainer.SetActive(true);
            _thirdLevelContainer.SetActive(true);
            _forthLevelContainer.SetActive(true);
        }

        private void DisableAllContainers()
        {
            _secondLevelContainer.SetActive(false);
            _thirdLevelContainer.SetActive(false);
            _forthLevelContainer.SetActive(false);
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
            _settingButton.onClick.RemoveAllListeners();
            _cardsLiblaryButton.onClick.RemoveAllListeners();
            _gameRuleButton.onClick.RemoveAllListeners();
            _backToFirstLevelContainerFromSecondButton.onClick.RemoveAllListeners();
            _backToFirstLevelContainerFromThirdButton.onClick.RemoveAllListeners();
            _backToFirstLevelContainerFromForthButton.onClick.RemoveAllListeners();
        }
    }
}
