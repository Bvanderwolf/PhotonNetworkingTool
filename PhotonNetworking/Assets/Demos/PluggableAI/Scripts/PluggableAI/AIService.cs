using System.Collections.Generic;
using UnityEngine;

public class AIService : MonoBehaviour
{
    public static AIService Instance { get; private set; }

    public List<Transform> AI { get; private set; } = new List<Transform>();

    private void Awake()
    {
        Instance = this;
    }

    public void Subscribe(Transform ai)
    {
        AI.Add(ai);
    }

    public void UnSubscribe(Transform ai)
    {
        AI.Remove(ai);
    }
}