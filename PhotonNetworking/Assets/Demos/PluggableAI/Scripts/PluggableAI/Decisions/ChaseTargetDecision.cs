using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ChaseTargetDecision")]
public class ChaseTargetDecision : AIDecision
{
    public override bool Decide(AIStateController controller)
    {
        ChaseDataContainer container = (ChaseDataContainer)controller.GetData(AIStateDataType.Chase);
        float distance = (container.ChaseTarget.position - controller.transform.position).magnitude;
        return distance > controller.Agent.stoppingDistance;
    }
}