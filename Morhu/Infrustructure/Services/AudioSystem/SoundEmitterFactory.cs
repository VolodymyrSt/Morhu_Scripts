using Morhu.Factory;
using Morhu.Infrustructure.AssetManagement;
using UnityEngine;

namespace Morhu.Infrustructure.Services.AudioSystem
{
    public class SoundEmitterFactory : BaseFactory<SoundEmitter>
    {
        public SoundEmitterFactory(IAssetProvider assetProvider) : base(assetProvider){}

        public override SoundEmitter Create(Vector3 at, Transform parent) => 
            AssetProvider.Instantiate<SoundEmitter>(AssetPath.SoundEmitterPath, at, parent);

        public SoundEmitter CreateAndConfigure(AudioService soundManager, Transform parent)
        {
            var soundEmitter = Create(Vector3.zero, parent);
            soundEmitter.InitEmitter(soundManager);
            return soundEmitter;
        }
    }
}
