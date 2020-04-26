using UnityEngine;
using UnityEngine.AI;

public class AIDataContainer
{
    protected AIStateController controller;

    public Transform transform
    {
        get
        {
            return controller.transform;
        }
    }

    public NavMeshAgent agent
    {
        get
        {
            return controller.Agent;
        }
    }

    public AIDataContainer(AIStateController controller)
    {
        this.controller = controller;
    }

    public void Transition(AIState nextState)
    {
        if (nextState != null)
        {
            controller.Transition(nextState);
        }
    }
}