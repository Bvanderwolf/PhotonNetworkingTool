using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/AttackActionDog")]
public class AttackActionDog : AttackAction
{
    public override void Act(AIStateController controller)
    {
        AttackDataContainer container = (AttackDataContainer)controller.GetData(AIStateDataType.Attack);
        if (container.CanAttack)
        {
            container.DamageTarget.Damage(new TimedHealthModifier(0, 10, false, true));
            controller.Learn(experienceGainPerAttack);
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
        AttackDataContainer container = (AttackDataContainer)controller.GetData(AIStateDataType.Attack);
        if (container.DamageTarget == null)
        {
            ChaseDataContainer chase = (ChaseDataContainer)controller.GetData(AIStateDataType.Chase);
            container.SetDamageTarget(chase.ChaseTarget);
        }
        controller.Animator.SetInteger("Walk", 0);
    }

    public override void End(AIStateController controller)
    {
        controller.Animator.SetInteger("Walk", 1);
    }
}