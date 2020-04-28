using UnityEngine;

public class AttackAction : AIAction
{
    [SerializeField]
    protected float attackTimeInterval;

    [SerializeField]
    protected int experienceGainPerAttack;

    public float AttackTimeInterval
    {
        get
        {
            return attackTimeInterval;
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