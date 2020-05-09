using System;
using UnityEngine;

[Serializable]
public class ConditionalHealthModifier : HealthModifier
{
    [SerializeField, Tooltip("The amount of value this modifier will modify each second while active")]
    private int valuePerSecond;

    private Func<bool> stopCondition;

    private float timePassed;

    private int lastModifiedSecond;
    private int seconds;

    /// <summary>Returns whether the condition has been met</summary>
    public override bool Finished
    {
        get { return stopCondition(); }
    }

    /// <summary>A conditional health modifier is always over time</summary>
    public override bool IsOverTime
    {
        get { return true; }
    }

    /// <summary>Returns whether value per second is valid</summary>
    public bool HasValidValuePerSecond
    {
        get { return valuePerSecond > 0; }
    }

    /// <summary>Returns whether condition is valid (doesn't throw exception)</summary>
    public bool HasValidCondition
    {
        get
        {
            try
            {
                stopCondition();
            }
            catch (Exception e)
            {
                Debug.LogWarning("condition threw exception: " + e);
                return false;
            }
            return true;
        }
    }

    /// <summary>Returns a new instance of this modifier. Use this if you have stored a modifier and want to re-use it</summary>
    public override HealthModifier Clone
    {
        get { return new ConditionalHealthModifier(name, valuePerSecond, regenerate, modifiesCurrent, canStack, stopCondition); }
    }

    /// <summary>Creates a health modifier than, until given condition is met, regenerates or decays current or max health
    /// with given value per second</summary>
    public ConditionalHealthModifier(string name, int valuePerSecond, bool regenerate, bool modifiesCurrent, bool canStack, Func<bool> stopCondition)
    {
        this.name = name;
        this.valuePerSecond = valuePerSecond;
        this.regenerate = regenerate;
        this.modifiesCurrent = modifiesCurrent;
        this.canStack = canStack;
        this.stopCondition = stopCondition;
    }

    /// <summary>Sets given condition as to when to stop this modifier</summary>
    public void SetStopCondition(Func<bool> stopCondition)
    {
        this.stopCondition = stopCondition;
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
                system.ModifyCurrent(this, Regenerate ? valuePerSecond : -valuePerSecond);
            }
            else
            {
                system.ModifyMax(this, Regenerate ? valuePerSecond : -valuePerSecond);
            }
        }
    }
}