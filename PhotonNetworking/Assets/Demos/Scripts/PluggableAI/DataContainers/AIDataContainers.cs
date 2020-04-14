using System.Collections.Generic;
using UnityEngine;

public abstract class AIDataContainer
{
    public AIDataContainer(AIStateController controller)
    {
        Controller = controller;
    }

    public abstract AIStateController Controller { get; set; }
}

public class PatrolDataContainer : AIDataContainer
{
    public readonly List<Transform> Patrollables = new List<Transform>();
    public bool PatrollingPatrollable { get; private set; } = false;

    public override AIStateController Controller
    {
        get => m_Controller;
        set => m_Controller = value;
    }

    private Transform m_Patrollable = null;
    private AIStateController m_Controller;

    public PatrolDataContainer(AIStateController controller) : base(controller)
    {
        Controller = controller;
    }

    public void SetPatrollable(Transform patrollable)
    {
        m_Patrollable = patrollable;
        PatrollingPatrollable = true;
    }
}