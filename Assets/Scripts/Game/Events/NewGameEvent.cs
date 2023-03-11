namespace Game.Events
{
    public sealed class NewGameEvent : IGameEvent
    {
        public int MapSeed = -1;
    }
}