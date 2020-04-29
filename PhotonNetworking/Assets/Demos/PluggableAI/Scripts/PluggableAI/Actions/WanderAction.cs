using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "PluggableAI/Actions/WanderAction")]
public class WanderAction : AIAction
{
    [SerializeField]
    private float wanderSpeed;

    [SerializeField]
    private float wanderTurnSpeed;

    [SerializeField]
    private float idleTime;

    [SerializeField]
    private float wanderDistance;

    public float IdleTime
    {
        get
        {
            return idleTime;
        }
    }

    public float WanderDistance
    {
        get
        {
            return wanderDistance;
        }
    }

    public override void Begin(AIStateController controller)
    {
        controller.Agent.speed = wanderSpeed;
        controller.Agent.stoppingDistance = 0f;
        controller.Agent.angularSpeed = wanderTurnSpeed;

        WanderDataContainer container = controller.Data.Wander;
        container.SetStartPosition(controller.transform.position);
        container.SetWanderTarget(this, controller);

        ChaseDataContainer chase = controller.Data.Chase;
        if (chase.ChaseTarget != null)
        {
            chase.SetChaseTarget(null);
        }

        AttackDataContainer attack = controller.Data.Attack;
        if (attack.DamageTarget != null)
        {
            attack.SetDamageTarget(null);
        }
    }

    public override void Act(AIStateController controller)
    {
        WanderDataContainer container = controller.Data.Wander;
        if (container.IsIdle)
        {
            container.UpdateCurrentIdleTime(this, controller);
        }
        else
        {
            NavMeshAgent agent = controller.Agent;
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        container.SetIdleState(true);
                        controller.Animator.SetInteger("Walk", 0);
                    }
                }
            }
        }
    }

    public override void End(AIStateController controller)
    {
        controller.Data.Wander.Reset();
    }
}