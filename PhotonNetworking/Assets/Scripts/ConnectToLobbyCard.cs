using UnityEngine;
using UnityEngine.UI;

public class ConnectToLobbyCard : ConnectCardAbstract
{
    [SerializeField]
    private Button[] m_LobbyButtons;

    private void Awake()
    {
        foreach(var button in m_LobbyButtons)
        {
            button.onClick.AddListener(() =>
            {
                var name = button.GetComponentInChildren<Text>().text;
                OnTaskFinished(name);
            });
        }
    }
}
