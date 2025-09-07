using Morhu.Util;
using System.Collections;
using UnityEngine;

namespace Morhu.Infrustructure.Services.AudioSystem
{
    public class SoundEmitter : MonoBehaviour 
    {
        public SoundData SoundData { get; private set; }

        private AudioService _audioService;
        private AudioSource _audioSource;
        private Coroutine _playingCoroutine;

        private float _soundDataVolume = 1.0f;

        private void Awake() => 
            _audioSource = gameObject.AddOrGet<AudioSource>();

        public void InitEmitter(AudioService soundManager) =>
            _audioService = soundManager;

        public void InitSound(SoundData soundData)
        {
            SoundData = soundData;
            _audioSource.clip = soundData.Clip;
            _audioSource.outputAudioMixerGroup = soundData.Mixer;
            _audioSource.loop = soundData.Loop;
            _audioSource.playOnAwake = soundData.PlayOnAwake;
            _audioSource.mute = soundData.Mute;
            _audioSource.priority = soundData.Priority;
            _audioSource.pitch = soundData.Pitch;
            _audioSource.minDistance = soundData.MinDistance;
            _audioSource.maxDistance = soundData.MaxDistance;
            _audioSource.volume = soundData.Volume;

            _soundDataVolume = soundData.Volume;
        }

        public void ChangeVolume(float volume) => 
            _audioSource.volume = volume * _soundDataVolume;

        public void Play()
        {
            if (_playingCoroutine != null) 
                StopCoroutine(_playingCoroutine);

            _audioSource.Play();

            _playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        public void Stop()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
                _playingCoroutine = null;
            }

            _audioSource.Stop();
            _audioService.ReturnToPool(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f) => 
            _audioSource.pitch = Random.Range(min, max);

        public float GetClipDuration() =>
            _audioSource.clip.length;

        private IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => _audioSource.isPlaying);
            _audioService.ReturnToPool(this);
        }
    }
}
