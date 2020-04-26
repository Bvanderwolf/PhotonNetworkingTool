using System.Collections.Generic;

public enum AIStateDataType
{
    Patrol
}

public class AIStateData
{
    public AIStateData(AIStateController controller)
    {
        m_Containers = new Dictionary<AIStateDataType, AIDataContainer>
        {
            { AIStateDataType.Patrol, new PatrolDataContainer(controller) }
        };
    }

    private Dictionary<AIStateDataType, AIDataContainer> m_Containers;

    public AIDataContainer GetData(AIStateDataType dataType)
    {
        return m_Containers[dataType];
    }

    public void UpdatePatrollables(Patrollable patrollable)
    {
        ((PatrolDataContainer)m_Containers[AIStateDataType.Patrol]).UpdatePatrollables(patrollable);
    }

    public void SpotPatrollable(int instanceID)
    {
        ((PatrolDataContainer)m_Containers[AIStateDataType.Patrol]).SpotPatrollable(instanceID);
    }
}