using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Patrol")]
public class PatrolDecision : AIDecision
{
    /// <summary>
    /// returns whether to patrol a patrollable object (true) or search
    /// for patrollable objects (false)
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    public override bool Decide(AIDataContainer data)
    {
        return ((PatrolDataContainer)data).HasPatrollable;
    }
}