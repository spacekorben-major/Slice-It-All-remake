using Cinemachine;
using Game.Data;
using Game.Environment;
using Game.Events;
using Game.Movement;
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

        [SerializeField] private GeneralConfig _generalConfig;

        [SerializeField] private NetworkManager _networkManager;

        [SerializeField] private CinemachineVirtualCamera _camera;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameInitializer>();
            builder.RegisterEntryPoint<MapGenerationService>();
            builder.RegisterEntryPoint<PlayerService>();
            builder.RegisterEntryPoint<PlayerMovementService>();
            builder.RegisterEntryPoint<CameraService>();

            builder.RegisterComponentInHierarchy<MainMenuView>();

            builder.Register<MainMenuService>(Lifetime.Singleton);
            builder.Register<InputMap>(Lifetime.Singleton);
            builder.Register<SignalBus>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterInstance(_prefabMap);
            builder.RegisterInstance(_camera);
            builder.RegisterInstance(_generalConfig);
            builder.RegisterInstance(_networkManager);
        }
    }
}
