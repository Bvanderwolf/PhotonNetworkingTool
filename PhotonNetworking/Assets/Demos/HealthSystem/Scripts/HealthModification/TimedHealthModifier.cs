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
        get { return new TimedHealthModifier(name, time, value, regenerate, modifiesCurrent, modifiesCurrentWithMax, canStack); }
    }

    /// <summary>
    /// Creates new instance of a timed health modifier
    /// </summary>
    /// <param name="name">used for comparing modifiers</param>
    /// <param name="time">Time it takes for this modifier to finish modifying the health system</param>
    /// <param name="value">The ammount of value it will modify over given amount of time</param>
    /// <param name="regenerate">Will this modifier regenerate health or decay</param>
    /// <param name="modifiesCurrent">Will this modifier modify current value or max value?</param>
    /// <param name="canStack">Can this modifier stack with modifiers with the same name?</param>
    public TimedHealthModifier(string name, float time, int value, bool regenerate, bool modifiesCurrent, bool modifiesCurrentWithMax, bool canStack)
    {
        this.name = name;
        this.time = time;
        this.value = value;
        this.regenerate = regenerate;
        this.modifiesCurrent = modifiesCurrent;
        this.modifiesCurrentWithMax = modifiesCurrentWithMax;
        this.canStack = canStack;
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
                if (modifiesCurrentWithMax && !system.IsFull)
                {
                    system.ModifyCurrent(this, regenerate ? difference : -difference);
                }
            }
        }
    }

    /// <summary>Returns identifaction string for this timed health modifier instance</summary>
    public override string ToString()
    {
        return $"TimedHealthModifier[name: {name}, time: {time}, valuePerSecond: {value}, regenerate: {regenerate}, modifiesCurrent: {modifiesCurrent}, canStack: {canStack}]";
    }
}