using System;

namespace Di
{
    public static class DI
    {
        private static DIContainer _mainContainer;
        private static DIContainer _container;

        public static DIContainer MainContainer =>
            _mainContainer;

        public static DIContainer Container =>
            _container;

        public static void InitMainContainer()
        {
            if (_mainContainer != null)
                throw new InvalidOperationException("Main container already initialized");

            _mainContainer = new DIContainer();
        }

        public static void InitContainer()
        {
            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }          

            _container = new DIContainer(_mainContainer);
        }
    }
}
