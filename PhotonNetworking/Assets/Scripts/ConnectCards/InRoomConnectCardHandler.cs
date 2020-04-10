namespace ConnectCards
{
    using UnityEngine;
    using UnityEngine.UI;

    public class InRoomConnectCardHandler : ContentConnectCardAbstract
    {
        [SerializeField]
        private Button PlayerListButton;

        [SerializeField]
        private Button ChatButton;

        [SerializeField]
        private Button LeaveRoomButton;

        public override void Init()
        {
            base.Init();

            PlayerListButton.onClick.AddListener(OnPlayerListButtonClick);
            ChatButton.onClick.AddListener(OnChatButtonClick);
            LeaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClick);
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
        }
    }
}