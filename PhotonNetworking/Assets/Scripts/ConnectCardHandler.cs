using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ConnectCard { StartConnect, ConnectStatus}

public class ConnectCardHandler : MonoBehaviour
{
    [SerializeField]
    private bool m_DontDestroyOnLoad;

    [SerializeField]
    private GameObject[] m_cards;

    private Dictionary<ConnectCard, GameObject> m_cardsDict = new Dictionary<ConnectCard, GameObject>();

    private void Awake()
    {
        var cards = Enum.GetValues(typeof(ConnectCard)).Cast<ConnectCard>();
        foreach (var card in m_cards)
        {
            var key = cards.Where(c => card.name.Contains(c.ToString())).FirstOrDefault();
            m_cardsDict.Add(key, card);
        }

        if(m_DontDestroyOnLoad)
            DontDestroyOnLoad(this.gameObject);
    }
}
