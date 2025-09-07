using Morhu.Infrustructure;
using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Morhu.UI.MenuHUB
{
    public class MenuSettingView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private Toggle _enToggle;
        [SerializeField] private Toggle _ukToggle;

        private IPersistanceDataService _persistanceDataService;
        private IAudioService _audioService;
        private Game _game;

        public void Init(IAudioService audioService, Game game, IPersistanceDataService persistanceDataService)
        {
            _audioService = audioService;
            _game = game;
            _persistanceDataService = persistanceDataService;

            InitVolumeSlider();

            _volumeSlider.onValueChanged.AddListener((float value) => {
                _audioService.ChangeVolumeForAll(value);
            });

            if (persistanceDataService.Data.Language == Infrustructure.Services.Localization.Language.En)
                SwitchToggle(_enToggle, _ukToggle);
            else
                SwitchToggle(_ukToggle, _enToggle);

            _enToggle.onValueChanged.AddListener(isOn => {
                if (!isOn && !_ukToggle.isOn)
                {
                    _enToggle.isOn = true;
                    return;
                }

                if (isOn)
                {
                    _ukToggle.isOn = false;
                    _game.LocalizationService.ChangeLanguage(Infrustructure.Services.Localization.Language.En);
                    _audioService.BuildSound().WithParent(transform).Play("SelectButton");
                }
            });

            _ukToggle.onValueChanged.AddListener(isOn => {
                if (!isOn && !_enToggle.isOn)
                {
                    _ukToggle.isOn = true;
                    return;
                }

                if (isOn)
                {
                    _enToggle.isOn = false;
                    _game.LocalizationService.ChangeLanguage(Infrustructure.Services.Localization.Language.Uk);
                    _audioService.BuildSound().WithParent(transform).Play("SelectButton");
                }
            });
        }

        private void SwitchToggle(Toggle newToggle, Toggle oldToggle)
        {
            newToggle.isOn = true;
            oldToggle.isOn = false;
        }

        private void InitVolumeSlider()
        {
            _volumeSlider.maxValue = Constants.MAX_VOLUME;
            _volumeSlider.value = _audioService.CurrentVolume;
        }

        private void Clear()
        {
            _volumeSlider.onValueChanged.RemoveAllListeners();
            _enToggle.onValueChanged.RemoveAllListeners();
            _ukToggle.onValueChanged.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}
