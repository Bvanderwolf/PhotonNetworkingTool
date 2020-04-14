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
        for (int i = 0; i < m_Actions.Length; i++)
        {
            m_Actions[i].Act(controller);
        }
    }

    private void CheckTransitions(AIStateController controller)
    {
        for (int i = 0; i < m_Transitions.Length; i++)
        {
            bool decisionSucceeded = m_Transitions[i].decision.Decide(controller);

            if (decisionSucceeded)
            {
                controller.Transition(m_Transitions[i].trueState);
            }
            else
            {
                controller.Transition(m_Transitions[i].falseState);
            }

            break;
        }
    }
}