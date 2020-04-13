namespace ConnectCards
{
    using ConnectCards.Enums;
    using Photon.Pun;
    using Photon.Realtime;
    using Singletons;
    using UnityEngine;
    using UnityEngine.UI;

    public class InRoomConnectCardHandler : ContentConnectCardAbstract
    {
        [SerializeField]
        private Button m_PlayerListButton;

        [SerializeField]
        private Button m_ChatButton;

        [SerializeField]
        private Button m_LeaveRoomButton;

        [SerializeField]
        private Text m_Title;

        public override void Init()
        {
            base.Init();

            m_PlayerListButton.onClick.AddListener(OnPlayerListButtonClick);
            m_ChatButton.onClick.AddListener(OnChatButtonClick);
            m_LeaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClick);

            m_ContentHandler.Init();

            ((InRoomContentHandler)m_ContentHandler).ReadyStatusChanged += OnInRoomReadyStatusChange;
        }

        private void OnEnable()
        {
            m_Title.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SetActiveStateOfReadyConstraints(true);
            SetActiveStateOfContent(ContentType.PlayerList, false);
            SetActiveStateOfContent(ContentType.Chat, false);
        }

        private void OnInRoomReadyStatusChange(bool ready)
        {
            SetActiveStateOfReadyConstraints(!ready);
        }

        private void SetActiveStateOfReadyConstraints(bool active)
        {
            m_PlayerListButton.interactable = active;
            m_ChatButton.interactable = active;
        }

        public void OnInRoomStatusChange(Player player, InRoomStatus status)
        {
            ((InRoomContentHandler)m_ContentHandler).OnInRoomStatusChange(player, status);
            if (PhotonNetwork.IsMasterClient)
            {
                var readyStatuses = PlayerManager.Instance.GetSharedProperties<InRoomStatus>(PlayerManager.INROOMSTATUS_KEY);
                var allReady = readyStatuses.TrueForAll(s => s == InRoomStatus.Ready);
                if (allReady)
                {
                    print("all ready");
                }
            }
        }

        public void OnMasterClientChange(Player newMaster)
        {
            ((InRoomContentHandler)m_ContentHandler).OnMasterClientChange(newMaster);
        }

        public void OnPlayerEnteredRoom(Player player)
        {
            ((InRoomContentHandler)m_ContentHandler).OnPlayerEnteredRoom(player);
        }

        public void OnPlayerLeftRoom(Player player)
        {
            ((InRoomContentHandler)m_ContentHandler).OnPlayerLeftRoom(player);
        }

        protected override void OnContentClosed()
        {
            base.OnContentClosed();

            SetActiveStateOfContent(m_ContentOpen, false);

            if (m_DetourContent == ContentType.None)
            {
                switch (m_ContentOpen)
                {
                    case ContentType.PlayerList:
                        m_PlayerListButton.interactable = true;
                        break;

                    case ContentType.Chat:
                        m_ChatButton.interactable = true;
                        break;
                }
                PlayerManager.Instance.SetInRoomStatus(InRoomStatus.Inactive);
                m_ContentOpen = ContentType.None;
            }
            else
            {
                OpenContent(m_DetourContent);
                m_DetourContent = ContentType.None;
            }
        }

        protected override void OnContentOpened()
        {
            base.OnContentOpened();

            switch (m_ContentOpen)
            {
                case ContentType.PlayerList:
                    m_PlayerListButton.interactable = true;
                    PlayerManager.Instance.SetInRoomStatus(InRoomStatus.InPlayerlist);
                    break;

                case ContentType.Chat:
                    m_ChatButton.interactable = true;
                    PlayerManager.Instance.SetInRoomStatus(InRoomStatus.Chatting);
                    break;
            }
        }

        private void OnPlayerListButtonClick()
        {
            switch (m_ContentOpen)
            {
                case ContentType.None:
                    OpenContent(ContentType.PlayerList);
                    break;

                case ContentType.PlayerList:
                    CloseContent();
                    break;

                case ContentType.Chat:
                    CloseContent(ContentType.PlayerList);
                    break;
            }

            m_PlayerListButton.interactable = false;
        }

        private void OnChatButtonClick()
        {
            switch (m_ContentOpen)
            {
                case ContentType.None:
                    OpenContent(ContentType.Chat);
                    break;

                case ContentType.PlayerList:
                    CloseContent(ContentType.Chat);
                    break;

                case ContentType.Chat:
                    CloseContent();
                    break;
            }

            m_ChatButton.interactable = false;
        }

        private void OnLeaveRoomButtonClick()
        {
            //leaving means, the task is not finished succesfully
            OnTaskFinished(false);
        }
    }
}