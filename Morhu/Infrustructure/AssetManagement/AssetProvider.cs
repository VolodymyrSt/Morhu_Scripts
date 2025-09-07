using UnityEngine;

namespace Morhu.Infrustructure.AssetManagement
{
    public class AssetProvider : IAssetProvider
    {
        public T Instantiate<T>(string path) where T : Object
        {
            var prefab = Resources.Load<T>(path);
            return Object.Instantiate<T>(prefab);
        }   
        
        public T Instantiate<T>(string path, Vector3 at, Transform parent) where T : Object
        {
            var prefab = Resources.Load<T>(path);
            return Object.Instantiate<T>(prefab, at, Quaternion.identity, parent);
        }

        public GameObject Instantiate(string path)
        {
            var prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab);
        }

        public GameObject Instantiate(string path, Vector3 at, Transform parent)
        {
            var prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab, at, Quaternion.identity, parent);
        }
    }
}
