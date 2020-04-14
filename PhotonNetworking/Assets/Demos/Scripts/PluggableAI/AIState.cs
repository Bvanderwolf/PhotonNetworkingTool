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

    private void DoActions(AIStateController controller)
    {
        foreach (var action in m_Actions)
            action.Act(controller);
    }

    private void CheckTransitions(AIStateController controller)
    {
        foreach (var transition in m_Transitions)
        {
            var decisionSucceeded = transition.decision.Decide(controller);
            var transitionState = decisionSucceeded ? transition.trueState : transition.falseState;
            var transitioned = controller.TryTransition(transitionState);
            if (transitioned) break;
        }
    }
}