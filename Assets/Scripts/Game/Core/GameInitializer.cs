using VContainer.Unity;

namespace Game.Core
{
    public sealed class GameInitializer : IStartable
    {
        private MainMenuService _mainMenuService;

        public void Start()
        {
            _mainMenuService.Start();
        }

        public GameInitializer(MainMenuService mainMenuService)
        {
            _mainMenuService = mainMenuService;
        }
    }
}