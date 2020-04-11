namespace ConnectCards
{
    using HelperStructs;
    using Photon.Pun;
    using Photon.Realtime;
    using Singletons;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class InLobbyContentHandler : ContentHandler
    {
        [SerializeField]
        private RectTransform m_RoomListContent;

        [SerializeField]
        private RectTransform m_CreateRoomContent;

        [SerializeField]
        private GameObject m_NoRoomsFeedback;

        public event Action<RoomInfo> RoomItemJoinButtonClick;

        public event Action<CreateRoomFormResult> CreateRoomFormTurnedIn;

        public const int ROOM_ITEM_AMOUNT = 7;
        private const int MIN_ROOM_CHAR_AMOUNT = 3;

        private struct RoomItem
        {
            public Text Name;
            public Text PlayerCount;
            public Button JoinButton;
            public GameObject GO;
        }

        private struct CreateRoomTools
        {
            public InputField RoomName;
            public InputField MaxPlayers;
            public Toggle IsVisible;
            public Toggle IsOpen;
            public Button CreateButton;
            public Text Feedback;
        }

        private RoomItem[] m_RoomItems;
        private int m_RoomCountSinceLastUpdate = 0;
        private Dictionary<string, List<RoomInfo>> m_LobbyRooms;

        private CreateRoomTools m_CreateRoomTools;

        public override void Init()
        {
            base.Init();

            m_RoomItems = new RoomItem[ROOM_ITEM_AMOUNT];
            m_LobbyRooms = new Dictionary<string, List<RoomInfo>>();

            for (int i = 0; i < m_RoomItems.Length; i++)
            {
                var roomItem = m_RoomListContent.GetChild(i);
                m_RoomItems[i].Name = roomItem.Find("Name").GetComponent<Text>();
                m_RoomItems[i].PlayerCount = roomItem.Find("PlayerCount").GetComponent<Text>();
                m_RoomItems[i].JoinButton = roomItem.Find("JoinButton").GetComponent<Button>();
                m_RoomItems[i].GO = roomItem.gameObject;
            }

            m_CreateRoomTools = new CreateRoomTools();
            m_CreateRoomTools.RoomName = m_CreateRoomContent.Find("RoomNameInput").GetComponent<InputField>();
            m_CreateRoomTools.MaxPlayers = m_CreateRoomContent.Find("MaxPlayerInput").GetComponent<InputField>();
            m_CreateRoomTools.IsVisible = m_CreateRoomContent.Find("IsVisibleToggle").GetComponent<Toggle>();
            m_CreateRoomTools.IsOpen = m_CreateRoomContent.Find("IsOpenToggle").GetComponent<Toggle>();
            m_CreateRoomTools.CreateButton = m_CreateRoomContent.Find("CreateButton").GetComponent<Button>();
            m_CreateRoomTools.Feedback = m_CreateRoomContent.Find("FeedBack").GetComponent<Text>();
            m_CreateRoomTools.CreateButton.onClick.AddListener(OnCreateRoomFormTurnIn);
        }

        private void OnCreateRoomFormTurnIn()
        {
            var name = m_CreateRoomTools.RoomName.text;
            if (name.Length <= MIN_ROOM_CHAR_AMOUNT)
            {
                m_CreateRoomTools.RoomName.text = "";
                m_CreateRoomTools.Feedback.text = "Need atleast " + MIN_ROOM_CHAR_AMOUNT + " characters";
                return;
            }

            var isFilledIn = m_CreateRoomTools.MaxPlayers.text != "";
            if (!isFilledIn)
            {
                m_CreateRoomTools.Feedback.text = "Max Players is not filled in";
                return;
            }

            var maxPlayers = int.Parse(m_CreateRoomTools.MaxPlayers.text);
            if (maxPlayers <= 1 || maxPlayers > InRoomManager.MAX_PLAYERS_AMMOUNT)
            {
                m_CreateRoomTools.MaxPlayers.text = "";
                m_CreateRoomTools.Feedback.text = "Max Players can be between 1 and " + InRoomManager.MAX_PLAYERS_AMMOUNT + 1;
                return;
            }

            CreateRoomFormTurnedIn(new CreateRoomFormResult()
            {
                Name = name,
                MaxPlayers = maxPlayers,
                IsVisible = m_CreateRoomTools.IsVisible.isOn,
                IsOpen = m_CreateRoomTools.IsOpen.isOn
            });
        }

        public void CleanCreateRoomFeedBackText()
        {
            m_CreateRoomTools.Feedback.text = "";
        }

        private void AddToLobbyRooms(string lobbyName, RoomInfo room)
        {
            if (m_LobbyRooms.ContainsKey(lobbyName))
                m_LobbyRooms[lobbyName].Add(room);
        }

        private void RemoveFromLobbyRooms(string lobbyName, RoomInfo room)
        {
            if (m_LobbyRooms.ContainsKey(lobbyName))
                m_LobbyRooms[lobbyName].Remove(room);
        }

        public void SetActiveStateOfRoomListContent(bool active)
        {
            var rooms = m_LobbyRooms[PhotonNetwork.CurrentLobby.Name];
            if (!active)
            {
                foreach (var item in m_RoomItems)
                    item.GO.SetActive(false);
            }
            else
            {
                for (int ri = 0; ri < rooms.Count; ri++)
                {
                    var room = rooms[ri];
                    m_RoomItems[ri].Name.text = room.Name;
                    m_RoomItems[ri].PlayerCount.text = $"({room.PlayerCount}/{room.MaxPlayers})";
                    bool isFull = room.PlayerCount == room.MaxPlayers;
                    if (isFull)
                    {
                        m_RoomItems[ri].JoinButton.interactable = false;
                    }
                    else
                    {
                        m_RoomItems[ri].JoinButton.onClick.RemoveAllListeners();
                        m_RoomItems[ri].JoinButton.onClick.AddListener(() => RoomItemJoinButtonClick(room));
                        m_RoomItems[ri].JoinButton.interactable = true;
                    }
                    m_RoomItems[ri].GO.SetActive(true);
                }
            }

            var noRooms = rooms.Count == 0;
            SetNoRoomsFeedback(active ? noRooms : false);
        }

        public void SetActiveStateOfCreateRoomContent(bool value)
        {
            m_CreateRoomContent.gameObject.SetActive(value);
            if (!value)
                CleanCreateRoomFeedBackText();
        }

        public void SetNoRoomsFeedback(bool value)
        {
            m_NoRoomsFeedback.SetActive(value);
        }

        public void UpdateLobbyRoomsWithLobbyStatistics(List<TypedLobbyInfo> statistics)
        {
            foreach (var lobbyStat in statistics)
            {
                if (!m_LobbyRooms.ContainsKey(lobbyStat.Name))
                {
                    m_LobbyRooms.Add(lobbyStat.Name, new List<RoomInfo>());
                }
            }
        }

        public void UpdateRoomListContent(List<RoomInfo> roomList, bool inRoomListUpdate)
        {
            if (roomList.Count > ROOM_ITEM_AMOUNT)
            {
                Debug.LogError("Wont update room list content :: room list count exceeds maximum of " + ROOM_ITEM_AMOUNT);
                return;
            }

            //remove rooms from the list that are being removed, closed or invisible
            var lobbyName = PhotonNetwork.CurrentLobby.Name;
            foreach (var room in roomList)
            {
                if (room.RemovedFromList)
                {
                    RemoveFromLobbyRooms(lobbyName, room);
                }
                else
                {
                    AddToLobbyRooms(lobbyName, room);
                }
            }

            if (inRoomListUpdate)
            {
                SetActiveStateOfRoomListContent(false);
                SetActiveStateOfRoomListContent(true);
            }
        }
    }
}