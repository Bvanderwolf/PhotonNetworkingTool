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
        get { return new ConditionalHealthModifier(name, valuePerSecond, regenerate, modifiesCurrent, modifiesCurrentWithMax, canStack, stopCondition); }
    }

    /// <summary>
    /// Creates a new instance of a conditinal health modifier
    /// </summary>
    /// <param name="name">used for comparing modifiers</param>
    /// <param name="valuePerSecond">The amount of value this modifier will modify each second while active</param>
    /// <param name="regenerate">Will this modifier regenerate health or decay</param>
    /// <param name="modifiesCurrent">Will this modifier modify current value or max value?</param>
    /// <param name="canStack">Can this modifier stack with modifiers with the same name?</param>
    /// <param name="stopCondition">The condition on which this health modifier needs to stop modifying</param>
    public ConditionalHealthModifier(string name, int valuePerSecond, bool regenerate, bool modifiesCurrent, bool modifiesCurrentWithMax, bool canStack, Func<bool> stopCondition)
    {
        this.name = name;
        this.valuePerSecond = valuePerSecond;
        this.regenerate = regenerate;
        this.modifiesCurrent = modifiesCurrent;
        this.modifiesCurrentWithMax = modifiesCurrentWithMax;
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
                system.ModifyCurrent(this, regenerate ? valuePerSecond : -valuePerSecond);
            }
            else
            {
                system.ModifyMax(this, regenerate ? valuePerSecond : -valuePerSecond);
                if (modifiesCurrentWithMax && !system.IsFull)
                {
                    system.ModifyCurrent(this, regenerate ? valuePerSecond : -valuePerSecond);
                }
            }
        }
    }

    public override string ToString()
    {
        return $"ConditionalHealthModifier[name: {name}, valuePerSecond: {valuePerSecond}, regenerate: {regenerate}, modifiesCurrent: {modifiesCurrent}, canStack: {canStack}]";
    }
}