using UnityEngine;

public class ChaseAction : AIAction
{
    [SerializeField]
    protected float pathUpdateIntervalTime;

    public float PathUpdateIntervalTime
    {
        get
        {
            return pathUpdateIntervalTime;
        }
    }

    public override void Act(AIStateController controller)
    {
    }

    public override void Begin(AIStateController controller)
    {
    }

    public override void End(AIStateController controller)
    {
    }
}