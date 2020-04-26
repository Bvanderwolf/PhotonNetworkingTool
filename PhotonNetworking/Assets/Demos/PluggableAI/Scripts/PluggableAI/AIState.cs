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
            if (transition.decision.Decide(data))
            {
                data.Transition(transition.trueState);
                break;
            }
        }
    }
}