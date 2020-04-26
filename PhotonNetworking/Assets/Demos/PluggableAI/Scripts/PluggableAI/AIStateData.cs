using System.Collections.Generic;

public enum AIStateDataType
{
    Wander
}

public class AIStateData
{
    private Dictionary<AIStateDataType, AIDataContainer> dataContainerDict;

    public AIStateData(AIStateController controller)
    {
        dataContainerDict = new Dictionary<AIStateDataType, AIDataContainer>
        {
            { AIStateDataType.Wander, new WanderDataContainer() }
        };
    }

    public AIDataContainer GetData(AIStateDataType dataType)
    {
        return dataContainerDict[dataType];
    }
}