using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "PluggableAI/Actions/AttackActionDog")]
public class AttackActionDog : AttackAction
{
    public override void Act(AIStateController controller)
    {
        AttackDataContainer container = (AttackDataContainer)controller.GetData(AIStateDataType.Attack);
        if (container.CanAttack)
        {
            controller.Animator.SetTrigger("Attack");
            container.ResetAttackInterval(AttackTimeInterval);
        }
        else
        {
            container.UpdateAttackIntervalTime();
        }
    }

    public override void Begin(AIStateController controller)
    {
        controller.Animator.SetInteger("Walk", 0);
    }

    public override void End(AIStateController controller)
    {
        controller.Animator.SetInteger("Walk", 1);
    }
}