using System.Collections.Generic;
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
            TrySpottingPatrollables(controller.transform, patrollables);
    }

    private void TrySpottingPatrollables(Transform transform, List<Patrollable> patrollables)
    {
        Vector3 c = transform.position;
        Vector3 a = transform.position + (transform.forward * m_LookRange) - (transform.right * m_LookView);
        Vector3 b = transform.position + (transform.forward * m_LookRange) - (-transform.right * m_LookView);
        DrawView(a, b, c);
        foreach (var patrollable in patrollables)
        {
            if (InsideView(patrollable.Transform.position, a, b, c))
            {
                Debug.DrawLine(transform.position, patrollable.Transform.position, Color.yellow);
                break;
            }
        }
    }

    private void DrawView(Vector3 a, Vector3 b, Vector3 c)
    {
        Debug.DrawLine(a, b, Color.red);
        Debug.DrawLine(a, c, Color.red);
        Debug.DrawLine(b, c, Color.red);
    }

    private bool InsideView(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
    {
        return OnSameSide(point, a, b, c)
            && OnSameSide(point, b, a, c)
            && OnSameSide(point, c, a, b);
    }

    private bool OnSameSide(Vector3 pt1, Vector3 pt2, Vector3 a, Vector3 b)
    {
        Vector3 cp1 = Vector3.Cross(b - a, pt1 - a);
        Vector3 cp2 = Vector3.Cross(b - a, pt2 - a);
        return Vector3.Dot(cp1, cp2) >= 0;
    }
}