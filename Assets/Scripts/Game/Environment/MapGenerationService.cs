using Game.Events;
using UnityEngine;
using VContainer.Unity;
using Random = System.Random;

namespace Game.Environment
{
    public sealed class MapGenerationService : IStartable
    {
        private ISignalBus _signalBus;

        private Random _random;

        private int _randomSeed = -1;

        public void Start()
        {
            _signalBus.Subscribe<InitializeGameEvent>(this, OnNewGame);
            _random = new Random();
        }

        private void OnNewGame(InitializeGameEvent gameEvent)
        {
            _random = new Random(_randomSeed);
            GenerateMap();
        }

        private void GenerateMap()
        {
            var mapString = "";

            for (int i = 0; i < 10; i++)
            {
                mapString += $" {_random.Next(0, 4)}";
            }
            
            Debug.Log(mapString);
        }

        public MapGenerationService(ISignalBus signalBus)
        {
            _signalBus = signalBus;
        }
    }
}