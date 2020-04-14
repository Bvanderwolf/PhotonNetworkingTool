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

        private PhotonView m_PV;

        private bool m_ClosingCountdownSuccesfully = false;

        public override void Init()
        {
            base.Init();

            m_PlayerListButton.onClick.AddListener(OnPlayerListButtonClick);
            m_ChatButton.onClick.AddListener(OnChatButtonClick);
            m_LeaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClick);

            m_PV = GetComponent<PhotonView>();

            InRoomContentHandler handler = (InRoomContentHandler)m_ContentHandler;
            handler.Init();
            handler.ReadyStatusChanged += OnInRoomReadyStatusChange;
            handler.ChatMessageHandled += OnChatMessageHandled;
            handler.CountdownStopped += OnCountdownStopped;
        }

        private void OnEnable()
        {
            m_Title.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SetInteractableStateOfContentButtons(true);
            SetActiveStateOfContent(ContentType.PlayerList, false);
            SetActiveStateOfContent(ContentType.Chat, false);

            m_ClosingCountdownSuccesfully = false;
            m_ContentOpen = ContentType.None;

            InRoomContentHandler handler = (InRoomContentHandler)m_ContentHandler;
            handler.ClearChat();
            handler.ClearChatInput();

            PlayerManager.Instance.SetInRoomStatus(InRoomStatus.Inactive);
        }

        private void OnInRoomReadyStatusChange(bool ready)
        {
            SetInteractableStateOfContentButtons(!ready);
        }

        private void OnChatMessageHandled(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
                m_PV.RPC("UpdateChatWithMessage", RpcTarget.AllViaServer, msg, PhotonNetwork.LocalPlayer);
        }

        private void OnCountdownStopped(bool atZeroCount, Player byPlayer)
        {
            if (atZeroCount)
            {
                //ending the countdown at zero means we ended the task succesfully
                m_ClosingCountdownSuccesfully = true;
                CloseContent();
            }
            else
            {
                //if the count down wasn't stopped at zero count, a player will have to let others know
                m_PV.RPC("StartOrStopCountdown", RpcTarget.AllViaServer, false, byPlayer.NickName);
            }
        }

        private void SetInteractableStateOfContentButtons(bool interactable)
        {
            m_PlayerListButton.interactable = interactable;
            m_ChatButton.interactable = interactable;
        }

        public void OnInRoomStatusChange(Player player, InRoomStatus status)
        {
            ((InRoomContentHandler)m_ContentHandler).OnInRoomStatusChange(player, status);
            if (PhotonNetwork.IsMasterClient)
            {
                var readyStatuses = PlayerManager.Instance.GetSharedProperties<InRoomStatus>(PlayerManager.INROOMSTATUS_KEY);
                var allReady = InRoomManager.Instance.IsFull && readyStatuses.TrueForAll(s => s == InRoomStatus.Ready);
                if (allReady)
                {
                    m_PV.RPC("StartOrStopCountdown", RpcTarget.AllViaServer, true, PhotonNetwork.LocalPlayer.NickName);
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
                SetInteractableStateOfContentButtons(true);

                PlayerManager.Instance.SetInRoomStatus(InRoomStatus.Inactive);

                var closingCountdown = m_ContentOpen == ContentType.Countdown;
                if (closingCountdown && m_ClosingCountdownSuccesfully)
                {
                    m_ClosingCountdownSuccesfully = false;
                    OnTaskFinished(true);
                }

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
                    SetInteractableStateOfContentButtons(true);
                    PlayerManager.Instance.SetInRoomStatus(InRoomStatus.InPlayerlist);
                    break;

                case ContentType.Chat:
                    SetInteractableStateOfContentButtons(true);
                    PlayerManager.Instance.SetInRoomStatus(InRoomStatus.Chatting);
                    break;

                case ContentType.Countdown:
                    ((InRoomContentHandler)m_ContentHandler).StartGameCountdown();
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

            SetInteractableStateOfContentButtons(false);
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

            SetInteractableStateOfContentButtons(false);
        }

        private void OnLeaveRoomButtonClick()
        {
            //leaving means, the task is not finished succesfully
            OnTaskFinished(false);
        }

        [PunRPC]
        private void UpdateChatWithMessage(string message, Player sender)
        {
            ((InRoomContentHandler)m_ContentHandler).AddTextToChat(message, sender);
        }

        [PunRPC]
        private void StartOrStopCountdown(bool start, string senderNickName)
        {
            if (start)
            {
                CloseContent(ContentType.Countdown);
            }
            else
            {
                CloseContent();
                ((InRoomContentHandler)m_ContentHandler).AddTextToChat(ChatBotMessages.StoppedCountdown, senderNickName);
            }
        }
    }
}