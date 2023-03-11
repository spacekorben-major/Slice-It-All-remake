using System;

namespace Game.Events
{
    public interface ISignalBus : ISignalPublisher
    {
        public void Subscribe<T>(object key, Action<T> action) where T : IGameEvent;

        public void UnsubscribeFromAll(object key);

        public void Unsubscribe<T>(object key, Action<T> action) where T : IGameEvent;
    }
}