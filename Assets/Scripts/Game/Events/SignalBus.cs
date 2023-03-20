using System;
using System.Collections.Generic;
using VContainer.Unity;

namespace Game.Events
{
    public sealed class SignalBus : ISignalBus, ITickable
    {
        private readonly Dictionary<Type, List<Delegate>> _eventToSubscriptionMap = new();

        private readonly Dictionary<object, List<(Type, Delegate)>> _keyToSubscriptionMap = new();

        private readonly object[] _args = new object[1];

        private Queue<IGameEvent> _eventQueue = new Queue<IGameEvent>(); 

        public void Publish(IGameEvent gameEvent)
        {
            _eventQueue.Enqueue(gameEvent);
        }

        public void Subscribe<T>(object key, Action<T> action) where T : IGameEvent
        {
            if (!_eventToSubscriptionMap.ContainsKey(typeof(T)))
            {
                _eventToSubscriptionMap.Add(typeof(T), new List<Delegate>());
            }

            _eventToSubscriptionMap[typeof(T)].Add(action);

            if (!_keyToSubscriptionMap.ContainsKey(key))
            {
                _keyToSubscriptionMap.Add(key, new List<(Type, Delegate)>());
            }

            _keyToSubscriptionMap[key].Add((typeof(T), action));
        }

        public void UnsubscribeFromAll(object key)
        {
            if (!_keyToSubscriptionMap.ContainsKey(key))
            {
                return;
            }

            foreach (var (type, action) in _keyToSubscriptionMap[key])
            {
                if (!_eventToSubscriptionMap.ContainsKey(type))
                {
                    continue;
                }

                _eventToSubscriptionMap[type].Remove(action);
            }

            _keyToSubscriptionMap[key].Clear();
        }

        public void Unsubscribe<T>(object key, Action<T> action) where T : IGameEvent
        {
            if (!_keyToSubscriptionMap.ContainsKey(key))
            {
                return;
            }

            _keyToSubscriptionMap[key].Remove((typeof(T), action));

            _eventToSubscriptionMap[typeof(T)].Remove(action);
        }

        public void Tick()
        {
            while (_eventQueue.Count > 0)
            {
                var gameEvent = _eventQueue.Dequeue();
                var eventType = gameEvent.GetType();

                if (!_eventToSubscriptionMap.ContainsKey(eventType))
                {
                    return;
                }

                _args[0] = gameEvent;

                foreach (var eventDelegate in _eventToSubscriptionMap[eventType])
                {
                    eventDelegate.DynamicInvoke(_args);
                }
            }
        }
    }
}