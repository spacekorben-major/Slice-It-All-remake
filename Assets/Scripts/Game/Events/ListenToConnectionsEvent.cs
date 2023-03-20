namespace Game.Events
{
    public sealed class ListenToConnectionsEvent : IGameEvent
    {
        public int ClientsToGo;
    }
}