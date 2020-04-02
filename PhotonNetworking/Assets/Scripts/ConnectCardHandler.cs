using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ConnectCard { StartConnect, ConnectStatus}

public class ConnectCardHandler : MonoBehaviour, IConnectionCallbacks
{
    [SerializeField]
    private bool m_DontDestroyOnLoad;

    [SerializeField]
    private GameObject[] m_cards;

    [SerializeField]
    private ConnectionManager m_ConnectionManager;

    private Dictionary<ConnectCard, GameObject> m_cardsDict = new Dictionary<ConnectCard, GameObject>();

    private void Awake()
    {
        var cards = Enum.GetValues(typeof(ConnectCard)).Cast<ConnectCard>();
        foreach (var card in m_cards)
        {
            var key = cards.Where(c => card.name.Contains(c.ToString())).First();
            m_cardsDict.Add(key, card);
        }

        EnableConnectCard(ConnectCard.StartConnect);

        m_ConnectionManager.AddCallbackTarget(this);

        if (m_DontDestroyOnLoad)
            DontDestroyOnLoad(this.gameObject);
    }   

    private void EnableConnectCard(ConnectCard card)
    {
        var cardGameObject = m_cardsDict[card];
        cardGameObject.SetActive(true);
        cardGameObject.GetComponent<ConnectCardAbstract>().TaskFinished += OnConnectCardFinishedTask;
    }

    private void DisableConnectCard(ConnectCard card)
    {
        m_cardsDict[card].SetActive(false);
    }

    private void OnConnectCardFinishedTask(GameObject cardGameObject)
    {
        var card = m_cardsDict.Where(pair => pair.Value == cardGameObject).First().Key;
        switch (card)
        {
            case ConnectCard.StartConnect:
                m_ConnectionManager.ConnectToMaster();
                break;
            case ConnectCard.ConnectStatus:
                break;
        }
    }

    public void OnConnected()
    {
        
    }

    public void OnDisconnected(DisconnectCause cause)
    {

    }

    public void OnConnectedToMaster()
    {
        
    }

    public void OnCustomAuthenticationFailed(string debugMessage){}

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data){}
    
    public void OnRegionListReceived(RegionHandler regionHandler){}   
}
