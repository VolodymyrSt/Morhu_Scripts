using System;

namespace Di
{
    public abstract class DIEntry : IDisposable
    {
        protected DIContainer Container {  get; set; }
        protected bool IsSingleton {  get; set; }
        protected bool IsInstance {  get; set; }
        public bool IsConstructed {  get; set; }

        public DIEntry() { }

        public DIEntry(DIContainer container) => Container = container;

        public T Resolve<T>() => 
            ((DIEntry<T>)this).Resolve();

        public DIEntry AsSingle()
        {
            IsSingleton = true;
            return this;
        }

        public DIEntry NeedToBeConstructed()
        {
            if (!IsInstance)
                throw new Exception("NeedToBeConstructed can only be applied to instance (Mono) entries.");

            IsConstructed = true;
            return this;
        }

        public abstract DIEntry Construct(IResolvable container);
        public abstract DIEntry NotLazy();
        public abstract void Dispose();
    }

    public class DIEntry<T> : DIEntry
    {
        private Func<DIContainer, T> _factory { get; set; }
        private T _instance;

        private IDisposable _disposableInstance;

        public DIEntry(DIContainer container, Func<DIContainer, T> factory) : base(container) =>  
            _factory = factory;

        
        public DIEntry(T instance)
        {
            _instance = instance;

            if (_instance is IDisposable disposableInstance)
                _disposableInstance = disposableInstance;

            IsSingleton = true;
            IsInstance = true;
        }

        public T Resolve()
        {
            if (IsSingleton)
            {
                if (_instance == null)
                {
                    _instance = _factory(Container);

                    if (_instance is IDisposable disposableInstance)
                        _disposableInstance = disposableInstance;
                }
                return _instance;
            }
            return _factory(Container);
        }

        public override DIEntry NotLazy()
        {
            if (!IsSingleton)
                throw new Exception("NotLazy can only be applied to singleton (AsSingle) entries.");

            Resolve();
            return this;
        }

        public override DIEntry Construct(IResolvable container)
        {
            if (Resolve() is ConstructedMonoBehaviour constructedMonoBehaviour)
                constructedMonoBehaviour.Construct(container);

            return this;
        }

        public override void Dispose() => 
            _disposableInstance?.Dispose();
    }
}
