namespace Singletons
{
    using ConnectCards;
    using ExitGames.Client.Photon;
    using Photon.Pun;
    using Photon.Realtime;
    using UnityEngine;

    public class InRoomManager : MonoBehaviour, IInRoomCallbacks
    {
        public static InRoomManager Instance { get; private set; }

        private IConnectCardCallbacks m_ConnectCardTarget = null;

        // ---According to photon, when using their cloud service, 20 ccu is maximum, this can be put to this value ---
        public const int MAX_PLAYERS_AMMOUNT = 8;

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(this.gameObject);
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void AddCallbackTarget(object target)
        {
            if (target as IConnectCardCallbacks != null)
            {
                if (m_ConnectCardTarget == null)
                    m_ConnectCardTarget = (IConnectCardCallbacks)target;
            }
        }

        public void RemoveCallbackTarget(object target)
        {
            if (target as IConnectCardCallbacks != null)
            {
                if (m_ConnectCardTarget != null)
                    m_ConnectCardTarget = null;
            }
        }

        public void LeaveRoom()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                //*Note: Becoming inactive is something is a design choice for which i now choose false
                //playerTTL is also set to 0 at creation of a room to prevent this
                PhotonNetwork.LeaveRoom(false);
            }
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            m_ConnectCardTarget.OnMasterClientSwitched(newMasterClient);
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            m_ConnectCardTarget.OnPlayerEnteredRoom(newPlayer);
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            m_ConnectCardTarget.OnPlayerLeftRoom(otherPlayer);
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }
    }
}