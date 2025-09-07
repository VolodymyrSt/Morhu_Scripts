using System;
using System.Collections.Generic;


namespace Di
{
    public class DIContainer : IResolvable
    {
        private readonly DIContainer _parentContainer;

        private readonly Dictionary<(string, Type), DIEntry> _entriesMap = new();
        private readonly HashSet<(string, Type)> _resolutionsCache = new();

        public DIContainer(DIContainer parentContainer = null) => 
            _parentContainer = parentContainer;

        public DIEntry Bind<T>(Func<DIContainer, T> factory) => 
            Bind(factory, null);

        public DIEntry Bind<T>(Func<DIContainer, T> factory, string tag)
        {
            var key = (tag, typeof(T));

            if (_entriesMap.ContainsKey(key))
            {
                throw new Exception(
                    $"DI: Factory with tag {key.Item1} and type {key.Item2.FullName} has already registered");
            }

            var diEntry = new DIEntry<T>(this, factory);

            _entriesMap[key] = diEntry;

            return diEntry;
        }

        public DIEntry BindInstance<T>(T instance) => 
            BindInstance(instance, null);

        public DIEntry BindInstance<T>(T instance, string tag)
        {
            var key = (tag, typeof(T));

            if (_entriesMap.ContainsKey(key))
            {
                throw new Exception(
                    $"DI: Instance with tag {key.Item1} and type {key.Item2.FullName} has already registered");
            }

            var diEntry = new DIEntry<T>(instance);

            _entriesMap[key] = diEntry;
            return diEntry;
        }

        public T Resolve<T>(string tag = null)
        {
            var key = (tag, typeof(T));

            if (_resolutionsCache.Contains(key))
                throw new Exception($"DI: Cyclic dependency for tag {key.tag} and type {key.Item2.FullName}");

            _resolutionsCache.Add(key);

            try
            {
                if (_entriesMap.TryGetValue(key, out DIEntry dIEntry))
                    return dIEntry.Resolve<T>();
                else
                {
                    if (_parentContainer != null)
                        return _parentContainer.Resolve<T>(tag);
                }
            }
            finally
            {
                _resolutionsCache.Remove(key);
            }

            throw new Exception($"Couldn't find dependency for tag {tag} and type {key.Item2.FullName}");
        }

        public void ConstructAllInstances(IResolvable container)
        {
            foreach (var entry in _entriesMap.Values)
            {
                if (entry.IsConstructed)
                    entry.Construct(container);
            }
        }

        public void Dispose()
        {
            foreach (var entry in _entriesMap.Values)
                entry.Dispose();
        }
    }
}
