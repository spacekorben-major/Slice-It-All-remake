namespace Game.Events
{
    public sealed class InitializeGameEvent : IGameEvent
    {
        public int MapSeed = -1;
    }
}