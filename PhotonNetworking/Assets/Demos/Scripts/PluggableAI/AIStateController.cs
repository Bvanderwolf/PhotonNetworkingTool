using UnityEngine;

public class AIStateController : MonoBehaviour
{
    [SerializeField]
    private AIState m_CurrentState;

    public float TimeSinceAwakening { get; private set; } = 0;
    public float SpawnHeight { get; private set; }

    private AIStateData m_Data;

    private void Awake()
    {
        SpawnHeight = transform.position.y;
        m_Data = new AIStateData(this);
    }

    private void Update()
    {
        TimeSinceAwakening += Time.deltaTime;
        m_CurrentState.UpdateState(this);
    }

    public AIDataContainer GetData(AIStateDataType _type)
    {
        return m_Data.GetData(_type);
    }

    public bool TryTransition(AIState nextState)
    {
        if (nextState != m_CurrentState)
        {
            m_CurrentState = nextState;
            return true;
        }
        return false;
    }
}