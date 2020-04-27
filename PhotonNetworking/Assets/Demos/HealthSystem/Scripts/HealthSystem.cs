using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HealthSystem
{
    [SerializeField, Tooltip("Fillable image for the system to influence")]
    private Image healthBar;

    [SerializeField, Tooltip("Text showing state in format: {current/max}")]
    private Text healthText;

    [SerializeField, Tooltip("Show given text or not")]
    private bool showText;

    [SerializeField, Range(100, 1000)]
    private int max;

    [SerializeField, Range(minimalMinimalMax, 100), Tooltip("Minimal value for max health value (when decreasing max health)")]
    private int minimalMax;

    //minimal value for minimal max, max health cannot be below 10
    private const int minimalMinimalMax = 10;

    private SortedSet<HealthSystem> highPrioritySystems = new SortedSet<HealthSystem>();
    private List<HealthModifier> modifiers = new List<HealthModifier>();

    private float current;

    public event Action OnRegenStart, OnRegenEnd;

    public event Action OnDecayStart, OnDecayEnd;

    public event Action OnReachedMax, OnReachedZero;

    /// <summary>Returns current health value rounded to nearest integer value</summary>
    public int Current
    {
        get
        {
            return Mathf.RoundToInt(current);
        }
    }

    public int Max
    {
        get
        {
            return max;
        }
    }

    /// <summary>Returns whether this system is being modified by health modifiers</summary>
    public bool BeingModified
    {
        get
        {
            return modifiers.Count != 0;
        }
    }

    public void Update()
    {
        //if there are systems with higher priority being modified we dont update
        if (highPrioritySystems.Any(system => system.BeingModified))
        {
            return;
        }

        //let each modifier modify this system and remove it if it is finished
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            modifiers[i].Modify(this);
            if (modifiers[i].Finished)
            {
                modifiers.RemoveAt(i);
            }
        }

        //show feedback of data on healthbar and text if showable
        healthBar.fillAmount = current / max;
        if (showText && healthText != null)
        {
            healthText.text = $"{Current}/{max}";
        }
    }

    /// <summary>Sets current to max</summary>
    public void SetCurrentToMax()
    {
        current = max;
    }

    /// <summary>Adds modifier to system modifiers if it is valid and fires start events if condition is right</summary>
    public void AddModifier(HealthModifier modifier)
    {
        if (modifier as TimedHealthModifier != null)
        {
            TimedHealthModifier timed = (TimedHealthModifier)modifier;
            if (!timed.HasValidTime)
            {
                Debug.LogWarning($"Didn't add timed modifier to health system :: time was invalid");
                return;
            }
            else if (!timed.HasValidValue)
            {
                Debug.LogWarning($"Didn't add timed modifier to health system :: value was invalid");
                return;
            }

            if (modifiers.Count(m => m.Regenerate && m.IsOverTime) == 0 && timed.Regenerate && timed.IsOverTime)
            {
                OnRegenStart?.Invoke();
            }
            else if (modifiers.Count(m => !m.Regenerate && m.IsOverTime) == 0 && !timed.Regenerate && timed.IsOverTime)
            {
                OnDecayStart?.Invoke();
            }
        }
        else if (modifier as ConditionalHealthModifier != null)
        {
            ConditionalHealthModifier conditional = (ConditionalHealthModifier)modifier;
            if (!conditional.HasValidValuePerSecond)
            {
                Debug.LogWarning($"Didn't add conditional modifier to health system :: value per second was invalid");
                return;
            }
            else if (!conditional.HasValidCondition)
            {
                Debug.LogWarning($"Didn't add conditional modifier to health system :: condition threw exception");
                return;
            }

            if (modifiers.Count(m => m.Regenerate && m.IsOverTime) == 0 && conditional.Regenerate)
            {
                OnRegenStart?.Invoke();
            }
            else if (modifiers.Count(m => !m.Regenerate && m.IsOverTime) == 0 && !conditional.Regenerate)
            {
                OnDecayStart?.Invoke();
            }
        }
        modifiers.Add(modifier);
    }

    /// <summary>Lets a modifier that is in the list of modifiers modify current</summary>
    public void ModifyCurrent(HealthModifier modifier, int value)
    {
        if (modifiers.Contains(modifier))
        {
            if (value > 0 && current != max)
            {
                current += value;
                if (current > max)
                {
                    current = max;
                }
            }
            else if (value < 0 && current != 0)
            {
                current += value;
                if (current < 0)
                {
                    current = 0;
                }
            }
        }
    }

    /// <summary>Lets a modifier that is in the list of modifiers modify max</summary>
    public void ModifyMax(HealthModifier modifier, int value)
    {
        if (modifiers.Contains(modifier))
        {
            if (value > 0)
            {
                max += value;
            }
            else if (value < 0 && max != minimalMax)
            {
                max += value;
                if (max < minimalMax)
                {
                    max = minimalMax;
                }
            }
        }
    }

    /// <summary>Adds given health system to the set of stored priority systems</summary>
    public void AddPrioritySystem(HealthSystem system)
    {
        highPrioritySystems.Add(system);
    }

    /// <summary>removes given health system from the priority systems set</summary>
    public void RemovePrioritySystem(HealthSystem system)
    {
        highPrioritySystems.Remove(system);
    }
}