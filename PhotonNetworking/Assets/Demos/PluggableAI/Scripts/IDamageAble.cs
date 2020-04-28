using UnityEngine.UI;

public interface IDamageAble
{
    void Damage(HealthModifier modifier);

    void AddDamageFeedback(Image fillableImage, Text hitpointText = null);
}