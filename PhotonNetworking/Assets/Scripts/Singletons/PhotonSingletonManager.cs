﻿namespace Singletons
{
    using UnityEngine;

    public class PhotonSingletonManager : MonoBehaviour
    {
        [SerializeField, Tooltip("Set to true if you want players to sync scene with the master client")]
        private bool m_AutomaticallySyncScene;

        private void Awake()
        {
            if (ConnectionManager.Instance == null)
            {
                new GameObject("ConnectionManager", typeof(ConnectionManager))
                    .GetComponent<ConnectionManager>()
                    .Init(m_AutomaticallySyncScene);
            }

            if (InLobbyManager.Instance == null)
                new GameObject("InLobbyManager", typeof(InLobbyManager));

            if (PlayerManager.Instance == null)
                new GameObject("PlayerManager", typeof(PlayerManager));

            if (InRoomManager.Instance == null)
                new GameObject("InRoomManager", typeof(InRoomManager));

            if (MatchmakingManager.Instance == null)
                new GameObject("MatchmakingManager", typeof(MatchmakingManager));

            Destroy(this.gameObject);
        }
    }
}