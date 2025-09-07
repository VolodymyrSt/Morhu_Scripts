using Morhu.Infrustructure.AssetManagement;
using UnityEngine;

namespace Morhu.Factory
{
    public abstract class BaseFactory<T> where T : class 
    {
        protected IAssetProvider AssetProvider;
        public BaseFactory(IAssetProvider assetProvider) => 
            AssetProvider = assetProvider;

        public abstract T Create(Vector3 at, Transform parent);

        public virtual T SpawnAt(Vector3 at, Transform parent)
        {
            var ship = Create(at , parent);
            return ship;
        }
        public virtual T SpawnAt(RectTransform parent)
        {
            var ship = Create(Vector3.zero , parent);
            return ship;
        }
        public virtual T Spawn()
        {
            var ship = Create(Vector3.zero, null);
            return ship;
        }
    }
}
