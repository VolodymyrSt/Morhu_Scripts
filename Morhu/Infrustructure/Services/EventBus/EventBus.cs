using System.Collections.Generic;
using System;

namespace Morhu.Infrustructure.Services.EventBus
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<object>> _signals;

        public EventBus() => _signals = new Dictionary<Type, List<object>>();

        public void SubscribeEvent<T>(Action<T> callback)
        {
            var key = typeof(T);

            if (_signals.ContainsKey(key))
                _signals[key].Add(callback);
            else
                _signals.Add(key, new List<object>() { callback });
        }
        
        public void SubscribeEvent<T>(Action callback)
        {
            var key = typeof(T);

            if (_signals.ContainsKey(key))
                _signals[key].Add(callback);
            else
                _signals.Add(key, new List<object>() { callback });
        }

        public void UnSubscribeEvent<T>(Action<T> callback)
        {
            var key = typeof(T);

            if (!_signals.ContainsKey(key))
                throw new Exception($"You are trying to unregister not existed event {key}");

            _signals[key]?.Remove(callback);
        }
        
        public void UnSubscribeEvent<T>(Action callback)
        {
            var key = typeof(T);

            if (!_signals.ContainsKey(key))
                throw new Exception($"You are trying to unregister not existed event {key}");

            _signals[key]?.Remove(callback);
        }

        public void Invoke<T>(T signal)
        {
            var key = typeof(T);

            if (!_signals.ContainsKey(key))
                throw new Exception($"You are trying to invoke not register event {key}");

            foreach (var obj in _signals[key])
            {
                var myEvent = obj as Action<T>;
                myEvent?.Invoke(signal);
            }
        }
        
        public void Invoke<T>()
        {
            var key = typeof(T);

            if (!_signals.ContainsKey(key))
                throw new Exception($"You are trying to invoke not register event {key}");

            foreach (var obj in _signals[key])
            {
                var myEvent = obj as Action;
                myEvent?.Invoke();
            }
        }

        public void ClearSignals() => _signals.Clear();
    }
}
