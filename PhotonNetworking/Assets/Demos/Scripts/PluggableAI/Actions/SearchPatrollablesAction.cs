using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol/Search")]
public class SearchPatrollablesAction : AIAction
{
    [SerializeField]
    private float m_SearchRange = 5f;

    public override void Act(AIDataContainer data)
    {
        //walk towards a random direction (Random.InsideUnitCirkel)
        var container = (PatrolDataContainer)data;
        var navAgent = container.Controller.Agent;
        if (!navAgent.hasPath)
        {
            var rdm = Random.insideUnitCircle * m_SearchRange;
            var target = container.Controller.transform.position + new Vector3(rdm.x, 0, rdm.y);
            navAgent.destination = target;
        }
    }
}