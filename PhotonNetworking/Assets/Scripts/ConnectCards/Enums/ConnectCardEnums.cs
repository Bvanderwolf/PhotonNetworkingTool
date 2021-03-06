﻿namespace ConnectCards.Enums
{
    public enum ConnectCard
    {
        StartConnect,
        ConnectStatus,
        Disconnect,
        ConnectToLobby,
        InLobby,
        InRoom
    }

    public enum InLobbyConnectChoice
    {
        Joining,
        Creating,
        Leaving
    }

    public enum InRoomStatus
    {
        Inactive,
        InPlayerlist,
        Chatting,
        Ready
    }

    public enum ChatBotMessages
    {
        PlayerLeft,
        PlayerJoined,
        NewMasterClient,
        IsReady,
        StoppedCountdown
    }
}