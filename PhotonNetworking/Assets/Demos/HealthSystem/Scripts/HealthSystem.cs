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

        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            modifiers[i].Modify(this);
            if (modifiers[i].Finished)
            {
                modifiers.RemoveAt(i);
            }
        }

        healthBar.fillAmount = current / max;
        if (showText && healthText != null)
        {
            healthText.text = $"{Current}/{max}";
        }
    }

    public void SetCurrentToMax()
    {
        current = max;
    }

    public void AddModifier(HealthModifier modifier)
    {
        modifiers.Add(modifier);
    }

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

    public void AddPrioritySystem(HealthSystem system)
    {
        highPrioritySystems.Add(system);
    }

    public void RemovePrioritySystem(HealthSystem system)
    {
        highPrioritySystems.Remove(system);
    }
}