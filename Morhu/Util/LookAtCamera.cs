using Di;
using UnityEngine;

namespace Morhu.Util
{
    public class LookAtCamera : MonoBehaviour
    {
        private Camera _camera;

        private void OnEnable() => 
            _camera =  DI.Container.Resolve<Camera>();

        private void Update()
        {
            Vector3 direction = _camera.transform.position - transform.position;
            direction.y = 0; 
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
