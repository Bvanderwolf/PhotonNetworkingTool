using UnityEngine;

public abstract class AIAction : ScriptableObject
{
    public abstract void Act(AIDataContainer data);
}