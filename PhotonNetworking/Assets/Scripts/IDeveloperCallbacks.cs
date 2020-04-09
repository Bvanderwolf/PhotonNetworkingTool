public interface IDeveloperCallbacks
{
    void OnConnected();

    void OnDisconnected();

    void OnPlayerNickNameUpdate(string newNickname);

    void OnLobbyStatisticsUpdate();

    void OnConnectedToMaster();

    void OnJoinedRoom();
}