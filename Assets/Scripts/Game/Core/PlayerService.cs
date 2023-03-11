using System.Collections.Generic;
using Game.Configs;
using Unity.Netcode;
using UnityEngine;
using VContainer.Unity;

namespace Game.Core
{
    public sealed class PlayerService : IStartable
    {
        private PrefabMap _prefabMap;

        private NetworkManager _networkManager;

        private Dictionary<ulong, PlayerDataSync> _playerDataSyncObjects;

        public PlayerService(PrefabMap prefabMap, NetworkManager networkManager)
        {
            _networkManager = networkManager;
            _prefabMap = prefabMap;
        }

        public void Start()
        {
            _networkManager.OnClientConnectedCallback += OnPlayerConnected;
        }

        private void OnPlayerConnected(ulong playerId)
        {
            var playerSyncData = Object.Instantiate(_prefabMap.PlayerDataSync);

            if (_networkManager.IsServer)
            {
                playerSyncData.NetworkObject.Spawn(true);
            }
        }
    }
}