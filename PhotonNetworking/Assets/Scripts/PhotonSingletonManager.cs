using UnityEngine;

public class PhotonSingletonManager : MonoBehaviour
{
    private void Awake()
    {
        if(ConnectionManager.Instance == null)        
            new GameObject("ConnectionManager", typeof(ConnectionManager));
        
        Destroy(this.gameObject);
    }
}
