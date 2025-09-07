using DG.Tweening;
using Di;
using Morhu.Infrustructure;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.States;
using Morhu.UI.MenuHUB;
using Morhu.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Morhu.UI.GameplaySetting
{
    public class GameplaySettingView : MonoBehaviour
    {
        private const float HIDDEN_Y_POSITION = 250f;
        private const float SHOWN_Y_POSITION = -20f;

        [Header("UI")]
        [SerializeField] private RectTransform _root;
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private Button _goToMenuButton;

        private IAudioService _audioService;
        private Game _game;

        public void Init(IAudioService audioService, Game game)
        {
            _audioService = audioService;
            _game = game;

            InitSlider();
            Hide();

            _goToMenuButton.GetComponent<ButtonAnimator>().Init(audioService);

            _volumeSlider.onValueChanged.AddListener((float value) => {
                _audioService.ChangeVolumeForAll(value);
            });

            _goToMenuButton.onClick.AddListener(() => {
                _game.StateMachine.Enter<LoadMenuState>();
                _audioService.BuildSound().WithParent(transform).Play("ClickButton");
            });
        }

        public void Show()
        {
            _root.DOAnchorPosY(SHOWN_Y_POSITION, Constants.ANIMATION_DURATION)
                .SetEase(Ease.Linear)
                .Play();
        }

        public void Hide()
        {
            _root.DOAnchorPosY(HIDDEN_Y_POSITION, Constants.ANIMATION_DURATION)
                .SetEase(Ease.Linear)
                .Play();
        }

        private void InitSlider()
        {
            _volumeSlider.maxValue = Constants.MAX_VOLUME;
            _volumeSlider.value = _audioService.CurrentVolume;
        }

        private void OnDestroy()
        {
            _volumeSlider.onValueChanged.RemoveListener((float value) => {
                _audioService.ChangeVolumeForAll(value);
            });

            _goToMenuButton.onClick.RemoveListener(() =>
                _game.StateMachine.Enter<LoadMenuState>());
        }
    }
}
