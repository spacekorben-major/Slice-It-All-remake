namespace Game.Events
{
    public sealed class MultiplayerConnectionError : IGameEvent
    {
        public string Error;
        public MultiplayerConnectionError(string error)
        {
            Error = error;
        }
    }
}