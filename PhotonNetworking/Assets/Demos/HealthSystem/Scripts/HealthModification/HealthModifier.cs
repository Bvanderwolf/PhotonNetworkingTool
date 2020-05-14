using System;
using UnityEngine;

public abstract class HealthModifier
{
    [Header("Settings")]
    [SerializeField, Tooltip("used for comparing modifiers")]
    protected string name = "";

    [SerializeField, Tooltip("Can this modifier stack with modifiers with the same name?")]
    protected bool canStack = true;

    [SerializeField, Tooltip("Will this modifier regenerate health or decay")]
    protected bool regenerate = false;

    [SerializeField, Tooltip("Will this modifier modify current value or max value?")]
    protected bool modifiesCurrent = false;

    [SerializeField, Tooltip("Does this modifier, when max is modified, also modify current?")]
    protected bool modifiesCurrentWithMax = true;

    public abstract bool Finished { get; }
    public abstract HealthModifier Clone { get; }

    public string Name
    {
        get { return name; }
    }

    public bool CanStack
    {
        get { return canStack; }
    }

    public bool Regenerate
    {
        get { return regenerate; }
    }

    public abstract void Modify(HealthSystem system);
}