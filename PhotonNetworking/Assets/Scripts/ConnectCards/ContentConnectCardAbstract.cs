namespace ConnectCards
{
    using UnityEngine;

    public class ContentConnectCardAbstract : ConnectCardAbstract
    {
        [SerializeField]
        protected GameObject m_Content;

        protected enum ContentType
        {
            None,
            RoomList,
            CreateRoom,
            PlayerList,
            Chat
        }

        protected Animator m_ContentAnimator;
        protected ContentHandler m_ContentHandler;

        protected ContentType m_DetourContent;
        protected ContentType m_ContentOpen;

        public override void Init()
        {
            base.Init();

            m_ContentAnimator = m_Content.GetComponent<Animator>();
            m_ContentHandler = m_Content.GetComponent<ContentHandler>();

            m_ContentHandler.ContentOpened += OnContentOpened;
            m_ContentHandler.ContentClosed += OnContentClosed;
        }

        /// <summary>Override to get callbacks on content opened</summary>
        protected virtual void OnContentOpened()
        {
        }

        /// <summary>Override to get callbacks on content closed</summary>
        protected virtual void OnContentClosed()
        {
        }

        protected virtual void OnDisable()
        {
            m_ContentOpen = ContentType.None;
            m_DetourContent = ContentType.None;
            m_Content.transform.localScale = new Vector3(0, 0, 1);
        }

        protected void OpenContent(ContentType content)
        {
            if (content != ContentType.None)
            {
                m_ContentAnimator.SetTrigger("Open");
                m_ContentOpen = content;
                SetActiveStateOfContent(m_ContentOpen, true);
            }
        }

        protected void SetActiveStateOfContent(ContentType content, bool value)
        {
            switch (content)
            {
                case ContentType.RoomList:
                    ((InLobbyContentHandler)m_ContentHandler).SetActiveStateOfRoomListContent(value);
                    break;

                case ContentType.CreateRoom:
                    ((InLobbyContentHandler)m_ContentHandler).SetActiveStateOfCreateRoomContent(value);
                    break;

                case ContentType.PlayerList:
                    ((InRoomContentHandler)m_ContentHandler).SetActiveStateOfPlayerListContent(value);
                    break;

                case ContentType.Chat:
                    ((InRoomContentHandler)m_ContentHandler).SetActiveStateOfChatContent(value);
                    break;
            }
        }

        protected void CloseContent(ContentType detourContent = ContentType.None)
        {
            m_ContentAnimator.SetTrigger("Close");
            m_DetourContent = detourContent;
        }
    }
}