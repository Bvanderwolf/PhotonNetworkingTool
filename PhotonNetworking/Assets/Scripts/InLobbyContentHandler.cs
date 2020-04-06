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

    private CreateRoomTools m_CreateRoomTools;

    public void Init()
    {
        m_RoomItems = new RoomItem[ROOM_ITEM_AMOUNT];

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

    public void OnContentOpened()
    {
        ContentOpened?.Invoke();
    }

    public void OnContentClosed()
    {
        ContentClosed?.Invoke();
    }

    public void SetActiveStateOfRoomListContent(bool value)
    {
        var count = value ? m_RoomCountSinceLastUpdate : ROOM_ITEM_AMOUNT;

        for (int ci = 0; ci < count; ci++)
            m_RoomListContent.GetChild(ci).gameObject.SetActive(value);
    }

    public void SetActiveStateOfCreateRoomContent(bool value)
    {
        m_CreateRoomContent.gameObject.SetActive(value);
    }

    public void SetNoRoomsFeedback(bool active)
    {
        m_NoRoomsFeedback.SetActive(active);
    }

    public void UpdateRoomListContent(List<RoomInfo> roomList)
    {
        if (roomList.Count > ROOM_ITEM_AMOUNT)
        {
            Debug.LogError("Wont update room list content :: room list count exceeds maximum of " + ROOM_ITEM_AMOUNT);
            return;
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            var listItem = roomList[i];
            m_RoomItems[i].Name.text = listItem.Name;
            m_RoomItems[i].PlayerCount.text = $"({listItem.PlayerCount}/{listItem.MaxPlayers})";
            m_RoomItems[i].JoinButton.onClick.RemoveAllListeners();
            m_RoomItems[i].JoinButton.onClick.AddListener(() => RoomItemJoinButtonClick(listItem));
        }

        m_RoomCountSinceLastUpdate = roomList.Count;
    }
}