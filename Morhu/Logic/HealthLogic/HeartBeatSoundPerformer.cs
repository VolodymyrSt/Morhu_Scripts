using DG.Tweening;
using Morhu.Infrustructure.Services.AudioSystem;
using UnityEngine;

namespace Morhu.Logic.HealthLogic
{
    public class HeartBeatSoundPerformer : MonoBehaviour
    {
        private IAudioService _audioService;

        public void Init(IAudioService audioService) => 
            _audioService = audioService;

        //animation event
        public void PlayHeartBeatSound() =>
              _audioService.BuildSound().WithParent(transform).Play("BeatHeart");
    }
}
