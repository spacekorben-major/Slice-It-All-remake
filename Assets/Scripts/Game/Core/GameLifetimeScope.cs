using Game.Data;
using Game.Environment;
using Game.Events;
using Game.Movement;
using Game.Multiplayer;
using Game.Utils;
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

        [SerializeField] private CameraView _cameraView;

        [SerializeField] private ScoreView _scoreView;

        [SerializeField] private FinalScoreView _finalScoreView;

        [SerializeField] private SliceableHolderView _sliceableHolder;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameInitializer>();
            builder.RegisterEntryPoint<MapGenerationService>();
            builder.RegisterEntryPoint<PlayerService>();
            builder.RegisterEntryPoint<PlayerMovementService>();
            builder.RegisterEntryPoint<CameraService>();
            builder.RegisterEntryPoint<SlicingService>();
            builder.RegisterEntryPoint<MultiplayerService>();
            builder.RegisterEntryPoint<MeshCut>().AsSelf();
            builder.RegisterEntryPoint<ScoringService>();

            builder.RegisterComponentInHierarchy<MainMenuView>();

            builder.Register<GameUIService>(Lifetime.Singleton);
            builder.Register<InputMap>(Lifetime.Singleton);
            builder.Register<SignalBus>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterInstance(_prefabMap);
            builder.RegisterInstance(_cameraView);
            builder.RegisterInstance(_generalConfig);
            builder.RegisterInstance(_networkManager);
            builder.RegisterInstance(_scoreView);
            builder.RegisterInstance(_finalScoreView);
            builder.RegisterInstance(_sliceableHolder);
        }
    }
}
