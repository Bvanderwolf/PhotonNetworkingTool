using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Veterancy
{
    public enum ActionUpgradeType
    {
        SpeedIncrease,
        EfficiencyIncrease
    }

    [System.Serializable]
    private struct VeterancyAction
    {
        public int VeterancyLevel;
        public AIAction Action;
        public ActionUpgradeType upgradeType;
    }

    [SerializeField]
    private HealthSystem system;

    [SerializeField]
    private float multiplierOnVeterancyUp;

    [SerializeField]
    private VeterancyAction[] actions;

    public int VeterancyLevel { get; private set; } = 1;

    public HealthSystem System
    {
        get
        {
            return system;
        }
    }

    public void Init()
    {
        system.OnReachedMax += OnVeterancyUp;
    }

    /// <summary>Returns the available actions based on veterancy level</summary>
    public List<AIAction> GetAvailableActions()
    {
        List<AIAction> a = new List<AIAction>();
        foreach (var action in actions)
        {
            if (action.VeterancyLevel <= VeterancyLevel)
            {
                a.Add(action.Action);
            }
        }
        return a;
    }

    /// <summary>when the system reaches max the veterancyLevel goes up and the system is reset with a higher max value</summary>
    private void OnVeterancyUp()
    {
        VeterancyLevel++;
        system.SetCurrentToZero();
        system.AddModifier(new TimedHealthModifier(0f, Mathf.RoundToInt(system.Max * multiplierOnVeterancyUp), true, false));
    }
}