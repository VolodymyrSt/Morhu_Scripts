using System;

namespace Morhu.Infrustructure.Services.EventBus
{
    public interface IEventBus
    {
        void Invoke<T>(T signal);
        void Invoke<T>();
        void SubscribeEvent<T>(Action<T> callback);
        void SubscribeEvent<T>(Action callback);
        void UnSubscribeEvent<T>(Action<T> callback);
        void UnSubscribeEvent<T>(Action callback);
    }
}