using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public enum ConnectTarget { MasterDefault, MasterReconnect, Room }

public class ConnectionManager : MonoBehaviour, IConnectionCallbacks
{
    public static ConnectionManager Instance { get; private set; }

    private List<IConnectionCallbacks> m_CallbackTargets = null;

    private IDeveloperCallbacks m_DeveloperTarget = null;

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

    public void AddCallbackTarget(object target)
    {
        if (target as IConnectionCallbacks != null)
        {
            if (m_CallbackTargets == null)
                m_CallbackTargets = new List<IConnectionCallbacks>();

            m_CallbackTargets.Add((IConnectionCallbacks)target);
        }
        else if (target as IDeveloperCallbacks != null)
        {
            if (m_DeveloperTarget == null)
                m_DeveloperTarget = (IDeveloperCallbacks)target;
        }
    }

    public void RemoveCallbackTarget(object target)
    {
        if (target as IConnectionCallbacks != null)
        {
            if (m_CallbackTargets != null)
            {
                m_CallbackTargets.Remove((IConnectionCallbacks)target);

                if (m_CallbackTargets.Count == 0)
                    m_CallbackTargets = null;
            }
        }
        else if (target as IDeveloperCallbacks != null)
        {
            if (m_DeveloperTarget != null)
                m_DeveloperTarget = null;
        }
    }

    public void OnConnected()
    {
        m_DeveloperTarget.OnConnected();

        if (m_CallbackTargets == null)
            return;

        foreach (var target in m_CallbackTargets)
            target.OnConnected();
    }

    public void ConnectToMaster(string nickname)
    {
        if (!PhotonNetwork.IsConnected)
        {
            var hasGenericNickname = nickname == StartConnectCardHandler.GENERIC_CONNECT_NAME;
            PlayerManager.Instance.UpdateProperties<bool>("GenericNickname", hasGenericNickname);
            PlayerManager.Instance.UpdateNickname(nickname);

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
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.Reconnect();
    }

    public void OnConnectedToMaster()
    {
        m_DeveloperTarget.OnConnectedToMaster();

        if (m_CallbackTargets == null)
            return;

        foreach (var target in m_CallbackTargets)
            target.OnConnectedToMaster();
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        m_DeveloperTarget?.OnDisconnected();

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