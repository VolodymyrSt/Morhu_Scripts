using Di;
using UnityEngine;

namespace Morhu.Infrustructure.Installers
{
    public abstract class LocalInstaller : MonoBehaviour
    {
        protected DIContainer Container;

        protected abstract void InstallBindings();

        public void PerformBinding()
        {
            DI.InitContainer();
            Container = DI.Container;

            InstallBindings();
            Container.ConstructAllInstances(Container);
        }

        private void OnDestroy() => 
            Container?.Dispose();
    }
}
