using UnityEngine;


namespace Di
{
    public abstract class ConstructedMonoBehaviour : MonoBehaviour
    {
        public abstract void Construct(IResolvable container);
    }
}
