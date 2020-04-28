using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AttackDecision")]
public class AttackDecision : AIDecision
{
    public override bool Decide(AIStateController controller)
    {
        ChaseDataContainer container = (ChaseDataContainer)controller.GetData(AIStateDataType.Chase);
        float distance = (container.ChaseTarget.position - controller.transform.position).magnitude;
        return distance < controller.Agent.stoppingDistance;
    }
}