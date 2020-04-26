using System;
using UnityEngine;

public abstract class HealthModifier
{
    public abstract bool Finished { get; }

    public abstract void Modify(HealthSystem system);
}

public class TimedHealthModifier : HealthModifier
{
    private float timePassed;
    private float time;

    private int value;
    private int valueModified;
    private int currentValue;

    private bool modifiesCurrent;
    private bool regenerate;

    /// <summary>Returns whether the given time has been reached</summary>
    public override bool Finished
    {
        get
        {
            return timePassed >= time;
        }
    }

    /// <summary>creates health modifer given time(sec), value, whether it regenerates or decays and whether it modifies current or max health</summary>
    public TimedHealthModifier(float time, int value, bool regenerate, bool modifiesCurrent)
    {
        this.time = time;
        this.value = value;
        this.regenerate = regenerate;
        this.modifiesCurrent = modifiesCurrent;
    }

    /// <summary>Modifies system health based on given time and time passed</summary>
    public override void Modify(HealthSystem system)
    {
        if (time == 0f)
        {
            if (modifiesCurrent)
            {
                system.ModifyCurrent(this, regenerate ? value : -value);
            }
            else
            {
                system.ModifyMax(this, regenerate ? value : -value);
            }
            return;
        }

        timePassed += Time.deltaTime;
        currentValue = (int)(timePassed / time * value);
        if (currentValue != valueModified)
        {
            int difference = Mathf.Abs(currentValue - valueModified);
            valueModified += difference;
            if (modifiesCurrent)
            {
                system.ModifyCurrent(this, regenerate ? difference : -difference);
            }
            else
            {
                system.ModifyMax(this, regenerate ? difference : -difference);
            }
        }
    }
}

public class ConditionalHealthModifier : HealthModifier
{
    private Func<bool> condition;

    private float timePassed;
    private float time;

    private int valuePerSecond;
    private int lastModifiedSecond;
    private int seconds;

    private bool modifiesCurrent;
    private bool regenerate;

    public override bool Finished
    {
        get
        {
            return condition();
        }
    }

    /// <summary>Creates a health modifier than, until given condition is met, regenerates or decays current or max health
    /// with given value per second</summary>
    public ConditionalHealthModifier(int valuePerSecond, bool regenerate, bool modifiesCurrent, Func<bool> condition)
    {
        this.valuePerSecond = valuePerSecond;
        this.regenerate = regenerate;
        this.modifiesCurrent = modifiesCurrent;
        this.condition = condition;
    }

    /// <summary>Modifies system by regenerating or decaying given value each second</summary>
    public override void Modify(HealthSystem system)
    {
        timePassed += Time.deltaTime;
        seconds = (int)timePassed;
        if (seconds != lastModifiedSecond)
        {
            lastModifiedSecond++;
            if (modifiesCurrent)
            {
                system.ModifyCurrent(this, regenerate ? valuePerSecond : -valuePerSecond);
            }
            else
            {
                system.ModifyMax(this, regenerate ? valuePerSecond : -valuePerSecond);
            }
        }
    }
}