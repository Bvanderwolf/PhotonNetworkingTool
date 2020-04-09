﻿using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InLobbyConnectChoice { Joining, Creating, Leaving }

public struct InLobbyConnectResult
{
    public object args;
    public InLobbyConnectChoice choice;
}

public struct CreateRoomFormResult
{
    public string Name;
    public int MaxPlayers;
    public bool IsVisible;
    public bool IsOpen;
}

public class InLobbyConnectCardHandler : ConnectCardAbstract
{
    [SerializeField]
    private Button m_JoinRoomButton;

    [SerializeField]
    private Button m_CreateRoomButton;

    [SerializeField]
    private Button m_LeaveLobbyButton;

    [SerializeField]
    private GameObject m_Content;

    private Animator m_ContentAnimator;
    private InLobbyContentHandler m_ContentHandler;

    private enum ContentType { None, RoomList, CreateRoom }

    private ContentType m_DetourContent;
    private ContentType m_ContentOpen;

    public override void Init()
    {
        base.Init();

        m_JoinRoomButton.onClick.AddListener(OnJoinRoomButtonClick);
        m_CreateRoomButton.onClick.AddListener(OnCreateRoomButtonClick);
        m_LeaveLobbyButton.onClick.AddListener(OnLeaveLobbyButtonClick);

        m_ContentAnimator = m_Content.GetComponent<Animator>();
        m_ContentHandler = m_Content.GetComponent<InLobbyContentHandler>();

        m_ContentHandler.ContentOpened += OnContentOpened;
        m_ContentHandler.ContentClosed += OnContentClosed;
        m_ContentHandler.RoomItemJoinButtonClick += OnRoomItemJoinButtonClick;
        m_ContentHandler.CreateRoomFormTurnedIn += OnCreateRoomTurnedIn;

        m_ContentHandler.Init();
    }

    private void OnDisable()
    {
        SetActiveStateOfContent(ContentType.CreateRoom, false);
        SetActiveStateOfContent(ContentType.RoomList, false);
        m_ContentOpen = ContentType.None;
        m_DetourContent = ContentType.None;
        m_ContentHandler.SetActiveStateOfCreateRoomContent(false);
        m_ContentHandler.SetActiveStateOfRoomListContent(false);
        m_Content.transform.localScale = new Vector3(0, 0, 1);
    }

    public void UpdateRoomListContent(List<RoomInfo> roomList)
    {
        var noRooms = roomList.Count == 0;
        var roomListContentOpen = m_ContentOpen == ContentType.RoomList;
        m_ContentHandler.UpdateRoomListContent(roomList, roomListContentOpen);
    }

    public void UpdateLobbyStatsForContentHandler(List<TypedLobbyInfo> statistics)
    {
        m_ContentHandler.UpdateLobbyRoomsWithLobbyStatistics(statistics);
    }

    public void SetEnableStateOfCreateRoomButton(bool value)
    {
        m_CreateRoomButton.enabled = value;
    }

    private void OnRoomItemJoinButtonClick(RoomInfo itemInfo)
    {
        var result = new InLobbyConnectResult()
        {
            args = itemInfo,
            choice = InLobbyConnectChoice.Joining
        };

        OnTaskFinished(result);
    }

    private void OnCreateRoomTurnedIn(CreateRoomFormResult createRoomResult)
    {
        var result = new InLobbyConnectResult()
        {
            args = createRoomResult,
            choice = InLobbyConnectChoice.Creating
        };

        OnTaskFinished(result);
    }

    private void OnJoinRoomButtonClick()
    {
        switch (m_ContentOpen)
        {
            case ContentType.None:
                OpenContent(ContentType.RoomList);
                break;

            case ContentType.RoomList:
                CloseContent();
                break;

            case ContentType.CreateRoom:
                CloseContent(ContentType.RoomList);
                break;
        }

        m_JoinRoomButton.interactable = false;
    }

    private void OpenContent(ContentType content)
    {
        if (content != ContentType.None)
        {
            m_ContentAnimator.SetTrigger("Open");
            m_ContentOpen = content;
            switch (m_ContentOpen)
            {
                case ContentType.RoomList:
                    m_ContentHandler.SetActiveStateOfRoomListContent(true);
                    break;

                case ContentType.CreateRoom:
                    m_ContentHandler.SetActiveStateOfCreateRoomContent(true);
                    break;
            }
        }
    }

    private void SetActiveStateOfContent(ContentType content, bool value)
    {
        switch (content)
        {
            case ContentType.RoomList:
                m_ContentHandler.SetActiveStateOfRoomListContent(value);
                break;

            case ContentType.CreateRoom:
                break;
        }
    }

    private void OnContentOpened()
    {
        switch (m_ContentOpen)
        {
            case ContentType.RoomList:
                m_JoinRoomButton.interactable = true;
                break;

            case ContentType.CreateRoom:
                m_CreateRoomButton.interactable = true;
                break;
        }
    }

    private void OnContentClosed()
    {
        switch (m_ContentOpen)
        {
            case ContentType.RoomList:
                m_ContentHandler.SetActiveStateOfRoomListContent(false);
                break;

            case ContentType.CreateRoom:
                m_ContentHandler.SetActiveStateOfCreateRoomContent(false);
                break;
        }

        if (m_DetourContent == ContentType.None)
        {
            switch (m_ContentOpen)
            {
                case ContentType.RoomList:
                    m_JoinRoomButton.interactable = true;
                    break;

                case ContentType.CreateRoom:
                    m_CreateRoomButton.interactable = true;
                    break;
            }
            m_ContentOpen = ContentType.None;
        }
        else
        {
            OpenContent(m_DetourContent);
            m_DetourContent = ContentType.None;
        }
    }

    private void CloseContent(ContentType detourContent = ContentType.None)
    {
        m_ContentAnimator.SetTrigger("Close");
        m_DetourContent = detourContent;
    }

    private void OnCreateRoomButtonClick()
    {
        switch (m_ContentOpen)
        {
            case ContentType.None:
                OpenContent(ContentType.CreateRoom);
                break;

            case ContentType.RoomList:
                CloseContent(ContentType.CreateRoom);
                break;

            case ContentType.CreateRoom:
                CloseContent();
                break;
        }

        m_CreateRoomButton.interactable = false;
    }

    private void OnLeaveLobbyButtonClick()
    {
        var result = new InLobbyConnectResult()
        {
            args = null,
            choice = InLobbyConnectChoice.Leaving
        };

        OnTaskFinished(result);
    }
}