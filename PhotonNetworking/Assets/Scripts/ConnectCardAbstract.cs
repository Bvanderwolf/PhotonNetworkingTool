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

    public void MoveToPosition(Vector2 screenPos)
    {
        var rectTransform = GetComponent<RectTransform>();
        StartCoroutine(MoveToPosition(screenPos, rectTransform.anchoredPosition, rectTransform));
    }

    private IEnumerator MoveToPosition(Vector2 target, Vector2 start, RectTransform tf)
    {
        float currentTime = 0;

        while (currentTime != 1f)
        {
            if (currentTime > 1f)
                currentTime = 1f;

            currentTime += Time.deltaTime;
            float t = currentTime / 1f;
            tf.anchoredPosition = Vector2.Lerp(target, start, t);
            yield return new WaitForFixedUpdate();
        }
    }
}
