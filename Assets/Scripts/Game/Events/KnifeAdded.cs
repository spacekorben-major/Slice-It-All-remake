using Game.Views;
using Unity.Netcode.Components;

namespace Game.Events
{
    public class KnifeAdded : IGameEvent
    {
        public KnifeView Transform;

        public NetworkTransform PlayerDataSync;

        public bool IsLocal;
    }
}