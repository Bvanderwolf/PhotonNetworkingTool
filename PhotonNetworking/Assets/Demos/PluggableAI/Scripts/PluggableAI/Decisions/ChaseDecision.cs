using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ChaseDecision")]
public class ChaseDecision : AIDecision
{
    [SerializeField]
    private float distanceToChaseAt;

    public override bool Decide(AIStateController controller)
    {
        Transform target = null;
        Transform myTransform = controller.transform;
        foreach (Transform transform in AIService.Instance.AI)
        {
            if (transform != myTransform && Vector3.Distance(transform.position, myTransform.position) < distanceToChaseAt)
            {
                target = transform;
                break;
            }
        }

        if (target != null)
        {
            ChaseDataContainer container = (ChaseDataContainer)controller.GetData(AIStateDataType.Chase);
            container.SetChaseTarget(target);
            return true;
        }
        else
        {
            return false;
        }
    }
}