using UnityEngine;
using UnityEngine.UI;

public class SimpleHealthSystemImplementer : MonoBehaviour
{
    [SerializeField]
    private HealthSystem healthSystem;

    [SerializeField]
    private InputField healthInput;

    [SerializeField]
    private InputField timeInput;

    [SerializeField]
    private Toggle regenToggle;

    [SerializeField]
    private Toggle currentToggle;

    private void Awake()
    {
        this.healthSystem.SetCurrentToMax();
    }

    private void Update()
    {
        this.healthSystem.Update();
    }

    public void OnModifyHealthButtonClick()
    {
        if (string.IsNullOrEmpty(healthInput.text) || string.IsNullOrEmpty(timeInput.text))
            return;

        int value = int.Parse(healthInput.text);
        float time = float.Parse(timeInput.text);
        healthSystem.AddModifier(new TimedHealthModifier(time, value, regenToggle.isOn, currentToggle.isOn));
    }
}