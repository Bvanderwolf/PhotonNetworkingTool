using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ConnectCard { StartConnect, ConnectStatus}

public class ConnectCardHandler : MonoBehaviour, IConnectionCallbacks
{
    [SerializeField, Tooltip("Check if you have multiple scenes for your game")]
    private bool m_DontDestroyOnLoad;

    [SerializeField]
    private GameObject[] m_cards;

    private Dictionary<ConnectCard, GameObject> m_cardsDict = new Dictionary<ConnectCard, GameObject>();
    private GameObject m_ActiveCard = null;

    private void Awake()
    {
        var cards = Enum.GetValues(typeof(ConnectCard)).Cast<ConnectCard>();
        foreach (var card in m_cards)
        {
            var key = cards.Where(c => card.name.Contains(c.ToString())).First();
            m_cardsDict.Add(key, card);
        }

        EnableConnectCard(ConnectCard.StartConnect);     

        if (m_DontDestroyOnLoad)
            DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        ConnectionManager.Instance.AddCallbackTarget(this);
    }

    private void EnableConnectCard(ConnectCard card)
    {
        var cardGameObject = m_cardsDict[card];
        cardGameObject.SetActive(true);
        cardGameObject.GetComponent<ConnectCardAbstract>().m_TaskFinished += OnConnectCardFinishedTask;
        m_ActiveCard = cardGameObject;
    }

    private void DisableConnectCard(ConnectCard card)
    {
        var cardGameObject = m_cardsDict[card];
        cardGameObject.SetActive(false);
        cardGameObject.GetComponent<ConnectCardAbstract>().m_TaskFinished -= OnConnectCardFinishedTask;
    }

    private void OnConnectCardFinishedTask(GameObject cardGameObject, object args)
    {     
        var card = m_cardsDict.Where(pair => pair.Value == cardGameObject).First().Key;
        switch (card)
        {
            case ConnectCard.StartConnect:
                ReplaceCard(ConnectCard.StartConnect, ConnectCard.ConnectStatus);
                SetupConnectStatusCard(ConnectTarget.MASTER);
                ConnectionManager.Instance.ConnectToMaster((string)args);
                break;
            case ConnectCard.ConnectStatus:
                DisableConnectCard(ConnectCard.ConnectStatus);
                break;
        }
    }

    private void SetupConnectStatusCard(ConnectTarget target)
    {
        var card = m_ActiveCard.GetComponent<ConnectStatusCardHandler>();
        if(card == null)
        {
            Debug.LogError("Wont setup connect status card :: card is not of right type");
            return;
        }
        card.SetConnectTarget(target);
    }

    private void ReplaceCard(ConnectCard oldCard, ConnectCard newCard)
    {
        EnableConnectCard(newCard);
        DisableConnectCard(oldCard);
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
