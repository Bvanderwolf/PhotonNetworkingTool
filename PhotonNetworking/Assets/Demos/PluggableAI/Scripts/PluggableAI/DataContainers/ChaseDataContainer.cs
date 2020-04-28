using UnityEngine;

public class ChaseDataContainer : AIDataContainer
{
    public Transform ChaseTarget { get; private set; }
    private float currentPathUpdateIntervalTime = 0;

    public void UpdateIntervalTime(ChaseAction action, AIStateController controller)
    {
        currentPathUpdateIntervalTime += Time.deltaTime;
        if (currentPathUpdateIntervalTime > action.PathUpdateIntervalTime)
        {
            currentPathUpdateIntervalTime = 0;
            UpdatePath(controller);
        }
    }

    public void SetChaseTarget(Transform target)
    {
        ChaseTarget = target;
    }

    private void UpdatePath(AIStateController controller)
    {
        controller.Agent.destination = ChaseTarget.position;
    }
}