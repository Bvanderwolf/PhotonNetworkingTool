using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentConnectCardAbstract : ConnectCardAbstract
{
    [SerializeField]
    protected GameObject m_Content;

    protected enum ContentType { None, RoomList, CreateRoom }

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

    protected virtual void OnContentOpened()
    {
    }

    protected virtual void OnContentClosed()
    {
    }

    private void OnDisable()
    {
        SetActiveStateOfContent(ContentType.CreateRoom, false);
        SetActiveStateOfContent(ContentType.RoomList, false);
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
        }
    }

    protected void CloseContent(ContentType detourContent = ContentType.None)
    {
        m_ContentAnimator.SetTrigger("Close");
        m_DetourContent = detourContent;
    }
}