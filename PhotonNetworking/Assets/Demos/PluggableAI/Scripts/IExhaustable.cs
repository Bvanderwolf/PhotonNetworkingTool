using UnityEngine.UI;

public interface IExhaustable
{
    void AddEnergyFeedback(Image fillableImage, Text energyText = null);
}