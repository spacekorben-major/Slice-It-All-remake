using Game.Events;
using Game.Views;

namespace Game.Core
{
    public sealed class MainMenuService
    {
        private MainMenuView _mainMenuView;

        private ISignalBus _signalBus;

        public void Start()
        {
            _mainMenuView.enabled = true;
            _mainMenuView.StartSolo.onClick.AddListener(() => _signalBus.Publish(new InitializeGameEvent()));
            _signalBus.Subscribe<StartGameEvent>(this, _ => _mainMenuView.enabled = false);
        }

        public MainMenuService(MainMenuView view, ISignalBus signalBus)
        {
            _mainMenuView = view;
            _signalBus = signalBus;
        }
    }
}