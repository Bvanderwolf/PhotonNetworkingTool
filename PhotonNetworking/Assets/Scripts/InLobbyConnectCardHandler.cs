using UnityEngine;
using UnityEngine.UI;

public enum InLobbyConnectResult { Joining, Creating, Leaving }

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

    public override void Init()
    {
        base.Init();

        m_JoinRoomButton.onClick.AddListener(OnJoinRoomButtonClick);
        m_CreateRoomButton.onClick.AddListener(OnCreateRoomButtonClick);
        m_LeaveLobbyButton.onClick.AddListener(OnLeaveLobbyButtonClick);
    }

    private void OnJoinRoomButtonClick()
    {
        if (!m_Content.activeInHierarchy)
        {
            m_Content.SetActive(true);
        }
    }

    private void OnCreateRoomButtonClick()
    {
        
    }

    private void OnLeaveLobbyButtonClick()
    {        
        OnTaskFinished(InLobbyConnectResult.Leaving);
    }
}
