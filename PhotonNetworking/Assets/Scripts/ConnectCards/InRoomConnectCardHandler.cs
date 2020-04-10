namespace ConnectCards
{
    using Photon.Pun;
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
        }

        private void OnEnable()
        {
            m_Title.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SetActiveStateOfContent(ContentType.PlayerList, false);
            SetActiveStateOfContent(ContentType.Chat, false);
        }

        protected override void OnContentClosed()
        {
            base.OnContentClosed();
        }

        protected override void OnContentOpened()
        {
            base.OnContentOpened();
        }

        private void OnPlayerListButtonClick()
        {
        }

        private void OnChatButtonClick()
        {
        }

        private void OnLeaveRoomButtonClick()
        {
            //leaving means, the task is not finished succesfully
            OnTaskFinished(false);
        }
    }
}