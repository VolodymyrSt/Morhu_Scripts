using DG.Tweening;
using Di;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.Services.EventBus;
using Morhu.Infrustructure.Services.EventBus.Signals;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Morhu.Logic.CameraLogic
{
    public enum SwitchedCameraType {
        PlayerCamera, PlayerHealthCamera, CharacterHealthCamera, DuelCamera,
        PlayerItemPresenter
    }

    public class CameraSwitcher : ConstructedMonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private CinemachineCamera _playerCamera;
        [SerializeField] private CinemachineCamera _playerHealthCamera;
        [SerializeField] private CinemachineCamera _characterHealthCamera;
        [SerializeField] private CinemachineCamera _duelCamera;
        [SerializeField] private CinemachineCamera _playerItemPresenterCamera;

        private IEventBus _eventBus;

        public override void Construct(IResolvable container) => 
            _eventBus = container.Resolve<IEventBus>();

        private void Start()
        {
            _eventBus.SubscribeEvent<OnCharacterWonSignal>(PerformPlayerDefeat);

            _eventBus.SubscribeEvent<OnPlayerWonSignal>((OnPlayerWonSignal signal) => 
                SwitchTo(SwitchedCameraType.PlayerCamera));
        }

        public void SwitchTo(SwitchedCameraType cameraType)
        {
            switch (cameraType)
            {
                case SwitchedCameraType.PlayerCamera:
                    SetPriority(_playerCamera);
                    break;
                case SwitchedCameraType.PlayerHealthCamera:
                    SetPriority(_playerHealthCamera);
                    break;
                case SwitchedCameraType.CharacterHealthCamera:
                    SetPriority(_characterHealthCamera);
                    break;
                case SwitchedCameraType.DuelCamera:
                    SetPriority(_duelCamera);
                    break;
                case SwitchedCameraType.PlayerItemPresenter:
                    SetPriority(_playerItemPresenterCamera);
                    break;

                default:
                    throw new Exception($"Cant switch to camera with type {cameraType}");
            }
        }

        public void PerformPlayerAwake(Action onComplete)
        {
            SwitchTo(SwitchedCameraType.PlayerCamera);
            _playerCamera.transform.DORotate(new Vector3(25, 0, 0), 14f)
                .SetEase(Ease.Linear)
                .Play()
                .OnComplete(() => onComplete?.Invoke());
        }


        public void SetPlayerCameraNoise(float amplitude, float frequency)
        {
            if (_playerCamera.TryGetComponent(out CinemachineBasicMultiChannelPerlin noise))
            {
                noise.AmplitudeGain = amplitude;
                noise.FrequencyGain = frequency;
            }
        }

        private void PerformPlayerDefeat(OnCharacterWonSignal signal)
        {
            SwitchTo(SwitchedCameraType.PlayerCamera);
            SetPlayerCameraNoise(6f, 6f);
            _playerCamera.transform.DORotate(new Vector3(50, 0, 0), 8f)
                .SetEase(Ease.Linear)
                .Play()
                .SetDelay(1f);
        }

        private void SetPriority(CinemachineCamera targetCamera)
        {
            _playerCamera.Priority = 0;
            _playerHealthCamera.Priority = 0;
            _characterHealthCamera.Priority = 0;
            _duelCamera.Priority = 0;
            _playerItemPresenterCamera.Priority = 0;
            targetCamera.Priority = 1;
        }

        private void OnDestroy()
        {
            _eventBus?.UnSubscribeEvent<OnCharacterWonSignal>(PerformPlayerDefeat);

            _eventBus?.UnSubscribeEvent<OnPlayerWonSignal>((OnPlayerWonSignal signal) =>
                SwitchTo(SwitchedCameraType.PlayerCamera));
        }
    }
}
