using System;
using UnityEngine;

public class ContentHandler : MonoBehaviour
{
    public event Action ContentOpened;

    public event Action ContentClosed;

    public void OnContentOpened()
    {
        ContentOpened?.Invoke();
    }

    public void OnContentClosed()
    {
        ContentClosed?.Invoke();
    }
}