using Game.Events;
using Game.Utils;
using VContainer.Unity;

namespace Game.Core
{
    public sealed class GameInitializer : IStartable
    {
        private GameUIService _gameUIService;

        private ISignalBus _signalBus;

        private MeshCut _meshCut;

        private bool _opponentFinished;

        private bool _playerFinished;

        public void Start()
        {
            _gameUIService.Start();
            _signalBus.Subscribe<InitializeGameEvent>(this, OnGameInitialize);
            _signalBus.Subscribe<KnifeAdded>(this, OnKnifeAdded);
            _signalBus.Subscribe<PlayerFinished>(this, OnPlayerFinished);
            _signalBus.Subscribe<ResetGame>(this, OnGameRestart);
        }

        private void OnGameInitialize(InitializeGameEvent obj)
        {
            PreHeatMeshCut();
        }

        private void PreHeatMeshCut()
        {
            // first mesh cut is considerably slower on the device
            MeshCutPreHeater.KickMono(_meshCut);
        }

        private void OnGameRestart(ResetGame obj)
        {
            _playerFinished = false;
            CheckGameRestart();
        }

        private void OnPlayerFinished(PlayerFinished obj)
        {
            _playerFinished = true;
            CheckGameCompletion();
        }

        private void OnKnifeAdded(KnifeAdded obj)
        {
            if (obj.IsLocal)
            {
                return;
            }

            obj.SyncView.Finished.OnValueChanged += OnOpponentFinishedDirty;
        }

        private void OnOpponentFinishedDirty(bool previousValue, bool newValue)
        {
            _opponentFinished = newValue;
            CheckGameCompletion();
            CheckGameRestart();
        }

        private void CheckGameRestart()
        {
            if (!_opponentFinished && !_playerFinished)
            {
                _signalBus.Publish(new StartGameEvent());
            }
        }

        private void CheckGameCompletion()
        {
            if (_opponentFinished && _playerFinished)
            {
                _signalBus.Publish(new ShowFinalScoreSignal());
            }
        }

        public GameInitializer(GameUIService gameUIService, ISignalBus signalBus, MeshCut meshCut)
        {
            _gameUIService = gameUIService;
            _signalBus = signalBus;
            _meshCut = meshCut;
        }
    }
}