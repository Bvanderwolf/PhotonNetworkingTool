using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VeterancySystem
{
    public enum VeterancyType
    {
        Fishing,
        Fighting,
        Cooking
    }

    public readonly Dictionary<VeterancyType, Veterancy> Veterancies = new Dictionary<VeterancyType, Veterancy>();

    [SerializeField]
    private Veterancy fishingVeterancy;

    [SerializeField]
    private Veterancy fightingVeterancy;

    [SerializeField]
    private Veterancy cookingVeterancy;

    public void Init()
    {
        //add veterancies to dictionary
        Veterancies.Add(VeterancyType.Fishing, fishingVeterancy);
        Veterancies.Add(VeterancyType.Fighting, fightingVeterancy);
        Veterancies.Add(VeterancyType.Cooking, cookingVeterancy);

        //initialize all veterancies
        foreach (var veterancy in Veterancies)
        {
            veterancy.Value.Init();
        }
    }

    public void Update()
    {
        //update veterancy system their health systems
        foreach (var veterancy in Veterancies)
        {
            veterancy.Value.System.Update();
        }
    }

    /// <summary>Increases given veterancy by given value</summary>
    public void IncreaseVeterancy(VeterancyType veterancyType, int value)
    {
        Veterancies[veterancyType].System.AddModifier(new TimedHealthModifier("", 0, value, true, true, true));
    }

    /// <summary>Returns a list of available actions for given veterancy</summary>
    public List<AIAction> GetActionsOfVeterancy(VeterancyType veterancyType)
    {
        return Veterancies[veterancyType].GetAvailableActions();
    }
}