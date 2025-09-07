using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.AudioSystem.ScriptableObjects;
using UnityEngine;

namespace Morhu.Infrustructure.Services.AudioSystem
{
    public class SoundBuilder
    {
        private readonly CompositionsHolderSO _compositionsHolder;
        private readonly AudioService _audioService;
        private readonly IPersistanceDataService _persistanceDataService;

        private Transform _parent = null;
        private Vector3 _position = Vector3.zero;

        private bool _randomPitch;

        public SoundBuilder(AudioService audioService, CompositionsHolderSO audioClipHolder, IPersistanceDataService persistanceDataService)
        {
            _audioService = audioService;
            _compositionsHolder = audioClipHolder;
            _persistanceDataService = persistanceDataService;
        }
        
        public SoundBuilder WithPosition(Vector3 position)
        {
            _position = position;
            return this;
        }
        
        public SoundBuilder WithParent(Transform parent)
        {
            _parent = parent;
            return this;
        }
        
        public SoundBuilder WithRandomPitch()
        {
            _randomPitch = true;
            return this;
        }

        public void Play(string clipId, out float clipDuration) => 
            clipDuration = PerformSound(clipId);

        public void Play(string clipId) => 
            PerformSound(clipId);

        private float PerformSound(string clipId)
        {
            var soundData = CreateSoundData(clipId);

            if (soundData == null)
            {
                Debug.LogError("SoundData is null");
                return 0f;
            }

            if (!_audioService.CanPlaySound(soundData)) return 0f;

            var soundEmitter = _audioService.Get();
            soundEmitter.InitSound(soundData);
            soundEmitter.ChangeVolume(_persistanceDataService.Data.GameVolume);
            soundEmitter.transform.position = _position;

            if (_parent != null)
                soundEmitter.transform.SetParent(_parent, false);

            if (_randomPitch)
                soundEmitter.WithRandomPitch();

            if (soundData.FrequentSound)
                _audioService.FrequentSoundEmitters.Enqueue(soundEmitter);

            soundEmitter.Play();
            return soundEmitter.GetClipDuration();
        }

        private SoundData CreateSoundData(string clipId)
        {
            var data = _compositionsHolder.GetDataById(clipId);
            var soundData = new SoundData(
                data.Clip, data.Mixer, data.Loop,
                data.PlayOnAwake, data.FrequentSound,
                data.Mute, data.Priority, data.Volume,
                data.Pitch, data.MinDistance, data.MaxDistance);
            return soundData;
        }
    }
}
