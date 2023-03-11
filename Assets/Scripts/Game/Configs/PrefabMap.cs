using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PrefabMap", order = 1)]
    public class PrefabMap : ScriptableObject
    {
        public PlayerDataSync PlayerDataSync;

        public GameObject Sword;
    }
}