using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/ChaseActionDog")]
public class ChaseActionDog : ChaseAction
{
    public override void Act(AIStateController controller)
    {
        ChaseDataContainer chase = (ChaseDataContainer)controller.GetData(AIStateDataType.Chase);
        chase.UpdateIntervalTime(this, controller);
    }

    public override void Begin(AIStateController controller)
    {
        ChaseDataContainer chase = (ChaseDataContainer)controller.GetData(AIStateDataType.Chase);
        controller.Agent.speed = chaseSpeed;
        controller.Agent.angularSpeed = chaseTurnSpeed;
        if (controller.Agent.stoppingDistance == 0f)
        {
            float stoppingDistance = GetStoppingDistanceBasedOnSizes(chase.ChaseTarget, controller.transform);
            controller.Agent.stoppingDistance = stoppingDistance;
        }
        controller.Animator.SetFloat("WalkSpeed", walkAnimationSpeedMultiplier);
    }

    public override void End(AIStateController controller)
    {
        controller.Animator.SetFloat("WalkSpeed", 1.0f);
    }

    private float GetStoppingDistanceBasedOnSizes(Transform chaseTarget, Transform myTransform)
    {
        Collider col1 = chaseTarget.GetComponentInChildren<Collider>();
        Collider col2 = myTransform.GetComponentInChildren<Collider>();
        Vector3 size1 = col1.bounds.size;
        Vector3 size2 = col2.bounds.size;
        return (((size1.x + size1.y + size1.z) * 0.33f) + ((size2.x + size2.y + size2.z) * 0.33f)) * 0.5f;
    }
}