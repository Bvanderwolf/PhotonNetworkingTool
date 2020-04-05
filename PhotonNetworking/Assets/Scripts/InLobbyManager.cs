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

    public void CreateRoom(string name, RoomOptions options)
    {        
        if (PhotonNetwork.IsConnectedAndReady)
            PhotonNetwork.CreateRoom(name, options, TypedLobby.Default);
    }

    public void JoinRoom(string name)
    {
        if (PhotonNetwork.IsConnectedAndReady)
            PhotonNetwork.JoinRoom(name);
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
