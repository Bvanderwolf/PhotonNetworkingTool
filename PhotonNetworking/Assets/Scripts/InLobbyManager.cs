using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InLobbyManager : MonoBehaviour, ILobbyCallbacks
{
    public static InLobbyManager Instance { get; private set; }

    private List<ILobbyCallbacks> m_CallbackTargets = null;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public void AddCallbackTarget(ILobbyCallbacks target)
    {
        if (m_CallbackTargets == null)
            m_CallbackTargets = new List<ILobbyCallbacks>();

        m_CallbackTargets.Add(target);
    }

    public void RemoveCallbackTarget(ILobbyCallbacks target)
    {
        if (m_CallbackTargets != null)
        {
            m_CallbackTargets.Remove(target);

            if (m_CallbackTargets.Count == 0)
                m_CallbackTargets = null;
        }
    }

    public void LeaveLobby()
    {
        if (PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();
    }

    public void CreateRoom(CreateRoomFormResult result)
    {
        if (!PhotonNetwork.IsConnectedAndReady)
            return;

        var options = new RoomOptions()
        {
            MaxPlayers = (byte)result.MaxPlayers,
            IsVisible = result.IsVisible,
            IsOpen = result.IsOpen
        };

        PhotonNetwork.CreateRoom(result.Name, options, TypedLobby.Default);
    }

    public void JoinRoom(RoomInfo info)
    {
        if (PhotonNetwork.IsConnectedAndReady)
            PhotonNetwork.JoinRoom(info.Name);
    }

    public void OnJoinedLobby()
    {
        if (m_CallbackTargets == null)
            return;

        foreach (var target in m_CallbackTargets)
            target.OnJoinedLobby();
    }

    public void OnLeftLobby()
    {
        if (m_CallbackTargets == null)
            return;

        foreach (var target in m_CallbackTargets)
            target.OnLeftLobby();
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        if (m_CallbackTargets == null)
            return;

        foreach (var target in m_CallbackTargets)
            target.OnLobbyStatisticsUpdate(lobbyStatistics);
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (m_CallbackTargets == null)
            return;

        foreach (var target in m_CallbackTargets)
            target.OnRoomListUpdate(roomList);
    }
}