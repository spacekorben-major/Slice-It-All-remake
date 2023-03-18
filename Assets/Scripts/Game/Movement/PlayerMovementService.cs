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

        private bool _gameRunning = false;

        public void Start()
        {
            _signalBus.Subscribe<KnifeAdded>(this, OnPlayerAdded);
            _signalBus.Subscribe<StartGameEvent>(this, _ =>
            {
                InitializeControllers();
                _gameRunning = true;
                _inputMap.Enable();
            });
        }

        public void Tick()
        {
            if (!_gameRunning)
            {
                return;
            }

            var deltaTime = Time.deltaTime;

            foreach (var localController in _localControllers)
            {
                localController.Apply(_localKnifeData, deltaTime);
            }

            foreach (var otherData in _enemyDatas)
            {
                foreach (var enemyController in _enemyControllers)
                {
                    enemyController.Apply(otherData);
                }
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

            _enemyControllers.Add(new PositionAdjustEnemyController(_generalConfig));
        }

        private void OnPlayerAdded(KnifeAdded knifeAdded)
        {
            if (knifeAdded.IsLocal)
            {
                _localKnifeData = new LocalKnifeData
                {
                    Stuck = true,
                    HandleHit = false,
                    Velocity = Vector3.zero,
                    AngularVelocity = Vector3.zero,
                    KnifeView = knifeAdded.Transform,
                    PlayerDataSync = knifeAdded.PlayerDataSync
                };

                //TODO: organize controllers
                _localControllers.Add(new CollisionController(_localKnifeData, _generalConfig, _signalBus)); 
                
                return;
            }

            _enemyDatas.Add(new KnifeData
            {
                KnifeView = knifeAdded.Transform,
                PlayerDataSync = knifeAdded.PlayerDataSync
            });
        }
    }
}