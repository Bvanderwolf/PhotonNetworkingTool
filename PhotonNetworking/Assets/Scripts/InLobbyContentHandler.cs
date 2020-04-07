using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InLobbyContentHandler : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_RoomListContent;

    [SerializeField]
    private RectTransform m_CreateRoomContent;

    [SerializeField]
    private GameObject m_NoRoomsFeedback;

    public event Action ContentOpened;

    public event Action ContentClosed;

    public event Action<RoomInfo> RoomItemJoinButtonClick;

    public event Action<CreateRoomFormResult> CreateRoomFormTurnedIn;

    public const int ROOM_ITEM_AMOUNT = 7;
    private const int MIN_ROOM_CHAR_AMOUNT = 3;
    private const int MAX_PLAYERS_AMMOUNT = 20;

    private struct RoomItem
    {
        public Text Name;
        public Text PlayerCount;
        public Button JoinButton;
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
    private Dictionary<string, int> m_LobbyRoomCounts;

    private CreateRoomTools m_CreateRoomTools;

    private bool m_NoRooms = false;

    public void Init()
    {
        m_RoomItems = new RoomItem[ROOM_ITEM_AMOUNT];
        m_LobbyRoomCounts = new Dictionary<string, int>();

        for (int i = 0; i < m_RoomItems.Length; i++)
        {
            var roomItem = m_RoomListContent.GetChild(i);
            m_RoomItems[i].Name = roomItem.Find("Name").GetComponent<Text>();
            m_RoomItems[i].PlayerCount = roomItem.Find("PlayerCount").GetComponent<Text>();
            m_RoomItems[i].JoinButton = roomItem.Find("JoinButton").GetComponent<Button>();
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
        if (maxPlayers < 0 || maxPlayers > MAX_PLAYERS_AMMOUNT)
        {
            m_CreateRoomTools.MaxPlayers.text = "";
            m_CreateRoomTools.Feedback.text = "Max Players can be between 0 and " + MAX_PLAYERS_AMMOUNT;
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

    private void UpdateLobbyRoomCount(string lobbyName, int count)
    {
        if (m_LobbyRoomCounts.ContainsKey(lobbyName))
            m_LobbyRoomCounts[lobbyName] = count;
        else
            m_LobbyRoomCounts.Add(lobbyName, count);
    }

    public void OnContentOpened()
    {
        ContentOpened?.Invoke();
    }

    public void OnContentClosed()
    {
        ContentClosed?.Invoke();
    }

    public void SetActiveStateOfRoomListContent(bool active)
    {
        var lobbyIn = PhotonNetwork.CurrentLobby.Name;
        var iterateCount = active ? (m_LobbyRoomCounts.ContainsKey(lobbyIn) ? m_LobbyRoomCounts[lobbyIn] : 0) : ROOM_ITEM_AMOUNT;

        for (int ci = 0; ci < iterateCount; ci++)
            m_RoomListContent.GetChild(ci).gameObject.SetActive(active);

        m_NoRoomsFeedback.SetActive(active ? m_NoRooms : false);
    }

    public void SetActiveStateOfCreateRoomContent(bool value)
    {
        m_CreateRoomContent.gameObject.SetActive(value);
        if (!value)
            CleanCreateRoomFeedBackText();
    }

    public void SetNoRoomsFeedback(bool value, bool canShow)
    {
        m_NoRooms = value;
        if (canShow)
            m_NoRoomsFeedback.SetActive(m_NoRooms);
    }

    public void UpdateRoomListContent(List<RoomInfo> roomList, bool canShowNoRoomsFeedback)
    {
        if (roomList.Count > ROOM_ITEM_AMOUNT)
        {
            Debug.LogError("Wont update room list content :: room list count exceeds maximum of " + ROOM_ITEM_AMOUNT);
            return;
        }

        //maxplayers == playerCount means room is full
        Func<RoomInfo, bool> hasMaxPlayers = (info) => info.PlayerCount == info.MaxPlayers;
        //maxplayers == 0 means room is being removed
        Func<RoomInfo, bool> isBeingRemoved = (info) => info.MaxPlayers == 0;
        //isOpen = false or isVisible = false
        Func<RoomInfo, bool> closedOrInvisible = (info) => !info.IsVisible || !info.IsOpen;
        //keep track of room items being set inactive
        var inActiveCount = 0;
        var roomListCount = roomList.Count;
        for (int i = 0; i < roomList.Count; i++)
        {
            var listItem = roomList[i];
            if (isBeingRemoved(listItem) || closedOrInvisible(listItem))
            {
                /*if an item is not supposed to be shown, set it inactive, increase inactive count
                 and, by removing this list item and decreasing i, re-iterate on the next list item*/
                m_RoomListContent.GetChild(i).gameObject.SetActive(false);
                roomList.Remove(listItem);
                i--;
                inActiveCount++;
                continue;
            }

            m_RoomItems[i].Name.text = listItem.Name;
            m_RoomItems[i].PlayerCount.text = $"({listItem.PlayerCount}/{listItem.MaxPlayers})";
            m_RoomItems[i].JoinButton.onClick.RemoveAllListeners();
            m_RoomItems[i].JoinButton.onClick.AddListener(() => RoomItemJoinButtonClick(listItem));
            m_RoomItems[i].JoinButton.interactable = !hasMaxPlayers(listItem);
            m_RoomListContent.GetChild(i).gameObject.SetActive(true);
        }
        //true room count is defined by subtracting items being removed or closedOrInvisble from roomlist count
        var trueRoomCount = roomListCount - inActiveCount;
        //set no rooms feedback based on the ammount of true rooms listed
        var noRoomsListed = trueRoomCount == 0;
        SetNoRoomsFeedback(noRoomsListed, canShowNoRoomsFeedback);
        //store true room count as our room count for this lobby
        UpdateLobbyRoomCount(PhotonNetwork.CurrentLobby.Name, trueRoomCount);
    }
}