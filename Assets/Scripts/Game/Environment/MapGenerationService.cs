using System.Collections.Generic;
using Game.Data;
using Game.Events;
using Game.Views;
using UnityEngine;
using VContainer.Unity;

namespace Game.Environment
{
    public sealed class MapGenerationService : IStartable
    {
        private ISignalBus _signalBus;

        private SliceableHolderView _sliceableHolder;

        private PrefabMap _prefabMap;

        private List<GameObject> _spawnedObjects = new List<GameObject>();

        public void Start()
        {
            _signalBus.Subscribe<InitializeGameEvent>(this, OnInitializeGame);
            _signalBus.Subscribe<ResetGame>(this, OnResetGame);
        }

        private void OnResetGame(ResetGame obj)
        {
            foreach (var spawnedObject in _spawnedObjects)
            {
                GameObject.Destroy(spawnedObject.gameObject);
            }

            _spawnedObjects.Add(GameObject.Instantiate(_prefabMap.Sliceables, _sliceableHolder.MySliceableRoot));
            _spawnedObjects.Add(GameObject.Instantiate(_prefabMap.Sliceables, _sliceableHolder.OpponentSliceableRoot));
        }

        private void OnInitializeGame(InitializeGameEvent obj)
        {
            _spawnedObjects.Add(GameObject.Instantiate(_prefabMap.Sliceables, _sliceableHolder.MySliceableRoot));
            _spawnedObjects.Add(GameObject.Instantiate(_prefabMap.Sliceables, _sliceableHolder.OpponentSliceableRoot));
        }

        public MapGenerationService(ISignalBus signalBus, SliceableHolderView sliceableHolder, PrefabMap prefabMap)
        {
            _signalBus = signalBus;
            _sliceableHolder = sliceableHolder;
            _prefabMap = prefabMap;
        }
    }
}