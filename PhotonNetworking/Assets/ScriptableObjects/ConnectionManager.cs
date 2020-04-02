using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[CreateAssetMenu(fileName = "ConnectionManager", menuName = "PhotonNetworkingTool/ConnectionManager", order = 1)]
public class ConnectionManager : ScriptableObject, IConnectionCallbacks
{
    private List<IConnectionCallbacks> callbackTargets = null;

    private void OnEnable()
    {
        if(Application.isPlaying)   
            PhotonNetwork.AddCallbackTarget(this);

        Debug.Log("loaded connection manager");
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
            PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void AddCallbackTarget(IConnectionCallbacks target)
    {
        if (callbackTargets == null)
            callbackTargets = new List<IConnectionCallbacks>();

        callbackTargets.Add(target);
    }

    public void OnConnected()
    {
    }

    public void ConnectToMaster()
    {
        
    }

    public void OnConnectedToMaster()
    {
    }

    public void OnDisconnected(DisconnectCause cause)
    {
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }
}
