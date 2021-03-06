﻿using UnityEngine;

public class AttackDataContainer : AIDataContainer
{
    public bool CanAttack { get; private set; }
    public IDamageAble DamageTarget { get; private set; }
    private float currentAttackIntervalTime;

    public void UpdateAttackIntervalTime()
    {
        currentAttackIntervalTime -= Time.deltaTime;
        if (currentAttackIntervalTime <= 0)
        {
            CanAttack = true;
        }
    }

    public void ResetAttackInterval(float attackTimeInterval)
    {
        CanAttack = false;
        currentAttackIntervalTime = attackTimeInterval;
    }

    public void SetDamageTarget(Transform chaseTarget)
    {
        DamageTarget = chaseTarget.GetComponent<IDamageAble>();
    }
}