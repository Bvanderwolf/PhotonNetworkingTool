using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/AttackActionDog")]
public class AttackActionDog : AttackAction
{
    public override void Act(AIStateController controller)
    {
        AttackDataContainer container = controller.Data.Attack;
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
        AttackDataContainer container = controller.Data.Attack;
        if (container.DamageTarget == null)
        {
            container.SetDamageTarget(controller.Data.Chase.ChaseTarget);
        }
        controller.Animator.SetInteger("Walk", 0);
    }

    public override void End(AIStateController controller)
    {
        controller.Animator.SetInteger("Walk", 1);
    }
}