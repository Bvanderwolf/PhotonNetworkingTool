using System;
using UnityEngine;

[Serializable]
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

    public abstract bool Finished { get; }
    public abstract bool IsOverTime { get; }
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