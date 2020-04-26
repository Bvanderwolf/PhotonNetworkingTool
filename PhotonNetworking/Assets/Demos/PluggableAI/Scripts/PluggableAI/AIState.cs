using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/State")]
public class AIState : ScriptableObject
{
    [SerializeField]
    private AIAction[] m_Actions;

    [SerializeField]
    private AIStateTransition[] m_Transitions;

    [SerializeField, Tooltip("Select type of data this state needs")]
    private AIStateDataType m_DataType;

    public void UpdateState(AIStateController controller)
    {
        var data = controller.GetData(m_DataType);
        DoActions(data);
        CheckTransitions(data);
    }

    private void DoActions(AIDataContainer data)
    {
        foreach (var action in m_Actions)
            action.Act(data);
    }

    private void CheckTransitions(AIDataContainer data)
    {
        foreach (var transition in m_Transitions)
        {
            var decisionSucceeded = transition.decision.Decide(data);
            var transitionState = decisionSucceeded ? transition.trueState : transition.falseState;
            var transitioned = data.Controller.TryTransition(transitionState);
            if (transitioned) break;
        }
    }
}