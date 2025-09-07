using Morhu.Factory;
using Morhu.Infrustructure.AssetManagement;
using UnityEngine;

namespace Morhu.Infrustructure.Services.AudioSystem
{
    public class SoundHolderFactory : BaseFactory<SoundHolder>
    {
        public SoundHolderFactory(IAssetProvider assetProvider) : base(assetProvider) { }

        public override SoundHolder Create(Vector3 at, Transform parent) =>
            AssetProvider.Instantiate<SoundHolder>(AssetPath.SoundHolderPath, at, parent);

        public SoundHolder CreateAndConfigure()
        {
            var soundEmitter = Create(Vector3.zero, null);
            return soundEmitter;
        }
    }
}
