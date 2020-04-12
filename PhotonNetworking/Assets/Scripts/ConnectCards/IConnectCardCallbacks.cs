namespace ConnectCards
{
    using ExitGames.Client.Photon;
    using Photon.Realtime;
    using System.Collections.Generic;

    public interface IConnectCardCallbacks
    {
        void OnDisconnected(DisconnectCause cause);

        void OnJoinedLobby();

        void OnLeftLobby();

        void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics);

        void OnRoomListUpdate(List<RoomInfo> roomList);

        void OnPlayerLeftRoom(Player otherPlayer);

        void OnPlayerEnteredRoom(Player newPlayer);

        void OnMasterClientSwitched(Player newMasterClient);

        void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps);
    }
}