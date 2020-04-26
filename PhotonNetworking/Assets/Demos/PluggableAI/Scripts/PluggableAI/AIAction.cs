using UnityEngine;

public abstract class AIAction : ScriptableObject
{
    public abstract void Start(AIStateController controller);

    public abstract void Act(AIStateController controller);

    public abstract void End(AIStateController controller);
}