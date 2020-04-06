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

    public event Action ContentOpened;

    public event Action ContentClosed;

    public event Action<RoomInfo> RoomItemJoinButtonClick;

    public event Action<RoomOptions> CreateRoomFormTurnedIn;

    public const int ROOM_ITEM_AMOUNT = 7;

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
        m_CreateRoomTools.CreateButton.onClick.AddListener(OnCreateRoomFormTurnIn);
    }

    private void OnCreateRoomFormTurnIn()
    {
        //evaluate form and call turn in event if meeting requirements
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
        var count = value ? m_RoomCountSinceLastUpdate : m_RoomListContent.childCount;

        for (int ci = 0; ci < count; ci++)
            m_RoomListContent.GetChild(ci).gameObject.SetActive(value);
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