using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public enum ConnectTarget { MASTER, LOBBY, ROOM }

public class ConnectionManager : MonoBehaviour, IConnectionCallbacks
{
    public static ConnectionManager Instance { get; private set; }

    private List<IConnectionCallbacks> m_CallbackTargets = null;

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

    public void AddCallbackTarget(IConnectionCallbacks target)
    {
        if (m_CallbackTargets == null)
            m_CallbackTargets = new List<IConnectionCallbacks>();

        m_CallbackTargets.Add(target);
    }

    public void OnConnected()
    {
       //todo: implementation
    }

    public void ConnectToMaster(string nickname)
    {
        PhotonNetwork.NickName = nickname;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnConnectedToMaster()
    {
        //todo: implementation
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        //todo: implementation (reset connectCard handler through callback targets)
        Debug.Log($"disconnected with cause {cause}");
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }
}
