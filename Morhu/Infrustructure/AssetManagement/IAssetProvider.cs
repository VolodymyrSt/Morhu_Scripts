using UnityEngine;

namespace Morhu.Infrustructure.AssetManagement
{
    public interface IAssetProvider
    {
        public T Instantiate<T>(string path) where T : Object;
        public T Instantiate<T>(string path, Vector3 at, Transform parent) where T : Object;

        GameObject Instantiate(string path);
        GameObject Instantiate(string path, Vector3 at, Transform parent);
    }
}