using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol/Search")]
public class SearchPatrollablesAction : AIAction
{
    [SerializeField]
    private float m_SearchRange = 5f;

    [SerializeField]
    private float m_LookRange = 5f;

    [SerializeField]
    private float m_LookView = 2.5f;

    private const int PATROL_LAYER = 8;

    public override void Act(AIDataContainer data)
    {
        //walk towards a random direction (Random.InsideUnitCirkel)
        var container = (PatrolDataContainer)data;
        var controller = container.Controller;
        var navAgent = controller.Agent;
        if (!navAgent.hasPath)
        {
            var rdm = Random.insideUnitCircle * m_SearchRange;
            var target = container.Controller.transform.position + new Vector3(rdm.x, 0, rdm.y);
            navAgent.destination = target;
        }

        var patrollables = container.Patrollables;
        if (patrollables.Count != 0)
            TrySpottingPatrollables(controller.Eye, container);
    }

    private void TrySpottingPatrollables(Transform eyeTransform, PatrolDataContainer data)
    {
        Collider[] colliders = null;
        var forward = eyeTransform.forward;
        var position = eyeTransform.position + forward;
        var radius = 0.25f;
        var layerMask = 1 << PATROL_LAYER;
        for (int i = 0; i < 5; i++)
        {
            colliders = Physics.OverlapSphere(position + (forward * i * (radius * i)), radius * i, layerMask);
            if (colliders.Length != 0)
            {
                data.SpotPatrollable(colliders[0].gameObject.GetInstanceID());
                break;
            }
        }
    }
}