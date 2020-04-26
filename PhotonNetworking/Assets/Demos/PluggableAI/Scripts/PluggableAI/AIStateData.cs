using System.Collections.Generic;

public enum AIStateDataType
{
}

public class AIStateData
{
    private Dictionary<AIStateDataType, AIDataContainer> dataContainerDict;

    public AIStateData(AIStateController controller)
    {
        dataContainerDict = new Dictionary<AIStateDataType, AIDataContainer>
        {
        };
    }

    public AIDataContainer GetData(AIStateDataType dataType)
    {
        return dataContainerDict[dataType];
    }
}