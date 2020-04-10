namespace ConnectCards
{
    using UnityEngine;

    public class InRoomConnectCardHandler : ContentConnectCardAbstract
    {
        public override void Init()
        {
            base.Init();
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