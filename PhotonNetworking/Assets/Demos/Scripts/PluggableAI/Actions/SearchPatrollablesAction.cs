using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol/Search")]
public class SearchPatrollablesAction : AIAction
{
    public override void Act(AIDataContainer data)
    {
        //walk towards a random direction (Random.InsideUnitCirkel)
    }
}