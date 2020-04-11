namespace Singletons
{
    using Utils;
    using System.Collections.Generic;
    using Photon.Pun;
    using Photon.Realtime;
    using UnityEngine;
    using ConnectCards.HelperStructs;
    using ConnectCards;

    public class InLobbyManager : MonoBehaviour, ILobbyCallbacks
    {
        public static InLobbyManager Instance { get; private set; }

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

            DontDestroyOnLoad(this.gameObject);
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
            m_ConnectCardTarget.OnJoinedLobby();
        }

        public void OnLeftLobby()
        {
            m_ConnectCardTarget.OnLeftLobby();
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            m_DeveloperTarget.OnLobbyStatisticsUpdate();
            m_ConnectCardTarget.OnLobbyStatisticsUpdate(lobbyStatistics);
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            m_ConnectCardTarget.OnRoomListUpdate(roomList);
        }
    }
}