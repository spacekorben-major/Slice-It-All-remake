using System;
using System.Collections.Generic;

namespace Game.Events
{
    public sealed class SignalBus : ISignalBus
    {
        private readonly Dictionary<Type, List<Delegate>> _eventToSubscriptionMap = new();

        private readonly Dictionary<object, List<(Type, Delegate)>> _keyToSubscriptionMap = new();

        private readonly object[] _args = new object[1];

        public void Publish(IGameEvent gameEvent)
        {
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
    }
}