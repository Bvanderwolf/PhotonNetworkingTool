using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public enum ConnectTarget { MasterDefault, MasterReconnect, Lobby, Room }

public class ConnectionManager : MonoBehaviour, IConnectionCallbacks
{
    public static ConnectionManager Instance { get; private set; }

    private List<IConnectionCallbacks> m_CallbackTargets = null;

    private bool m_AutomaticallySyncScene;

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

        DontDestroyOnLoad(this);
    }

    public void Init(bool autoSyncScene)
    {
        m_AutomaticallySyncScene = autoSyncScene;
    }

    public void AddCallbackTarget(IConnectionCallbacks target)
    {
        if (m_CallbackTargets == null)
            m_CallbackTargets = new List<IConnectionCallbacks>();

        m_CallbackTargets.Add(target);
    }

    public void RemoveCallbackTarget(IConnectionCallbacks target)
    {
        if(m_CallbackTargets != null)
        {
            m_CallbackTargets.Remove(target);

            if (m_CallbackTargets.Count == 0)
                m_CallbackTargets = null;
        }
    }

    public void OnConnected()
    {
        if (m_CallbackTargets == null)
            return;

        foreach (var target in m_CallbackTargets)
            target.OnConnected();
    }

    public void ConnectToMaster(string nickname)
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.NickName = nickname;
            PhotonNetwork.AutomaticallySyncScene = m_AutomaticallySyncScene;
            PhotonNetwork.ConnectUsingSettings();
        }              
    }

    public void ConnectToLobby(string name)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.InLobby)
        {
            var lobby = new TypedLobby(name, LobbyType.Default);
            PhotonNetwork.JoinLobby(lobby);
        }             
    }

    public void ReconnectToMaster()
    {
        if(!PhotonNetwork.IsConnected)
            PhotonNetwork.Reconnect();
    }

    public void OnConnectedToMaster()
    {
        if (m_CallbackTargets == null)
            return;

        foreach (var target in m_CallbackTargets)
            target.OnConnectedToMaster();
    }

    public void OnDisconnected(DisconnectCause cause)
    {        
        Debug.Log($"disconnected with cause {cause}");

        if (m_CallbackTargets == null)
            return;

        foreach (var target in m_CallbackTargets)
            target.OnDisconnected(cause);
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        if (m_CallbackTargets == null)
            return;

        foreach (var target in m_CallbackTargets)
            target.OnRegionListReceived(regionHandler);
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        if (m_CallbackTargets == null)
            return;

        foreach (var target in m_CallbackTargets)
            target.OnCustomAuthenticationResponse(data);
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        if (m_CallbackTargets == null)
            return;

        foreach (var target in m_CallbackTargets)
            target.OnCustomAuthenticationFailed(debugMessage);
    }
}
