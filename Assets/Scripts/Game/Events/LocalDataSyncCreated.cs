using Unity.Netcode;

namespace Game.Events
{
    public sealed class LocalDataSyncCreated : IGameEvent
    {
        public NetworkBehaviour PlayerDataSync;
    }
}