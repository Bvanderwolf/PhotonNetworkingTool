using UnityEngine;

public class WanderDataContainer : AIDataContainer
{
    public bool IsIdle { get; private set; }
    private Vector3 wanderTarget;
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
        wanderTarget = startPosition + new Vector3(v.x, 0, v.y);
        controller.Agent.destination = wanderTarget;
        controller.Animator.SetInteger("Walk", 1);
    }
}