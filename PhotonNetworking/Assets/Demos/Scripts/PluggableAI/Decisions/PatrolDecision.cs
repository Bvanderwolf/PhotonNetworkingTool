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
        var container = (PatrolDataContainer)data;
        var patrollables = container.Patrollables.Where(p => p.Spotted);
        var count = patrollables.Count();
        if (!container.PatrollingPatrollable && count != 0)
        {
            var patrolRandom = count > 1 && Random.Range(0, 1f) < m_ChanceOfRandomPatrol;
            var patrollable = patrolRandom ? GetRandomPatrollable(patrollables, count) : patrollables.First();
            container.SetPatrollable(patrollable);
            return true;
        }

        return false;
    }

    private Patrollable GetRandomPatrollable(IEnumerable<Patrollable> patrollables, int count)
    {
        Patrollable patrollable = null;
        int rdmCount = Random.Range(0, count);
        int counter = 0;
        foreach (var p in patrollables)
        {
            if (counter++ == rdmCount)
            {
                patrollable = p;
                break;
            }
        }
        return patrollable;
    }
}