using Cinemachine;
using Game.Events;
using VContainer.Unity;

namespace Game.Core
{
    public class CameraService : IStartable
    {
        private ISignalBus _signalBus;

        private CinemachineVirtualCamera _camera;
        
        public void Start()
        {
            _signalBus.Subscribe<KnifeAdded>(this, OnKnifeAdded);
        }

        private void OnKnifeAdded(KnifeAdded obj)
        {
            if (obj.IsLocal)
            {
                var transform = obj.Transform.transform;

                _camera.Follow = transform;
                _camera.LookAt = transform;
            }
        }

        public CameraService(ISignalBus signalBus, CinemachineVirtualCamera camera)
        {
            _signalBus = signalBus;
            _camera = camera;
        }
    }
}