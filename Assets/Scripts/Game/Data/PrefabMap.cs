using Game.Views;
using Unity.Netcode.Components;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PrefabMap", order = 1)]
    public class PrefabMap : ScriptableObject
    {
        public KnifeView Sword;

        public NetworkTransform PlayerDataSync;
    }
}