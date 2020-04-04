using UnityEngine;

public class PhotonSingletonManager : MonoBehaviour
{
    [SerializeField, Tooltip("Set to true if you want players to sync scene with the master client")]
    private bool m_AutomaticallySyncScene;

    private void Awake()
    {
        if(ConnectionManager.Instance == null)
        {
            new GameObject("ConnectionManager", typeof(ConnectionManager))
                .GetComponent<ConnectionManager>()
                .Init(m_AutomaticallySyncScene);
        }  
        
        if(InLobbyManager.Instance == null)
        {
            new GameObject("InLobbyManager", typeof(InLobbyManager));
        }
        
        Destroy(this.gameObject);
    }
}
