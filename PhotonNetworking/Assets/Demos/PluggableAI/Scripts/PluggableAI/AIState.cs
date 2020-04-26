using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/State")]
public class AIState : ScriptableObject
{
    [SerializeField]
    private AIAction[] m_Actions;

    [SerializeField]
    private AIStateTransition[] m_Transitions;

    public void UpdateState(AIStateController controller)
    {
        DoActions(controller);
        CheckTransitions(controller);
    }

    public void StartState(AIStateController controller)
    {
        StartActions(controller);
    }

    public void EndState(AIStateController controller)
    {
        EndActions(controller);
    }

    private void StartActions(AIStateController controller)
    {
        foreach (var action in m_Actions)
        {
            action.Start(controller);
        }
    }

    private void DoActions(AIStateController controller)
    {
        foreach (var action in m_Actions)
        {
            action.Act(controller);
        }
    }

    private void EndActions(AIStateController controller)
    {
        foreach (var action in m_Actions)
        {
            action.End(controller);
        }
    }

    private void CheckTransitions(AIStateController controller)
    {
        foreach (var transition in m_Transitions)
        {
            if (transition.decision.Decide(controller))
            {
                controller.Transition(transition.trueState);
                break;
            }
        }
    }
}