using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AttackDecision")]
public class AttackDecision : AIDecision
{
    public override bool Decide(AIStateController controller)
    {
        float distance = (controller.Data.Chase.ChaseTarget.position - controller.transform.position).magnitude;
        return distance < controller.Agent.stoppingDistance;
    }
}