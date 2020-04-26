using UnityEngine;

public abstract class AIDataContainer
{
}

public class WanderDataContainer : AIDataContainer
{
    public bool IsIdle { get; private set; }
    public Vector3 WanderTarget { get; private set; }
    private Vector3 startPosition;
    private float currentIdleTime = 0;

    public void UpdateCurrentIdleTime(WanderAction action, AIStateController controller)
    {
        currentIdleTime += Time.deltaTime;
        if (currentIdleTime > action.IdleTime)
        {
            currentIdleTime = 0;
            IsIdle = false;
            SetWanderTarget(action, controller);
        }
    }

    public void Reset()
    {
        currentIdleTime = 0;
        IsIdle = false;
    }

    public void SetIdleState(bool idle)
    {
        IsIdle = idle;
    }

    public void SetStartPosition(Vector3 startPosition)
    {
        this.startPosition = startPosition;
    }

    public void SetWanderTarget(WanderAction action, AIStateController controller)
    {
        Vector2 v = Random.insideUnitCircle * action.WanderDistance;
        WanderTarget = startPosition + new Vector3(v.x, 0, v.y);
        controller.Agent.destination = WanderTarget;
        controller.Animator.SetInteger("Walk", 1);
    }
}