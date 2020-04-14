using UnityEngine;

public class AIStateController : MonoBehaviour
{
    [SerializeField]
    private AIState m_CurrentState;

    public void Transition(AIState nextState)
    {
        if (nextState != m_CurrentState)
        {
            m_CurrentState = nextState;
        }
    }
}