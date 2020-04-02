using System;
using UnityEngine;

public abstract class ConnectCardAbstract : MonoBehaviour
{
    public event Action<GameObject> TaskFinished;

    protected virtual void OnTaskFinished()
    {
        TaskFinished?.Invoke(this.gameObject);
    }
}
