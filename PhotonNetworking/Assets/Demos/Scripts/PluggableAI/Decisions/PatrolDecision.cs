using System.Collections.Generic;
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
        var container = (PatrolDataContainer)data;
        var patrollables = container.Patrollables;
        if (!container.PatrollingPatrollable && patrollables.Count != 0)
        {
            var patrolRandom = patrollables.Count > 1 && Random.Range(0, 1f) < m_ChanceOfRandomPatrol;
            var patrollable = patrolRandom ? GetRandomPatrollable(patrollables) : patrollables[0];
            container.SetPatrollable(patrollable);
            return true;
        }

        return false;
    }

    private Transform GetRandomPatrollable(List<Transform> patrollables)
    {
        return patrollables[Random.Range(0, patrollables.Count + 1)];
    }
}