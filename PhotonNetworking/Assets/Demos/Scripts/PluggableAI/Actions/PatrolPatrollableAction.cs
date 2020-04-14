using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol/PatrolPatrollable")]
public class PatrolPatrollableAction : AIAction
{
    public override void Act(AIDataContainer data)
    {
        //walk around patrollable object (make use of navmesh, navAgent)
    }
}