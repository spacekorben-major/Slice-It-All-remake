using Unity.Netcode;

public class PlayerDataSync : NetworkBehaviour
{
    public float PlayerX => _playerX.Value;
    public float PlayerY => _playerY.Value;
    public float RotationZ => _rotationZ.Value;

    private NetworkVariable<float> _playerX = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    private NetworkVariable<float> _playerY = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    private NetworkVariable<float> _rotationZ = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
}