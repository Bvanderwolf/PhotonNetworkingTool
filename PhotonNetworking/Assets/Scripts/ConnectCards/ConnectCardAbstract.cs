namespace ConnectCards
{
    using System;
    using UnityEngine;

    public abstract class ConnectCardAbstract : MonoBehaviour
    {
        public event Action<GameObject, object> m_TaskFinished;

        public virtual void Init()
        {
        }

        protected virtual void OnTaskFinished(object args)
        {
            m_TaskFinished?.Invoke(this.gameObject, args);
        }
    }
}