using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "PluggableAI/Actions/AttackActionDog")]
public class AttackActionDog : AttackAction
{
    public override void Act(AIStateController controller)
    {
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