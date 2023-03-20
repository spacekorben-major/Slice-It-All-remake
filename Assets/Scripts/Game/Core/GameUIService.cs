using Game.Events;
using Game.Views;

namespace Game.Core
{
    public sealed class GameUIService
    {
        private MainMenuView _mainMenuView;

        private FinalScoreView _finalScoreView;

        private ISignalBus _signalBus;

        public void Start()
        {
            _mainMenuView.gameObject.SetActive(true);
            _finalScoreView.gameObject.SetActive(false);
            _finalScoreView.Button.onClick.AddListener(Rematch);
            _mainMenuView.StartSolo.onClick.AddListener(InitializeGame);
            _signalBus.Subscribe<FinalScorePublished>(this, OnScorePublished);
        }

        private void OnScorePublished(FinalScorePublished obj)
        {
            _finalScoreView.gameObject.SetActive(true);
            _finalScoreView.EnemyScore.text = $"Opponent scored: {obj.OpponentScore.ToString()}";
            _finalScoreView.MyScore.text = $"You scored {obj.MyScore.ToString()}";

            if (obj.OpponentScore > obj.MyScore)
            {
                _finalScoreView.Result.text = "You lost";
            }
            else if (obj.MyScore > obj.OpponentScore)
            {
                _finalScoreView.Result.text = "You win";
            }
            else
            {
                _finalScoreView.Result.text = "Draw";
            }
        }

        private void Rematch()
        {
            _finalScoreView.gameObject.SetActive(false);
            _signalBus.Publish(new ResetGame());
        }

        private void InitializeGame()
        {
            _signalBus.Publish(new InitializeGameEvent());
            _mainMenuView.gameObject.SetActive(false);
        }

        public GameUIService(MainMenuView view, FinalScoreView finalScoreView, ISignalBus signalBus)
        {
            _mainMenuView = view;
            _finalScoreView = finalScoreView;
            _signalBus = signalBus;
        }
    }
}