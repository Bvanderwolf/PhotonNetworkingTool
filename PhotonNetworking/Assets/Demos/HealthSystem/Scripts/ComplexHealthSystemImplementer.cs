using UnityEngine;
using UnityEngine.UI;

public class ComplexHealthSystemImplementer : MonoBehaviour
{
    [Header("Health Systems")]
    [SerializeField]
    private HealthSystem hitPointSystem;

    [SerializeField]
    private HealthSystem energySystem;

    [SerializeField]
    private HealthSystem thirstSystem;

    [Header("Inputs")]
    [SerializeField]
    private InputField healthInput;

    [SerializeField]
    private InputField timeInput;

    [SerializeField]
    private Toggle regenToggle;

    [SerializeField]
    private Toggle currentToggle;

    [SerializeField]
    private Dropdown systemChoice;

    [SerializeField]
    private Button modifyHealthButton;

    [Header("Weather")]
    [SerializeField]
    private Toggle hotWeatherToggle;

    private enum SystemChoice { Hitpoints, Energy, Thirst }

    private void Awake()
    {
        hitPointSystem.SetCurrentToMax();
        energySystem.SetCurrentToMax();
        thirstSystem.SetCurrentToMax();
        regenToggle.onValueChanged.AddListener(OnRegenToggled);
        modifyHealthButton.onClick.AddListener(OnModifyHealthButtonClick);
    }

    private void Update()
    {
        hitPointSystem.Update();
        energySystem.Update();
        thirstSystem.Update();
    }

    public void OnModifyHealthButtonClick()
    {
        if (string.IsNullOrEmpty(healthInput.text) || string.IsNullOrEmpty(timeInput.text))
            return;

        int value = int.Parse(healthInput.text);
        float time = float.Parse(timeInput.text);
        SystemChoice choice = (SystemChoice)systemChoice.value;
        switch (choice)
        {
            case SystemChoice.Hitpoints:
                hitPointSystem.AddModifier(new TimedHealthModifier("", time, value, regenToggle.isOn, currentToggle.isOn, true, true));
                break;

            case SystemChoice.Energy:
                energySystem.AddModifier(new TimedHealthModifier("", time, value, regenToggle.isOn, currentToggle.isOn, true, true));
                break;

            case SystemChoice.Thirst:
                thirstSystem.AddModifier(new TimedHealthModifier("", time, value, regenToggle.isOn, currentToggle.isOn, true, true));
                break;
        }
    }

    public void OnRegenToggled(bool value)
    {
        string text = value ? "Regenerate" : "Decay";
        modifyHealthButton.GetComponentInChildren<Text>().text = text;
    }

    public void OnHotWeatherToggle(bool value)
    {
        if (value)
        {
            thirstSystem.AddModifier(new ConditionalHealthModifier("", 2, false, true, true, true, () => !hotWeatherToggle.isOn));
        }
    }
}