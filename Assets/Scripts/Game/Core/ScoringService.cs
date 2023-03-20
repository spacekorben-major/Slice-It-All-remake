using Game.Events;
using Game.Views;
using UnityEngine;
using VContainer.Unity;

namespace Game.Core
{
    public sealed class ScoringService : IStartable
    {
        private int _playerScore;

        private ISignalBus _signalBus;

        private ScoreView _scoreView;

        private PlayerDataSyncView _localPlayedDataSync;

        private PlayerDataSyncView _opponentsScoreView;

        public ScoringService(ISignalBus signalBus, ScoreView scoreView)
        {
            _signalBus = signalBus;

            _scoreView = scoreView;
        }

        public void Start()
        {
            _scoreView.gameObject.SetActive(false);
            _signalBus.Subscribe<SlicedEvent>(this, OnSlicedEvent);
            _signalBus.Subscribe<KnifeAdded>(this, OnKnifeAdded);
            _signalBus.Subscribe<StartGameEvent>(this, OnStartGame);
            _signalBus.Subscribe<PlayerFinished>(this, OnPlayerFinished);
            _signalBus.Subscribe<ShowFinalScoreSignal>(this, OnFinalScoreRequest);
        }

        private void OnFinalScoreRequest(ShowFinalScoreSignal obj)
        {
            _scoreView.gameObject.SetActive(false);
            _signalBus.Publish(new FinalScorePublished
            {
                MyScore = _playerScore,
                OpponentScore = _opponentsScoreView.PlayerScore.Value
            });
        }

        private void OnKnifeAdded(KnifeAdded obj)
        {
            if (obj.IsLocal)
            {
                _localPlayedDataSync = obj.SyncView;
            }
            else
            {
                _opponentsScoreView = obj.SyncView;
            }
        }

        private void OnPlayerFinished(PlayerFinished obj)
        {
            _playerScore = (int)Mathf.Floor(_playerScore * obj.ScoreMod);
            UpdateScore();
        }

        private void OnStartGame(StartGameEvent obj)
        {
            _scoreView.gameObject.SetActive(true);
            _playerScore = 0;
            UpdateScore();
        }

        private void OnSlicedEvent(SlicedEvent obj)
        {
            if (!obj.IsLocalPlayer)
            {
                return;
            }

            _playerScore++;
            UpdateScore();
        }

        private void UpdateScore()
        {
            _localPlayedDataSync.PlayerScore.Value = _playerScore;
            _scoreView.TextMeshProUGUI.text = _playerScore.ToString();
        }
    }
}