namespace Singletons
{
    using ConnectCards;
    using ExitGames.Client.Photon;
    using Photon.Pun;
    using Photon.Realtime;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    public class InRoomManager : MonoBehaviour, IInRoomCallbacks
    {
        public static InRoomManager Instance { get; private set; }

        private IConnectCardCallbacks m_ConnectCardTarget = null;

        // ---According to photon, when using their cloud service, 20 ccu is maximum, this can be put to this value ---
        public const int MAX_PLAYERS_AMMOUNT = 8;

        public bool AutoMaticallySyncScene { get; private set; }

        public bool IsFull
        {
            get
            {
                var room = PhotonNetwork.CurrentRoom;
                return room != null && room.PlayerCount == room.MaxPlayers;
            }
        }

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(this.gameObject);
        }

        public void Init(bool autoSyncScene)
        {
            AutoMaticallySyncScene = autoSyncScene;
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

        public void TryLoadScene(int buildIndex, UnityAction<Scene, LoadSceneMode> onLoad)
        {
            if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError("Wont load scene :: Build Index " + buildIndex + " out of bounds");
                return;
            }

            SceneManager.sceneLoaded += onLoad;

            if (AutoMaticallySyncScene && !PhotonNetwork.IsMasterClient)
                return;

            PhotonNetwork.LoadLevel(buildIndex);
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
            m_ConnectCardTarget.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }
    }
}