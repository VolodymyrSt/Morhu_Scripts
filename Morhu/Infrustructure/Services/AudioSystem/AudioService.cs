using Morhu.Infrustructure.AssetManagement;
using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.AudioSystem.ScriptableObjects;
using Morhu.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Morhu.Infrustructure.Services.AudioSystem
{
    public class AudioService : IAudioService
    {
        private const int DEFAULT_CAPACITY = 10;
        private const int MAX_POOL_SIZE = 100;
        private const int MAX_SOUND_INSTANCES = 30;

        private readonly List<SoundEmitter> _activeSoundEmitters;
        private IObjectPool<SoundEmitter> _soundEmitterPool;
        public Queue<SoundEmitter> FrequentSoundEmitters;
        private readonly List<SoundEmitter> _inactiveSoundEmitters = new();

        private readonly SoundEmitterFactory _soundEmitterFactory;
        private readonly SoundHolderFactory _soundHolderFactory;
        private readonly CompositionsHolderSO _compositionsHolder;
        private SoundHolder _soundHolder;
        private IPersistanceDataService _persistanceDataService;

        private readonly bool _collectionCheck = true;
        private float _currentVolume;
        public float CurrentVolume => _currentVolume;


        public AudioService(CompositionsHolderSO audioClipHolder, IAssetProvider assetProvider, IPersistanceDataService persistanceDataService)
        {
            _compositionsHolder = audioClipHolder;
            _persistanceDataService = persistanceDataService;

            _currentVolume = _persistanceDataService.Data.GameVolume;

            _soundEmitterFactory = new SoundEmitterFactory(assetProvider);
            _soundHolderFactory = new SoundHolderFactory(assetProvider);
            _activeSoundEmitters = new List<SoundEmitter>();
            FrequentSoundEmitters = new Queue<SoundEmitter>();
            InitializePool();
        }

        public SoundBuilder BuildSound() => new SoundBuilder(this, _compositionsHolder, _persistanceDataService);

        public SoundEmitter Get() => _soundEmitterPool.Get();

        public void ReturnToPool(SoundEmitter soundEmitter)
        {
            _soundEmitterPool.Release(soundEmitter);
            _soundHolder.ReturnToPlace(soundEmitter);
        }

        public void StopAll()
        {
            if (_activeSoundEmitters.Count <= 0) return;

            for (int i = _activeSoundEmitters.Count - 1; i >= 0; i--)
                _activeSoundEmitters[i].Stop();
        }

        public void ChangeVolumeForAll(float volume)
        {
            _persistanceDataService.Data.GameVolume = volume;
            _currentVolume = volume;

            foreach (var emitter in _activeSoundEmitters)
                emitter.ChangeVolume(volume);

            foreach (var emitter in FrequentSoundEmitters)
                emitter.ChangeVolume(volume);

            foreach (var emitter in _inactiveSoundEmitters)
                emitter.ChangeVolume(volume);
        }

        public bool CanPlaySound(SoundData soundData)
        {
            if (!soundData.FrequentSound) return true;

            if (FrequentSoundEmitters.Count >= MAX_SOUND_INSTANCES && FrequentSoundEmitters.TryDequeue(out var soundEmitter))
            {
                try {
                    soundEmitter.Stop();
                    return true;
                }
                catch {
                    Debug.Log("SoundEmitter was already released");
                }
                return false;
            }
            return true;
        }

        private void InitializePool()
        {
            _soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                _collectionCheck,
                DEFAULT_CAPACITY,
                MAX_POOL_SIZE);
        }

        private SoundEmitter CreateSoundEmitter()
        {
            if (_soundHolder == null)
                _soundHolder = _soundHolderFactory.CreateAndConfigure();

            var soundEmitter = _soundEmitterFactory.CreateAndConfigure(this, _soundHolder.GetPlace());
            soundEmitter.SetActive(false);
            _inactiveSoundEmitters.Add(soundEmitter);
            return soundEmitter;
        }

        private void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.SetActive(true);
            _inactiveSoundEmitters.Remove(soundEmitter);
            _activeSoundEmitters.Add(soundEmitter);
        }

        private void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            soundEmitter.SetActive(false);
            _activeSoundEmitters.Remove(soundEmitter);
            _inactiveSoundEmitters.Add(soundEmitter);
        }

        private void OnDestroyPoolObject(SoundEmitter soundEmitter) =>
            Object.Destroy(soundEmitter.gameObject);
    }
}
