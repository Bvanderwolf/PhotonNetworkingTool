using System;
using System.Collections;
using UnityEngine;

public abstract class ConnectCardAbstract : MonoBehaviour
{
    public event Action<GameObject, object> m_TaskFinished;

    protected virtual void OnTaskFinished(object args)
    {
        m_TaskFinished?.Invoke(this.gameObject, args);
    }
}
