using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/ChaseActionDog")]
public class ChaseActionDog : ChaseAction
{
    public override void Act(AIStateController controller)
    {
        ChaseDataContainer chase = (ChaseDataContainer)controller.GetData(AIStateDataType.Chase);
        chase.UpdateIntervalTime(this, controller);
    }

    public override void Begin(AIStateController controller)
    {
        //set animation speed
    }

    public override void End(AIStateController controller)
    {
        //reste animation speed
    }
}