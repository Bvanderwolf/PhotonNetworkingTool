using System.Collections.Generic;

public enum AIStateDataType
{
    Wander,
    Chase,
    Attack
}

public class AIStateData
{
    private Dictionary<AIStateDataType, AIDataContainer> dataContainerDict;

    public AIStateData(AIStateController controller)
    {
        dataContainerDict = new Dictionary<AIStateDataType, AIDataContainer>
        {
            { AIStateDataType.Wander, new WanderDataContainer() },
            { AIStateDataType.Chase, new ChaseDataContainer() },
            { AIStateDataType.Attack, new AttackDataContainer() }
        };
    }

    public AIDataContainer GetData(AIStateDataType dataType)
    {
        return dataContainerDict[dataType];
    }
}