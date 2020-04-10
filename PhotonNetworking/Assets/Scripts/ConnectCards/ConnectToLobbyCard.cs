namespace ConnectCards
{
    using Photon.Realtime;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ConnectToLobbyCard : ConnectCardAbstract
    {
        [SerializeField]
        private Button[] m_LobbyButtons;

        [SerializeField]
        private Text[] m_LobbyPlayerCounts;

        private List<TypedLobbyInfo> m_Statistics;

        private string[] m_LobbyButtonNames;

        public override void Init()
        {
            base.Init();

            m_LobbyButtonNames = new string[m_LobbyButtons.Length];

            for (int i = 0; i < m_LobbyButtons.Length; i++)
            {
                var button = m_LobbyButtons[i];
                var name = button.GetComponentInChildren<Text>().text;
                m_LobbyButtonNames[i] = name;
                button.onClick.AddListener(() =>
                {
                    OnTaskFinished(name);
                });
            }
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        public void UpdateLobbyStatistics(List<TypedLobbyInfo> statistics)
        {
            if (m_Statistics == null)
                m_Statistics = new List<TypedLobbyInfo>();

            for (int i = 0; i < statistics.Count; i++)
            {
                if (i == m_Statistics.Count)
                    m_Statistics.Add(statistics[i]);
                else
                    m_Statistics[i] = statistics[i];
            }

            for (int i = 0; i < m_LobbyButtonNames.Length; i++)
            {
                foreach (var lobbyInfo in m_Statistics)
                {
                    if (m_LobbyButtonNames[i] == lobbyInfo.Name)
                        UpdateButtonWithPlayerCount(i, lobbyInfo.PlayerCount);
                }
            }
        }

        private void UpdateButtonWithPlayerCount(int index, int playerCount)
        {
            m_LobbyPlayerCounts[index].text = $"({playerCount})";
        }
    }
}