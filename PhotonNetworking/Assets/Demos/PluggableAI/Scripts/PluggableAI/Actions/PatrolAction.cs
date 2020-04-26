using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol/Base")]
public class PatrolAction : AIAction
{
    [SerializeField]
    private AIAction m_PatrolPatrollable;

    [SerializeField]
    private AIAction m_SearchPatrollables;

    [SerializeField]
    private AIDecision m_PatrolDecision;

    public override void Act(AIDataContainer data)
    {
        var patrollingPatrollable = m_PatrolDecision.Decide(data);

        if (patrollingPatrollable)
        {
            m_PatrolPatrollable.Act(data);
        }           
        else
        {
            m_SearchPatrollables.Act(data);
        }
    }
}