using System.Collections.Generic;
using Game.Data;
using Game.Events;
using Game.Movement.Controllers;
using UnityEngine;
using VContainer.Unity;

namespace Game.Movement
{
    public class PlayerMovementService : IStartable, ITickable
    {
        private ISignalBus _signalBus;

        private GeneralConfig _generalConfig;

        private InputMap _inputMap;

        private List<ILocalController> _localControllers = new();

        private List<IEnemyController> _enemyControllers = new();

        private LocalKnifeData _localKnifeData;

        private List<KnifeData> _enemyDatas = new();

        private bool _localPlayerActive;
        private bool _gameStarted;

        public void Start()
        {
            _signalBus.Subscribe<KnifeAdded>(this, OnPlayerAdded);
            _signalBus.Subscribe<StartGameEvent>(this, OnGameStarted);
            _signalBus.Subscribe<PlayerFinished>(this, OnPlayerFinished);
            _signalBus.Subscribe<ResetGame>(this, OnGameReset);
        }

        private void OnGameReset(ResetGame obj)
        {
            _localKnifeData.KnifeView.transform.position = _generalConfig.StartingPosition;
            _localKnifeData.KnifeView.transform.rotation = Quaternion.Euler(_generalConfig.StartingRotation);

            _localKnifeData.Transform.transform.position = _generalConfig.StartingPosition;
            _localKnifeData.Transform.transform.rotation = Quaternion.Euler(_generalConfig.StartingRotation);

            _localKnifeData.PlayerDataSync.Finished.Value = false;

            _localKnifeData.Stuck = true;
            _localKnifeData.Velocity = Vector3.zero;
            _localKnifeData.HandleHit = false;
            _localKnifeData.AngularVelocity = Vector3.zero;
        }

        private void OnGameStarted(StartGameEvent obj)
        {
            if (_localControllers.Count == 0)
            {
                InitializeControllers();
            }

            _localPlayerActive = true;
            _gameStarted = true;
            _inputMap.Enable();
        }

        private void OnPlayerFinished(PlayerFinished obj)
        {
            _inputMap.Disable();
            _localPlayerActive = false;
        }

        public void Tick()
        {
            if (!_gameStarted)
            {
                return;
            }

            foreach (var otherData in _enemyDatas)
            {
                foreach (var enemyController in _enemyControllers)
                {
                    enemyController.Apply(otherData);
                }
            }

            if (!_localPlayerActive)
            {
                return;
            }

            var deltaTime = Time.deltaTime;

            foreach (var localController in _localControllers)
            {
                localController.Apply(_localKnifeData, deltaTime);
            }
        }

        public PlayerMovementService(ISignalBus signalBus, GeneralConfig generalConfig, InputMap inputMap)
        {
            _signalBus = signalBus;
            _generalConfig = generalConfig;
            _inputMap = inputMap;
        }

        private void InitializeControllers()
        {
            _localControllers.Add(new GravityController(_generalConfig));
            _localControllers.Add(new PlayerHorizontalMovementController(_generalConfig));
            _localControllers.Add(new InputController(_generalConfig, _inputMap));
            _localControllers.Add(new RotationController(_generalConfig));
            _localControllers.Add(new VelocityApplierController());
            _localControllers.Add(new PositionTranslationController());
            _localControllers.Add(new CollisionController(_generalConfig, _signalBus)); 

            _enemyControllers.Add(new PositionAdjustEnemyController(_generalConfig));
            _enemyControllers.Add(new EnemyCollisionController(_signalBus));
        }

        private void OnPlayerAdded(KnifeAdded knifeAdded)
        {
            if (knifeAdded.IsLocal)
            {

                knifeAdded.Transform.transform.position = _generalConfig.StartingPosition;
                knifeAdded.Transform.transform.rotation = Quaternion.Euler(_generalConfig.StartingRotation);

                _localKnifeData = new LocalKnifeData
                {
                    Stuck = true,
                    HandleHit = false,
                    Velocity = Vector3.zero,
                    AngularVelocity = Vector3.zero,
                    KnifeView = knifeAdded.Transform,
                    Transform = knifeAdded.NetworkTransform,
                    PlayerDataSync = knifeAdded.SyncView
                };

                foreach (var collider in knifeAdded.Transform.Colliders)
                {
                    collider.CollisionTrigger += collision => _localKnifeData.UnprocessedCollision = collision;
                }

                return;
            }

            var enemyData = new KnifeData
            {
                KnifeView = knifeAdded.Transform,
                Transform = knifeAdded.NetworkTransform,
                PlayerDataSync = knifeAdded.SyncView
            };

            foreach (var collider in knifeAdded.Transform.Colliders)
            {
                collider.CollisionTrigger += collision => enemyData.UnprocessedCollision = collision;
            }

            _enemyDatas.Add(enemyData);
        }
    }
}