namespace Singletons
{
    using Utils;
    using System.Collections.Generic;
    using Photon.Pun;
    using Photon.Realtime;
    using UnityEngine;
    using ConnectCards;

    public class ConnectionManager : MonoBehaviour, IConnectionCallbacks
    {
        public static ConnectionManager Instance { get; private set; }

        private IDeveloperCallbacks m_DeveloperTarget = null;
        private IConnectCardCallbacks m_ConnectCardTarget = null;

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

        public void AddCallbackTarget(object target)
        {
            if (target as IConnectCardCallbacks != null)
            {
                if (m_ConnectCardTarget == null)
                    m_ConnectCardTarget = (IConnectCardCallbacks)target;
            }
            else if (target as IDeveloperCallbacks != null)
            {
                if (m_DeveloperTarget == null)
                    m_DeveloperTarget = (IDeveloperCallbacks)target;
            }
        }

        public void RemoveCallbackTarget(object target)
        {
            if (target as IConnectCardCallbacks != null)
            {
                if (m_ConnectCardTarget != null)
                    m_ConnectCardTarget = null;
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
        }

        public void ConnectToMaster(string nickname)
        {
            if (!PhotonNetwork.IsConnected)
            {
                PlayerManager.Instance.SetHasGenericNickname(nickname);
                PlayerManager.Instance.UpdateNickname(nickname);

                PhotonNetwork.AutomaticallySyncScene = InRoomManager.Instance.AutoMaticallySyncScene;
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
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            m_DeveloperTarget?.OnDisconnected();
            m_ConnectCardTarget?.OnDisconnected(cause);
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
}