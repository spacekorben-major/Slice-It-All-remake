using Game.Events;
using Game.Views;
using UnityEngine;
using VContainer.Unity;

namespace Game.Core
{
    public class CameraService : IStartable
    {
        private ISignalBus _signalBus;

        private CameraView _cameraView;

        private Transform _opponentKnifeView;
        
        private Transform _localKnifeView;

        public void Start()
        {
            _cameraView.TreeCamera.Priority = 10;
            _cameraView.SwordCamera.Priority = 1;
            _signalBus.Subscribe<StartGameEvent>(this, OnStartGameEvent);
            _signalBus.Subscribe<PlayerFinished>(this, OnPlayerFinished);
            _signalBus.Subscribe<KnifeAdded>(this, OnKnifeAdded);
            _signalBus.Subscribe<FinalScorePublished>(this, OnScorePublished);
        }

        private void OnScorePublished(FinalScorePublished obj)
        {
            _cameraView.TreeCamera.Priority = 10;
            _cameraView.SwordCamera.Priority = 1;
        }

        private void OnPlayerFinished(PlayerFinished obj)
        {
            _cameraView.SwordCamera.LookAt = _opponentKnifeView;
        }

        private void OnStartGameEvent(StartGameEvent obj)
        {
            if (_cameraView.SwordCamera.LookAt != _localKnifeView)
            {
                _cameraView.SwordCamera.LookAt = _localKnifeView;
            }

            _cameraView.SwordCamera.Priority = 10;
            _cameraView.TreeCamera.Priority = 1;
        }

        private void OnKnifeAdded(KnifeAdded obj)
        {
            if (obj.IsLocal)
            {
                _localKnifeView = obj.Transform.transform;

                _cameraView.SwordCamera.Follow = _localKnifeView;
                _cameraView.SwordCamera.LookAt = _localKnifeView;
            }
            else
            {
                _opponentKnifeView = obj.Transform.transform;
            }
        }

        public CameraService(ISignalBus signalBus, CameraView cameraView)
        {
            _signalBus = signalBus;
            _cameraView = cameraView;
        }
    }
}