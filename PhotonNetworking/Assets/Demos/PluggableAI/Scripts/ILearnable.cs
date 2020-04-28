using UnityEngine.UI;

public interface ILearnable
{
    void AddExperienceFeedback(Image fillableImage, Text experienceText = null);
}