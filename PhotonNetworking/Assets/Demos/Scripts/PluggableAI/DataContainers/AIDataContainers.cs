using System.Collections.Generic;
using System.Linq;

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
    public bool PatrollingPatrollable { get; private set; } = false;

    public override AIStateController Controller
    {
        get => m_Controller;
        set => m_Controller = value;
    }

    private Patrollable m_Patrollable;
    private AIStateController m_Controller;

    public PatrolDataContainer(AIStateController controller) : base(controller)
    {
        Controller = controller;
    }

    public void SetPatrollable(Patrollable patrollable)
    {
        m_Patrollable = patrollable;
        PatrollingPatrollable = true;
    }

    public void UpdatePatrollables(Patrollable patrollable)
    {
        if (!Patrollables.Any(p => p.ID == patrollable.ID))
            Patrollables.Add(patrollable);
    }
}