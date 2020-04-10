namespace Singletons
{
    using ExitGames.Client.Photon;
    using Photon.Pun;
    using Photon.Realtime;
    using UnityEngine;

    public class InRoomManager : MonoBehaviour, IInRoomCallbacks
    {
        public static InRoomManager Instance { get; private set; }

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

        public void OnMasterClientSwitched(Player newMasterClient)
        {
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }
    }
}