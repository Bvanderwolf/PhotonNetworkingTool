namespace ConnectCards
{
    using Singletons;
    using Singletons.Enums;
    using Photon.Realtime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Enums;
    using HelperStructs;
    using ExitGames.Client.Photon;

    public class ConnectCardHandler : MonoBehaviour, IConnectCardCallbacks
    {
        [SerializeField, Tooltip("Check this flag, if you have multiple scenes for your game")]
        private bool m_DontDestroyOnLoad;

        [SerializeField]
        private GameObject[] m_cards;

        private Dictionary<ConnectCard, GameObject> m_cardsDict = new Dictionary<ConnectCard, GameObject>();
        private GameObject m_ActiveCardGO = null;

        private void Awake()
        {
            var cards = Enum.GetValues(typeof(ConnectCard)).Cast<ConnectCard>();
            foreach (var card in m_cards)
            {
                var key = cards.Where(c => card.name.Contains(c.ToString())).First();
                m_cardsDict.Add(key, card);
            }

            if (m_DontDestroyOnLoad)
                DontDestroyOnLoad(this.gameObject);
        }

        private void InitCards()
        {
            foreach (var card in m_cards)
            {
                var cardAbstract = card.GetComponent<ConnectCardAbstract>();
                cardAbstract.Init();
            }
        }

        private void Start()
        {
            //initialize all cards and enable the starting card (startconnect)
            InitCards();
            EnableConnectCard(ConnectCard.StartConnect);

            //get connection callbacks
            ConnectionManager.Instance.AddCallbackTarget(this);
            InLobbyManager.Instance.AddCallbackTarget(this);
            InRoomManager.Instance.AddCallbackTarget(this);
        }

        private void OnDestroy()
        {
            //unsubscribe from getting callbacks
            ConnectionManager.Instance.RemoveCallbackTarget(this);
            InLobbyManager.Instance.RemoveCallbackTarget(this);
            InRoomManager.Instance.AddCallbackTarget(this);
        }

        /// <summary>
        /// Enables given connect card, and sets it up
        /// </summary>
        /// <param name="card"></param>
        private void EnableConnectCard(ConnectCard card)
        {
            var cardGameObject = m_cardsDict[card];
            cardGameObject.SetActive(true);
            cardGameObject.GetComponent<ConnectCardAbstract>().m_TaskFinished += OnConnectCardFinishedTask;
            m_ActiveCardGO = cardGameObject;
        }

        /// <summary>
        /// Disables given connect card, unsubscribing the task listener
        /// among other things
        /// </summary>
        /// <param name="card"></param>
        private void DisableConnectCard(ConnectCard card)
        {
            var cardGameObject = m_cardsDict[card];
            cardGameObject.SetActive(false);
            cardGameObject.GetComponent<ConnectCardAbstract>().m_TaskFinished -= OnConnectCardFinishedTask;
        }

        private void OnConnectCardFinishedTask(GameObject cardGameObject, object args)
        {
            var card = m_cardsDict.Where(pair => pair.Value == cardGameObject).First().Key;
            switch (card)
            {
                case ConnectCard.StartConnect:
                    ReplaceCard(ConnectCard.StartConnect, ConnectCard.ConnectStatus);
                    SetupConnectStatusCard(ConnectTarget.ConnectingToMaster);
                    string nickName = (string)args;
                    ConnectionManager.Instance.ConnectToMaster(nickName);
                    break;

                case ConnectCard.ConnectStatus:
                    HandleConnectStatusTargetReached((ConnectTarget)args);
                    break;

                case ConnectCard.Disconnect:
                    ReplaceCard(ConnectCard.Disconnect, ConnectCard.ConnectStatus);
                    SetupConnectStatusCard(ConnectTarget.ReconnectingToMaster);
                    ConnectionManager.Instance.ReconnectToMaster();
                    break;

                case ConnectCard.ConnectToLobby:
                    DisableConnectCard(ConnectCard.ConnectToLobby);
                    string lobbyName = (string)args;
                    ConnectionManager.Instance.ConnectToLobby(lobbyName);
                    break;

                case ConnectCard.InLobby:
                    HandleInLobbyConnectResult((InLobbyConnectResult)args);
                    break;

                case ConnectCard.InRoom:
                    bool succes = (bool)args;
                    HandleInRoomConnectResult(succes);
                    break;
            }
        }

        private void SetupConnectStatusCard(ConnectTarget target)
        {
            var card = m_ActiveCardGO.GetComponent<ConnectStatusCardHandler>();
            if (card == null)
            {
                Debug.LogError("Wont setup connect status card :: card is not of right type");
                return;
            }
            card.StartLoadWithTarget(target);
        }

        private void ProvideDisconnectCardWithCause(DisconnectCause cause)
        {
            var card = m_ActiveCardGO.GetComponent<DisconnectCardHandler>();
            if (card == null)
            {
                Debug.LogError("Wont setup connect status card :: card is not of right type");
                return;
            }
            card.SetCause(cause.ToString());
        }

        private void HandleInLobbyConnectResult(InLobbyConnectResult result)
        {
            switch (result.choice)
            {
                case InLobbyConnectChoice.Joining:
                    ReplaceCard(ConnectCard.InLobby, ConnectCard.ConnectStatus);
                    SetupConnectStatusCard(ConnectTarget.JoiningRoom);
                    InLobbyManager.Instance.JoinRoom((RoomInfo)result.args);
                    break;

                case InLobbyConnectChoice.Creating:
                    ReplaceCard(ConnectCard.InLobby, ConnectCard.ConnectStatus);
                    SetupConnectStatusCard(ConnectTarget.CreatingRoom);
                    InLobbyManager.Instance.CreateRoom((CreateRoomFormResult)result.args);
                    break;

                case InLobbyConnectChoice.Leaving:
                    DisableConnectCard(ConnectCard.InLobby);
                    InLobbyManager.Instance.LeaveLobby();
                    break;
            }
        }

        private void HandleInRoomConnectResult(bool succes)
        {
            if (succes)
            {
                Debug.LogError("game can now be loaded");
                DisableConnectCard(ConnectCard.InRoom);
            }
            else
            {
                ReplaceCard(ConnectCard.InRoom, ConnectCard.ConnectStatus);
                SetupConnectStatusCard(ConnectTarget.LeavingRoom);
                InRoomManager.Instance.LeaveRoom();
            }
        }

        private void HandleConnectStatusTargetReached(ConnectTarget target)
        {
            switch (target)
            {
                case ConnectTarget.ConnectingToMaster:
                    ReplaceCard(ConnectCard.ConnectStatus, ConnectCard.ConnectToLobby);
                    break;

                case ConnectTarget.ReconnectingToMaster:
                    ReplaceCard(ConnectCard.ConnectStatus, ConnectCard.ConnectToLobby);
                    break;

                case ConnectTarget.JoiningRoom:
                    ReplaceCard(ConnectCard.ConnectStatus, ConnectCard.InRoom);
                    break;

                case ConnectTarget.CreatingRoom:
                    ReplaceCard(ConnectCard.ConnectStatus, ConnectCard.InRoom);
                    break;

                case ConnectTarget.LeavingRoom:
                    ReplaceCard(ConnectCard.ConnectStatus, ConnectCard.InLobby);
                    break;
            }
        }

        /// <summary>
        /// Tries replacing old card with new card
        /// </summary>
        /// <param name="oldCard"></param>
        /// <param name="newCard"></param>
        private void ReplaceCard(ConnectCard oldCard, ConnectCard newCard)
        {
            var activeCardCount = m_cardsDict.Count(p => p.Value.activeInHierarchy);
            if (activeCardCount != 1)
            {
                Debug.LogError($"Wont replace card :: active card count {activeCardCount} is not 1");
                return;
            }

            if (m_cardsDict[oldCard] != m_ActiveCardGO)
            {
                var newOldCard = m_cardsDict.Where(p => p.Value == m_ActiveCardGO).First().Key;
                DisableConnectCard(newOldCard);
            }
            else
            {
                DisableConnectCard(oldCard);
            }

            EnableConnectCard(newCard);
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            var activeCardCount = m_cardsDict.Count(p => p.Value.activeInHierarchy);
            if (activeCardCount == 0)
            {
                //if there active no active cars, just enable the disconnect card
                EnableConnectCard(ConnectCard.Disconnect);
            }
            else if (activeCardCount == 1)
            {
                //if there is an active card, replace that one
                var activeCard = m_cardsDict.Where(p => p.Value == m_ActiveCardGO).First().Key;
                ReplaceCard(activeCard, ConnectCard.Disconnect);
            }
            else
            {
                Debug.LogError($"Disconnect Card could not be shown :: active card count {activeCardCount} is neither 0 or 1");
                return;
            }
            ProvideDisconnectCardWithCause(cause);
        }

        public void OnJoinedLobby()
        {
            EnableConnectCard(ConnectCard.InLobby);
        }

        public void OnLeftLobby()
        {
            EnableConnectCard(ConnectCard.ConnectToLobby);
        }

        // --- Will Callback on connected to master client and every 60 seconds after that ---
        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            var connectToLobbyCard = m_cardsDict[ConnectCard.ConnectToLobby].GetComponent<ConnectToLobbyCard>();
            if (connectToLobbyCard == null)
            {
                Debug.LogError("Wont update lobby statistics :: card is not in dictionary");
                return;
            }
            connectToLobbyCard.UpdateLobbyStatistics(lobbyStatistics);

            var inLobbyCard = m_cardsDict[ConnectCard.InLobby].GetComponent<InLobbyConnectCardHandler>();
            if (inLobbyCard == null)
            {
                Debug.LogError("Wont update lobby statistics :: card is not in dictionary");
                return;
            }
            inLobbyCard.UpdateLobbyStatsForContentHandler(lobbyStatistics);
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            var card = m_cardsDict[ConnectCard.InLobby].GetComponent<InLobbyConnectCardHandler>();
            if (card == null)
            {
                Debug.LogError("Wont update room list content :: card is not in dictionary");
                return;
            }
            var roomsAvailable = roomList.Count != InLobbyContentHandler.ROOM_ITEM_AMOUNT;
            card.SetEnableStateOfCreateRoomButton(roomsAvailable);
            card.UpdateRoomListContent(roomList);
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            var card = m_cardsDict[ConnectCard.InRoom].GetComponent<InRoomConnectCardHandler>();
            if (card == null)
            {
                Debug.LogError("Wont Update in room connect card :: card is not in dictionary");
                return;
            }
            card.OnPlayerLeftRoom(otherPlayer);
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            var card = m_cardsDict[ConnectCard.InRoom].GetComponent<InRoomConnectCardHandler>();
            if (card == null)
            {
                Debug.LogError("Wont Update in room connect card :: card is not in dictionary");
                return;
            }
            card.OnPlayerEnteredRoom(newPlayer);
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            var card = m_cardsDict[ConnectCard.InRoom].GetComponent<InRoomConnectCardHandler>();
            if (card == null)
            {
                Debug.LogError("Wont Update in room connect card :: card is not in dictionary");
                return;
            }
            card.OnMasterClientChange(newMasterClient);
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(PlayerManager.INROOMSTATUS_KEY))
            {
                var card = m_cardsDict[ConnectCard.InRoom].GetComponent<InRoomConnectCardHandler>();
                if (card == null)
                {
                    Debug.LogError("Wont Update in room connect card :: card is not in dictionary");
                    return;
                }
                card.OnInRoomStatusChange(targetPlayer, (InRoomStatus)changedProps[PlayerManager.INROOMSTATUS_KEY]);
            }
        }
    }
}