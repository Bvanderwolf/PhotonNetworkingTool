using System.Collections.Generic;

public enum AIStateDataType
{
    Wander,
    Chase
}

public class AIStateData
{
    private Dictionary<AIStateDataType, AIDataContainer> dataContainerDict;

    public AIStateData(AIStateController controller)
    {
        dataContainerDict = new Dictionary<AIStateDataType, AIDataContainer>
        {
            { AIStateDataType.Wander, new WanderDataContainer() },
            { AIStateDataType.Chase, new ChaseDataContainer() }
        };
    }

    public AIDataContainer GetData(AIStateDataType dataType)
    {
        return dataContainerDict[dataType];
    }
}