﻿using UnityEngine;

public class ChaseAction : AIAction
{
    [SerializeField]
    protected float pathUpdateIntervalTime;

    [SerializeField]
    protected float chaseSpeed;

    [SerializeField]
    protected float chaseTurnSpeed;

    protected const float walkAnimationSpeedMultiplier = 3.0f;

    public float PathUpdateIntervalTime
    {
        get
        {
            return pathUpdateIntervalTime;
        }
    }

    public override void Act(AIStateController controller)
    {
    }

    public override void Begin(AIStateController controller)
    {
    }

    public override void End(AIStateController controller)
    {
    }
}