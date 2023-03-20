using Unity.Netcode;

namespace Game.Views
{
    public class PlayerDataSyncView : NetworkBehaviour
    {
        public NetworkVariable<int> PlayerScore =
            new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public NetworkVariable<bool> Finished =
            new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Owner);
    }
}