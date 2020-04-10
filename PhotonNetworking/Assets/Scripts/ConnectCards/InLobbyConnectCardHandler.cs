namespace ConnectCards
{
    using Photon.Realtime;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class InLobbyConnectCardHandler : ContentConnectCardAbstract
    {
        [SerializeField]
        private Button m_JoinRoomButton;

        [SerializeField]
        private Button m_CreateRoomButton;

        [SerializeField]
        private Button m_LeaveLobbyButton;

        public override void Init()
        {
            base.Init();

            m_JoinRoomButton.onClick.AddListener(OnJoinRoomButtonClick);
            m_CreateRoomButton.onClick.AddListener(OnCreateRoomButtonClick);
            m_LeaveLobbyButton.onClick.AddListener(OnLeaveLobbyButtonClick);

            InLobbyContentHandler handler = (InLobbyContentHandler)m_ContentHandler;
            handler.RoomItemJoinButtonClick += OnRoomItemJoinButtonClick;
            handler.CreateRoomFormTurnedIn += OnCreateRoomTurnedIn;

            m_ContentHandler.Init();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SetActiveStateOfContent(ContentType.CreateRoom, false);
            SetActiveStateOfContent(ContentType.RoomList, false);
        }

        public void UpdateRoomListContent(List<RoomInfo> roomList)
        {
            var noRooms = roomList.Count == 0;
            var roomListContentOpen = m_ContentOpen == ContentType.RoomList;
            ((InLobbyContentHandler)m_ContentHandler).UpdateRoomListContent(roomList, roomListContentOpen);
        }

        public void UpdateLobbyStatsForContentHandler(List<TypedLobbyInfo> statistics)
        {
            ((InLobbyContentHandler)m_ContentHandler).UpdateLobbyRoomsWithLobbyStatistics(statistics);
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

        protected override void OnContentOpened()
        {
            base.OnContentOpened();

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

        protected override void OnContentClosed()
        {
            base.OnContentClosed();

            SetActiveStateOfContent(m_ContentOpen, false);

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
}