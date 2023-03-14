using Game.Data;
using Game.Events;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using VContainer.Unity;

namespace Game.Core
{
    public sealed class PlayerService : IStartable
    {
        private PrefabMap _prefabMap;

        private NetworkManager _networkManager;

        private ISignalBus _signalBus;

        public PlayerService(PrefabMap prefabMap, NetworkManager networkManager, ISignalBus signalBus)
        {
            _networkManager = networkManager;
            _prefabMap = prefabMap;
            _signalBus = signalBus;
        }

        public void Start()
        {
            _signalBus.Subscribe<InitializeGameEvent>(this, SpawnSwords);
        }

        public void SpawnSwords(InitializeGameEvent e)
        {
            foreach (var networkObject in _networkManager.SpawnManager.SpawnedObjectsList)
            {
                if (networkObject.gameObject.name == _prefabMap.PlayerDataSync.gameObject.name + "(Clone)")
                {
                    var transform = GameObject.Instantiate(_prefabMap.Sword);
                    _signalBus.Publish(new KnifeAdded
                    {
                        Transform = transform,
                        IsLocal = networkObject.IsLocalPlayer,
                        PlayerDataSync = networkObject.GetComponent<NetworkTransform>()
                    });
                }
            }

            _signalBus.Publish(new StartGameEvent());
        }
    }
}