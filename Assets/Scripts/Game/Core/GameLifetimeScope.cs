using Game.Configs;
using Game.Events;
using Game.Views;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Core
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private PrefabMap _prefabMap;

        [SerializeField] private NetworkManager _networkManager;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameInitializer>();
            builder.RegisterEntryPoint<MapGenerationService>();
            builder.RegisterEntryPoint<PlayerService>();

            builder.RegisterComponentInHierarchy<MainMenuView>();

            builder.Register<MainMenuService>(Lifetime.Singleton);
            builder.Register<ISignalBus, SignalBus>(Lifetime.Singleton);

            builder.RegisterInstance(_prefabMap);
            builder.RegisterInstance(_networkManager);
        }
    }
}
