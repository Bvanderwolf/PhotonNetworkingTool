using UnityEngine;

[System.Serializable]
public class TimedHealthModifier : HealthModifier
{
    [SerializeField, Tooltip("Time it takes for this modifier to finish modifying the health system")]
    private float time = 0;

    [SerializeField, Tooltip("The ammount of value it will modify over given amount of time")]
    private int value = 0;

    private float timePassed;

    private int valueModified;
    private int currentValue;

    /// <summary>Returns whether the given time has been reached</summary>
    public override bool Finished
    {
        get { return timePassed >= time; }
    }

    /// <summary>Returns whether the health modifier modifies the system over a period and not instant</summary>
    public override bool IsOverTime
    {
        get { return time > 0f; }
    }

    /// <summary>Returns whether this modifier has a valid time</summary>
    public bool HasValidTime
    {
        get { return time >= 0; }
    }

    /// <summary>Returns whether this modifier has a valid value</summary>
    public bool HasValidValue
    {
        get { return value > 0; }
    }

    /// <summary>Returns a new instance of this modifier. Use this if you have stored a modifier and want to re-use it</summary>
    public override HealthModifier Clone
    {
        get { return new TimedHealthModifier(name, time, value, regenerate, modifiesCurrent, canStack); }
    }

    /// <summary>creates health modifer given time(sec), value, whether it regenerates or decays and whether it modifies current or max health</summary>
    public TimedHealthModifier(string name, float time, int value, bool regenerate, bool modifiesCurrent, bool canStack)
    {
        this.name = name;
        this.time = time;
        this.value = value;
        this.regenerate = regenerate;
        this.modifiesCurrent = modifiesCurrent;
        this.canStack = canStack;
    }

    /// <summary>Modifies system health based on given time and time passed</summary>
    public override void Modify(HealthSystem system)
    {
        if (time == 0f)
        {
            if (modifiesCurrent)
            {
                system.ModifyCurrent(this, Regenerate ? value : -value);
            }
            else
            {
                system.ModifyMax(this, Regenerate ? value : -value);
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
                system.ModifyCurrent(this, Regenerate ? difference : -difference);
            }
            else
            {
                system.ModifyMax(this, Regenerate ? difference : -difference);
            }
        }
    }
}