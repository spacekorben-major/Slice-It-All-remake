using Game.Views;
using Unity.Netcode.Components;
using UnityEngine;

namespace Game.Data
{
    public class KnifeData
    {
        public KnifeView KnifeView;

        public NetworkTransform Transform;

        public PlayerDataSyncView PlayerDataSync;

        public Collision UnprocessedCollision;
    }
}