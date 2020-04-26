using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/WanderAction")]
public class WanderAction : AIAction
{
    [SerializeField]
    private float wanderSpeed;

    [SerializeField]
    private float idleTime;

    [SerializeField]
    private float wanderDistance;

    public float IdleTime
    {
        get
        {
            return idleTime;
        }
    }

    public float WanderDistance
    {
        get
        {
            return wanderDistance;
        }
    }

    public override void Begin(AIStateController controller)
    {
        WanderDataContainer container = (WanderDataContainer)controller.GetData(AIStateDataType.Wander);
        container.SetWanderTarget(this, controller);
    }

    public override void Act(AIStateController controller)
    {
        WanderDataContainer container = (WanderDataContainer)controller.GetData(AIStateDataType.Wander);
        if (container.IsIdle)
        {
            container.UpdateCurrentIdleTime(this, controller);
        }
        else
        {
            if (Vector3.Distance(controller.transform.position, container.WanderTarget) < 0.1f)
            {
                container.SetIdleState(true);
            }

            controller.transform.position = Vector3.MoveTowards(controller.transform.position, container.WanderTarget, Time.deltaTime * wanderSpeed);
        }
    }

    public override void End(AIStateController controller)
    {
        WanderDataContainer container = (WanderDataContainer)controller.GetData(AIStateDataType.Wander);
        container.Reset();
    }
}