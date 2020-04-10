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

    public class ConnectCardHandler : MonoBehaviour, IConnectionCallbacks, ILobbyCallbacks
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

            InitCards();
            EnableConnectCard(ConnectCard.StartConnect);

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
            //get connection callbacks
            ConnectionManager.Instance.AddCallbackTarget(this);
            InLobbyManager.Instance.AddCallbackTarget(this);
        }

        private void OnDestroy()
        {
            //unsubscribe from getting callbacks
            ConnectionManager.Instance.RemoveCallbackTarget(this);
            InLobbyManager.Instance.AddCallbackTarget(this);
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
                    ConnectionManager.Instance.ConnectToMaster((string)args);
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
                    ConnectionManager.Instance.ConnectToLobby((string)args);
                    break;

                case ConnectCard.InLobby:
                    HandleInLobbyConnectResult((InLobbyConnectResult)args);
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

        public void OnConnected()
        {
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

        public void OnConnectedToMaster()
        {
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
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
    }
}