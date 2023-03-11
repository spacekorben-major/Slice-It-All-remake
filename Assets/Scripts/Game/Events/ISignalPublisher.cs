namespace Game.Events
{
    public interface ISignalPublisher
    {
        public void Publish(IGameEvent gameEvent);
    }
}