using UnityEngine;

namespace Morhu.Util
{
    public static class Extentions
    {
        public static T AddOrGet<T>(this GameObject gameObject) where T : Component
        {
            T componet = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
            return componet;
        }

        public static void SetActive(this MonoBehaviour monoBehaviour, bool value) => 
            monoBehaviour.gameObject.SetActive(value);  
        
        public static void SetActive(this Transform transform, bool value) =>
            transform.gameObject.SetActive(value);
    }
}
