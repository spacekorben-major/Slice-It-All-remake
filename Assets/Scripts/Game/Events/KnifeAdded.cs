using Game.Views;
using Unity.Netcode.Components;

namespace Game.Events
{
    public sealed class KnifeAdded : IGameEvent
    {
        public KnifeView Transform;

        public NetworkTransform NetworkTransform;

        public PlayerDataSyncView SyncView;

        public bool IsLocal;
    }
}