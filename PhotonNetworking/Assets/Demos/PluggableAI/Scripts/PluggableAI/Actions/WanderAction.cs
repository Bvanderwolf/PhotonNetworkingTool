﻿using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "PluggableAI/Actions/WanderAction")]
public class WanderAction : AIAction
{
    [SerializeField]
    private float wanderSpeed;

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
        WanderDataContainer container = (WanderDataContainer)controller.GetData(AIStateDataType.Wander);
        container.SetWanderTarget(this, controller);
        container.SetStartPosition(controller.transform.position);
    }

    public override void Act(AIStateController controller)
    {
        WanderDataContainer container = (WanderDataContainer)controller.GetData(AIStateDataType.Wander);
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
        WanderDataContainer container = (WanderDataContainer)controller.GetData(AIStateDataType.Wander);
        container.Reset();
    }
}