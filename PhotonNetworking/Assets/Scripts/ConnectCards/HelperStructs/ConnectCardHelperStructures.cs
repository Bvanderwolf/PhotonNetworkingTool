public struct InLobbyConnectResult
{
    public object args;
    public InLobbyConnectChoice choice;
}

public struct CreateRoomFormResult
{
    public string Name;
    public int MaxPlayers;
    public bool IsVisible;
    public bool IsOpen;
}