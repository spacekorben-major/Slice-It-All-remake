using Game.Data;
using Game.Events;
using Game.Views;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using VContainer.Unity;

namespace Game.Core
{
    public sealed class PlayerService : IStartable
    {
        private bool isSingle = false;

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
            _signalBus.Subscribe<InitializeGameEvent>(this, OnInitializeGame);
        }

        private void OnInitializeGame(InitializeGameEvent obj)
        {
            _networkManager.OnClientConnectedCallback += id =>
            {
                if (isSingle)
                {
                    _signalBus.Publish(new AllPlayersConnected());
                    SpawnSwords();
                    return;
                }

                if (!_networkManager.IsHost || (_networkManager.IsHost && id != _networkManager.LocalClientId))
                {
                    _signalBus.Publish(new AllPlayersConnected());
                    SpawnSwords();
                }
            };
        }

        private void SpawnSwords()
        {
            foreach (var networkObject in _networkManager.SpawnManager.SpawnedObjectsList)
            {
                var syncView = networkObject.gameObject.GetComponent<PlayerDataSyncView>();
                if (syncView != null)
                {
                    var transform = GameObject.Instantiate(_prefabMap.Sword);

                    _signalBus.Publish(new KnifeAdded
                    {
                        Transform = transform,
                        IsLocal = networkObject.IsLocalPlayer,
                        NetworkTransform = networkObject.GetComponent<NetworkTransform>(),
                        SyncView = syncView
                    });
                }
            }

            _signalBus.Publish(new StartGameEvent());
        }
    }
}