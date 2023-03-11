using Game.Events;
using UnityEngine;
using VContainer.Unity;
using Random = System.Random;

namespace Game.Core
{
    public sealed class MapGenerationService : IStartable
    {
        private ISignalBus _signalBus;

        private Random _random;

        public void Start()
        {
            _signalBus.Subscribe<NewGameEvent>(this, OnNewGame);
            _random = new Random();
        }

        private void OnNewGame(NewGameEvent gameEvent)
        {
            _random = gameEvent.MapSeed == -1 ? new Random() : new Random(gameEvent.MapSeed);
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