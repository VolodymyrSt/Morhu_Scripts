using Di;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.Services.EventBus;
using Morhu.Infrustructure.Services.EventBus.Signals;
using Morhu.Logic.CameraLogic;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Morhu.Logic.HealthLogic
{
    public class HealthSystem : ConstructedMonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private List<JarHeartHandler> _jarHearts = new();
        [SerializeField] private List<HeartBeatSoundPerformer> _heartBeatSoundPerformers = new();
        [SerializeField] private SwitchedCameraType _attachedCamera;
        [SerializeField] private bool _isPlayarHealth;

        private IEventBus _eventBus;
        private IAudioService _audioService;

        public SwitchedCameraType AttachedCamera => _attachedCamera;
        public bool IsAllHeartsDone => GetCountOfAllAliveHearts() <= 0;

        public override void Construct(IResolvable container)
        {
            _eventBus = container.Resolve<IEventBus>();
            _audioService = container.Resolve<IAudioService>();
            InitHeartSoundPerformers();
        }

        public void InvokeDeathEvent()
        {
            if (_isPlayarHealth)
                _eventBus.Invoke(new OnCharacterWonSignal());
            else
                _eventBus.Invoke(new OnPlayerWonSignal());
        }

        public void KillOneHeart()
        {
            foreach (var heart in _jarHearts)
            {
                if (heart.IsAlive())
                {
                    heart.KillSelf();
                    _audioService.BuildSound().WithParent(heart.transform).Play("KillHeart");
                    break;
                }
            }
        }
        
        public void HealOneHeart()
        {
            for (int i = _jarHearts.Count - 1; i >= 0; i--)
            {
                var heart = _jarHearts[i];
                if (!heart.IsAlive())
                {
                    _audioService.BuildSound().WithParent(heart.transform).Play("HealHeart");
                    heart.HealSelf();
                    break;
                }
            }
        }

        public int GetCountOfAllAliveHearts()
        {
            var count = 0;
            for (int i = 0; i < _jarHearts.Count; i++)
            {
                if (_jarHearts[i].IsAlive())
                    count++;
            }
            return count;
        }
        
        public int GetCountOfAllDiedHearts()
        {
            var count = 0;
            for (int i = 0; i < _jarHearts.Count; i++)
            {
                if (!_jarHearts[i].IsAlive())
                    count++;
            }
            return count;
        }

        private void InitHeartSoundPerformers()
        {
            foreach (var soundPerformer in _heartBeatSoundPerformers)
                soundPerformer.Init(_audioService);
        }
    }
}
