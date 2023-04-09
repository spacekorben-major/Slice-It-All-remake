namespace Game.Multiplayer
{
    public enum MultiplayerJoinSequence
    {
        None,
        InitializeUnityServices,
        SignIn,
        ListLobbies,
        CreateRelayAllocation,
        GetAllocationJoinCode,
        CreateLobby,
        JoinLobby,
        JoinAllocation,
        CheckLobbyCollision
    }
}