using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
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
    private List<HealthModifier> activeModifiers = new List<HealthModifier>();
    private List<HealthModifier> queuedModifiers = new List<HealthModifier>();

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
            return activeModifiers.Count != 0;
        }
    }

    public void Update()
    {
        //if there are systems with higher priority being modified we dont update
        if (highPrioritySystems.Any(system => system.BeingModified))
        {
            return;
        }

        bool currentIsMax = current == max;
        bool currentIsZero = current == 0f;

        //let each modifier modify this system and remove it if it is finished giving callbacks if the condition is right
        for (int i = activeModifiers.Count - 1; i >= 0; i--)
        {
            HealthModifier modifier = activeModifiers[i];
            modifier.Modify(this);

            bool finished = modifier.Finished;
            if (finished)
            {
                if (queuedModifiers.Count != 0 && queuedModifiers.Any(m => m.Name == modifier.Name))
                {
                    //if there are queued modifiers an one has the same name as this one, replace this one with the queued one
                    for (int j = queuedModifiers.Count - 1; j >= 0; j--)
                    {
                        if (queuedModifiers[j].Name == modifier.Name)
                        {
                            activeModifiers[i] = queuedModifiers[j];
                            queuedModifiers.RemoveAt(j);
                            break;
                        }
                    }
                }
                else
                {
                    //if this modifier cannot be replaced by a queued one, remove it
                    activeModifiers.RemoveAt(i);
                }

                if (activeModifiers.Count(m => m.Regenerate && m.IsOverTime) == 0 && modifier.Regenerate && modifier.IsOverTime)
                {
                    //if there are no more regenerating over time modifiers and this one (which was removed) was, the regeneration has ended
                    OnRegenEnd?.Invoke();
                }
                else if (activeModifiers.Count(m => !m.Regenerate && m.IsOverTime) == 0 && !modifier.Regenerate && modifier.IsOverTime)
                {
                    //if there are no more decay over time modifiers and this one (which was removed) was, the decaying has ended
                    OnDecayEnd?.Invoke();
                }

                if (finished)
                {
                    //if the modifier is finished (and thus removed from the list) it can be cleaned up
                    modifier = null;
                }
            }
        }

        //show feedback of data on healthbar and text if showable
        if (healthBar != null)
        {
            healthBar.fillAmount = current / max;
        }
        if (showText && healthText != null)
        {
            healthText.text = $"{Current}/{max}";
        }

        //if current was not equal to max before modification but is now equal after modification, on reached max is called
        if (!currentIsMax && current == max)
        {
            OnReachedMax?.Invoke();
        }

        //if current was not equal to zero before modification but is now equal after modification, on reached zero is called
        if (!currentIsZero && current == 0f)
        {
            OnReachedZero?.Invoke();
        }
    }

    /// <summary>Sets current to max without callbacks</summary>
    public void SetCurrentToMax()
    {
        current = max;
    }

    /// <summary>Sets current to zero without callbacks</summary>
    public void SetCurrentToZero()
    {
        current = 0;
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

            if (activeModifiers.Count(m => m.Regenerate && m.IsOverTime) == 0 && timed.Regenerate && timed.IsOverTime)
            {
                OnRegenStart?.Invoke();
            }
            else if (activeModifiers.Count(m => !m.Regenerate && m.IsOverTime) == 0 && !timed.Regenerate && timed.IsOverTime)
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

            if (activeModifiers.Count(m => m.Regenerate && m.IsOverTime) == 0 && conditional.Regenerate)
            {
                OnRegenStart?.Invoke();
            }
            else if (activeModifiers.Count(m => !m.Regenerate && m.IsOverTime) == 0 && !conditional.Regenerate)
            {
                OnDecayStart?.Invoke();
            }
        }

        if (!modifier.CanStack && activeModifiers.Any(m => m.Name == modifier.Name))
        {
            queuedModifiers.Add(modifier);
        }
        else
        {
            activeModifiers.Add(modifier);
        }
    }

    /// <summary>Lets a modifier that is in the list of modifiers modify current</summary>
    public void ModifyCurrent(HealthModifier modifier, int value)
    {
        if (activeModifiers.Contains(modifier))
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
        if (activeModifiers.Contains(modifier))
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
                if (current > max)
                {
                    current = max;
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

    /// <summary>Sets health bar reference if given image is of type filled</summary>
    public void AttachHealthBar(Image healthBar)
    {
        if (healthBar != null && healthBar.type == Image.Type.Filled)
        {
            this.healthBar = healthBar;
        }
    }

    /// <summary>Sets health text reference</summary>
    public void AttachHealthText(Text healthText)
    {
        if (healthText != null)
        {
            this.healthText = healthText;
        }
    }
}