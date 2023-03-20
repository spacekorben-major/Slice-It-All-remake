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

        public void Start()
        {
            _signalBus.Subscribe<InitializeGameEvent>(this, OnInitializeGame);
            _signalBus.Subscribe<ResetGame>(this, OnResetGame);
        }

        private void OnResetGame(ResetGame obj)
        {
            GameObject.Instantiate(_prefabMap.Sliceables, _sliceableHolder.MySliceableRoot);
            GameObject.Instantiate(_prefabMap.Sliceables, _sliceableHolder.OpponentSliceableRoot);
        }

        private void OnInitializeGame(InitializeGameEvent obj)
        {
            GameObject.Instantiate(_prefabMap.Sliceables, _sliceableHolder.MySliceableRoot);
            GameObject.Instantiate(_prefabMap.Sliceables, _sliceableHolder.OpponentSliceableRoot);
        }

        public MapGenerationService(ISignalBus signalBus, SliceableHolderView sliceableHolder, PrefabMap prefabMap)
        {
            _signalBus = signalBus;
            _sliceableHolder = sliceableHolder;
            _prefabMap = prefabMap;
        }
    }
}