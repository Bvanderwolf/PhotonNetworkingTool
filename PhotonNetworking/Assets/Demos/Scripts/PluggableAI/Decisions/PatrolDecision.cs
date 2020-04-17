using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Patrol")]
public class PatrolDecision : AIDecision
{
    [SerializeField]
    private float m_ChanceOfRandomPatrol = 0.5f;

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