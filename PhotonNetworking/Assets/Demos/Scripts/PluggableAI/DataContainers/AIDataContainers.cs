using System.Collections.Generic;
using System.Linq;
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
    public readonly List<Patrollable> Patrollables = new List<Patrollable>();

    public override AIStateController Controller
    {
        get => m_Controller;
        set => m_Controller = value;
    }

    public bool HasPatrollable => m_Patrollable != null;

    private Patrollable m_Patrollable;
    private AIStateController m_Controller;

    public PatrolDataContainer(AIStateController controller) : base(controller)
    {
        Controller = controller;
    }

    public void SetPatrollable(Patrollable patrollable)
    {
        m_Patrollable = patrollable;
    }

    public void UpdatePatrollables(Patrollable patrollable)
    {
        if (!Patrollables.Any(p => p.ID == patrollable.ID))
            Patrollables.Add(patrollable);
    }

    public void SpotPatrollable(int instanceID)
    {
        var patrollable = Patrollables.Where(p => p.ID == instanceID).FirstOrDefault();

        if (patrollable != null)
        {
            patrollable.Spotted = true;

            if (m_Patrollable == null)
                SetPatrollable(patrollable);
        }
    }
}